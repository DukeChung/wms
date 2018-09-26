using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    /// <summary>
    /// 上架
    /// </summary>
    public class ShelvesAppService : WMSApplicationService, IShelvesAppService
    {
        private ICrudRepository _crudRepository = null;
        private IShelvesRepository _shelvesRepository = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IPackageAppService _packageAppService = null;
        public ShelvesAppService(ICrudRepository crudRepository, IShelvesRepository shelvesRepository, IWMSSqlRepository wmsSqlRepository, IPackageAppService packageAppService)
        {
            this._crudRepository = crudRepository;
            _shelvesRepository = shelvesRepository;
            this._WMSSqlRepository = wmsSqlRepository;
            _packageAppService = packageAppService;
        }

        /// <summary>
        /// 获取待上架收货单
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingShelvesListDto> GetWaitingShelvesList(RFShelvesQuery shelvesQuery)
        {
            _crudRepository.ChangeDB(shelvesQuery.WarehouseSysId);
            return _shelvesRepository.GetWaitingShelvesList(shelvesQuery);
        }

        /// <summary>
        /// 获取某个单据的待上架商品
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public List<RFWaitingShelvesSkuListDto> GetWaitingShelvesSkuList(RFShelvesQuery shelvesQuery)
        {
            _crudRepository.ChangeDB(shelvesQuery.WarehouseSysId);
            return _shelvesRepository.GetWaitingShelvesSkuList(shelvesQuery);
        }

        /// <summary>
        /// 检查商品是否存在于收货明细中
        /// </summary>
        /// <returns></returns>
        public RFCommResult CheckReceiptDetailSku(ScanShelvesDto scanShelvesDto)
        {
            _crudRepository.ChangeDB(scanShelvesDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var receipt = _crudRepository.GetQuery<receipt>(x => x.ReceiptOrder == scanShelvesDto.ReceiptOrder && x.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (receipt == null)
                {
                    throw new Exception("收货单不存在");
                }

                sku sku = null;
                if (scanShelvesDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == scanShelvesDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == scanShelvesDto.UPC).ToList();

                    var query = from a in receipt.receiptdetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                var rdList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receipt.SysId && x.SkuSysId == sku.SysId
                && (x.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || x.ShelvesStatus == (int)ShelvesStatus.Shelves)
                        && (x.Status == (int)ReceiptStatus.Received)).ToList();

                if (!rdList.Any())
                {
                    throw new Exception("收货单中没有待上架的此商品");
                }

                var totalWaitQty = rdList.Sum(x => x.ReceivedQty - x.ShelvesQty);
                if (scanShelvesDto.Qty > totalWaitQty)
                {
                    throw new Exception("上架数量大于待上架商品数量");
                }

                result.IsSucess = true;
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取收货明细推荐货位
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        public string GetAdviceToLoc(RFShelvesQuery shelvesQuery)
        {
            _crudRepository.ChangeDB(shelvesQuery.WarehouseSysId);
            return _shelvesRepository.GetAdviceToLoc(shelvesQuery);
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        public List<RFInventoryListDto> GetInventoryList(RFInventoryQuery inventoryQuery)
        {
            _crudRepository.ChangeDB(inventoryQuery.WarehouseSysId);
            var response = _shelvesRepository.GetInventoryList(inventoryQuery);

            //gavin: 原材料单位反转
            //if (response != null && response.Count > 0)
            //{
            //    foreach (var item in response)
            //    {
            //        decimal transQty = 0;
            //        if (_packageAppService.GetSkuDeconversiontransQty(item.SkuSysId, item.Qty, out transQty) == true)
            //        {
            //            item.DisplayQty = transQty;
            //        }
            //        else
            //        {
            //            item.DisplayQty = item.Qty;
            //        }
            //    }
            //}

            return response;
        }

        /// <summary>
        /// 扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        public RFCommResult ScanShelves(ScanShelvesDto scanShelvesDto)
        {
            _crudRepository.ChangeDB(scanShelvesDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                scanShelvesDto.Loc = scanShelvesDto.Loc.Trim();
                scanShelvesDto.Lot = scanShelvesDto.Lot != null ? scanShelvesDto.Lot.Trim() : scanShelvesDto.Lot;
                var loc = _crudRepository.GetQuery<location>(x => x.Loc == scanShelvesDto.Loc && x.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (loc == null)
                {
                    throw new Exception("货位不存在");
                }
                scanShelvesDto.Loc = loc.Loc;

                if (loc.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"货位{loc.Loc}已被冻结，不能上架!");
                }

                var receipt = _crudRepository.GetQuery<receipt>(x => x.ReceiptOrder == scanShelvesDto.ReceiptOrder && x.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (receipt == null)
                {
                    throw new Exception("收货单不存在");
                }

                if (receipt.Status != (int)ReceiptStatus.Received)
                {
                    throw new Exception("收货单状态不等于收货完成,无法进行上架");
                }

                sku sku = null;

                if (scanShelvesDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == scanShelvesDto.SkuSysId).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == scanShelvesDto.UPC).ToList();

                    var query = from a in receipt.receiptdetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                //冻结校验
                var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && p.SkuSysId.Value == sku.SysId
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (frozenSku != null)
                {
                    throw new Exception($"商品已被冻结，不能做上架!");
                }

                var frozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == sku.SysId
                              && p.Loc == scanShelvesDto.Loc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                //校验冻结: 货位商品 
                if (frozenLocSku != null)
                {
                    var skuSysId = frozenLocSku.SkuSysId;
                    var frozenSkuInfo = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSkuInfo.SkuName}'在货位'{scanShelvesDto.Loc}'已被冻结，不能做上架!");
                }

                var rdList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receipt.SysId && x.SkuSysId == sku.SysId
                && (x.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || x.ShelvesStatus == (int)ShelvesStatus.Shelves)
                        && (x.Status == (int)ReceiptStatus.Received)).ToList();

                if (!rdList.Any())
                {
                    throw new Exception("收货单中没有待上架的此商品");
                }
                if (rdList.Any(x => x.ToLot == null || x.ToLot == ""))
                {
                    throw new Exception("此商品必须先采集批次才能上架");
                }

                if (!string.IsNullOrEmpty(scanShelvesDto.Lot))
                {
                    rdList = rdList.Where(x => x.ToLot == scanShelvesDto.Lot).ToList();
                    if (!rdList.Any())
                    {
                        throw new Exception("收货单中没有待上架的此批次的此商品");
                    }
                }

                var totalWaitQty = rdList.Sum(x => x.ReceivedQty - x.ShelvesQty);

                //gavin: 此处增加单位转换数量，用于更新库存表
                int transQty = 0;
                pack transPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(sku.SysId, scanShelvesDto.InputQty, out transQty, ref transPack) == true)
                {
                    //gavin: 单位转换更新库存后需要记录
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        DocOrder = receipt.ReceiptOrder,
                        DocSysId = receipt.SysId,
                        DocDetailSysId = Guid.Empty,
                        SkuSysId = sku.SysId,
                        FromQty = scanShelvesDto.InputQty,
                        ToQty = transQty,
                        Loc = scanShelvesDto.Loc,
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transPack.SysId,
                        PackCode = transPack.PackCode,
                        FromUOMSysId = transPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = scanShelvesDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.Inbound,
                        SourceTransType = InvSourceTransType.Shelve
                    };
                    _crudRepository.Insert(unitTran);

                    scanShelvesDto.Qty = transQty;
                }
                else
                {
                    scanShelvesDto.Qty = Convert.ToInt32(scanShelvesDto.InputQty);
                }

                if (scanShelvesDto.Qty > totalWaitQty)
                {
                    throw new Exception("上架数量大于待上架商品数量");
                }

                var shelvesQty = scanShelvesDto.Qty;

                var invLotList = new List<invlot>();
                var invSkuLocList = new List<invskuloc>();
                var invLotLocLpnList = new List<invlotloclpn>();

                var updateInventoryDtos = new List<UpdateInventoryDto>();
                var totalShelvesQty = 0;

                foreach (var item in rdList)
                {
                    var rd = _crudRepository.Get<receiptdetail>(item.SysId);
                    if (shelvesQty <= 0)
                    {
                        break;
                    }

                    //待上架数量
                    var waitQty = (int)rd.ReceivedQty - rd.ShelvesQty;

                    //入库存数量
                    var invQty = 0;

                    if (waitQty > shelvesQty)
                    {
                        rd.ShelvesQty = rd.ShelvesQty + shelvesQty;
                        rd.ShelvesStatus = (int)ShelvesStatus.Shelves;
                        invQty = shelvesQty;
                        shelvesQty = 0;
                    }
                    else
                    {
                        rd.ShelvesQty = rd.ShelvesQty + waitQty;
                        shelvesQty = shelvesQty - waitQty;
                        rd.ShelvesStatus = (int)ShelvesStatus.Finish;
                        invQty = waitQty;
                    }

                    totalShelvesQty += invQty;

                    rd.TS = Guid.NewGuid();
                    rd.UpdateBy = scanShelvesDto.CurrentUserId;
                    rd.UpdateDate = DateTime.Now;
                    rd.UpdateUserName = scanShelvesDto.CurrentDisplayName;
                    _crudRepository.Update(rd);

                    #region InvLot
                    var invLot = _crudRepository.GetQuery<invlot>(x => x.Lot == rd.ToLot && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                    if (invLot != null)
                    {
                        //var oldInvLot = _crudRepository.Get<invlot>(invLot.SysId);
                        //oldInvLot.Qty += invQty;
                        //oldInvLot.UpdateBy = scanShelvesDto.UserId;
                        //oldInvLot.UpdateDate = DateTime.Now;
                        //oldInvLot.UpdateUserName = scanShelvesDto.CurrentDisplayName;
                        //_crudRepository.Update(oldInvLot);
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = new Guid(),
                            InvLotSysId = invLot.SysId,
                            InvSkuLocSysId = new Guid(),
                            Qty = invQty,
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        if (!invLotList.Any(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                        {
                            var newInvLot = new invlot()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = scanShelvesDto.WarehouseSysId,
                                Lot = rd.ToLot,
                                SkuSysId = rd.SkuSysId,
                                CaseQty = 0,
                                InnerPackQty = 0,
                                Qty = invQty,
                                AllocatedQty = 0,
                                PickedQty = 0,
                                HoldQty = 0,
                                Status = 1,
                                Price = rd.Price != null ? (decimal)rd.Price : 0,
                                CreateBy = scanShelvesDto.UserId,
                                CreateDate = DateTime.Now,
                                UpdateBy = scanShelvesDto.UserId,
                                UpdateDate = DateTime.Now,
                                CreateUserName = scanShelvesDto.CurrentDisplayName,
                                LotAttr01 = rd.LotAttr01,
                                LotAttr02 = rd.LotAttr02,
                                LotAttr03 = rd.LotAttr03,
                                LotAttr04 = rd.LotAttr04,
                                LotAttr05 = rd.LotAttr05,
                                LotAttr06 = rd.LotAttr06,
                                LotAttr07 = rd.LotAttr07,
                                LotAttr08 = rd.LotAttr08,
                                LotAttr09 = rd.LotAttr09,
                                ReceiptDate = rd.ReceivedDate,
                                ProduceDate = rd.ProduceDate,
                                ExpiryDate = rd.ExpiryDate,
                                ExternalLot = rd.ExternalLot
                            };
                            invLotList.Add(newInvLot);
                        }
                        else
                        {
                            var oldInvLot = invLotList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                            oldInvLot.Qty += invQty;
                        }
                    }
                    #endregion

                    #region InvSkuLoc
                    var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.Loc == scanShelvesDto.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                    if (invSkuLoc != null)
                    {
                        //var oldSkuLoc = _crudRepository.Get<invskuloc>(invSkuLoc.SysId);
                        //oldSkuLoc.Qty += invQty;
                        //oldSkuLoc.UpdateBy = scanShelvesDto.UserId;
                        //oldSkuLoc.UpdateDate = DateTime.Now;
                        //oldSkuLoc.UpdateUserName = scanShelvesDto.CurrentDisplayName;
                        //_crudRepository.Update(oldSkuLoc);
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = new Guid(),
                            InvLotSysId = new Guid(),
                            InvSkuLocSysId = invSkuLoc.SysId,
                            Qty = invQty,
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        if (!invSkuLocList.Any(x => x.Loc == scanShelvesDto.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                        {
                            var newInvSkuLoc = new invskuloc()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = scanShelvesDto.WarehouseSysId,
                                SkuSysId = rd.SkuSysId,
                                Loc = scanShelvesDto.Loc,
                                Qty = invQty,
                                AllocatedQty = 0,
                                PickedQty = 0,
                                CreateBy = scanShelvesDto.UserId,
                                CreateDate = DateTime.Now,
                                UpdateBy = scanShelvesDto.UserId,
                                UpdateDate = DateTime.Now,
                                CreateUserName = scanShelvesDto.CurrentDisplayName
                            };
                            invSkuLocList.Add(newInvSkuLoc);
                        }
                        else
                        {
                            var oldInvSkuLoc = invSkuLocList.Where(x => x.Loc == scanShelvesDto.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                            oldInvSkuLoc.Qty += invQty;
                        }
                    }
                    #endregion

                    #region InvLotLocLpn
                    var invLotLocLpn = _crudRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == scanShelvesDto.Loc && x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                    if (invLotLocLpn != null)
                    {
                        //var oldInvLotLocLpn = _crudRepository.Get<invlotloclpn>(invLotLocLpn.SysId);
                        //invLotLocLpn.Qty += invQty;
                        //invLotLocLpn.UpdateBy = scanShelvesDto.UserId;
                        //invLotLocLpn.UpdateDate = DateTime.Now;
                        //invLotLocLpn.UpdateUserName = scanShelvesDto.CurrentDisplayName;
                        //_crudRepository.Update(invLotLocLpn);
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = invLotLocLpn.SysId,
                            InvLotSysId = new Guid(),
                            InvSkuLocSysId = new Guid(),
                            Qty = invQty,
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        if (!invLotLocLpnList.Any(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == scanShelvesDto.Loc && x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                        {
                            var newInvLotLocLpn = new invlotloclpn()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = scanShelvesDto.WarehouseSysId,
                                SkuSysId = rd.SkuSysId,
                                Loc = scanShelvesDto.Loc,
                                Lot = rd.ToLot,
                                Lpn = "",
                                Qty = invQty,
                                AllocatedQty = 0,
                                PickedQty = 0,
                                Status = 1,
                                CreateBy = scanShelvesDto.UserId,
                                CreateDate = DateTime.Now,
                                UpdateBy = scanShelvesDto.UserId,
                                UpdateDate = DateTime.Now,
                                CreateUserName = scanShelvesDto.CurrentDisplayName
                            };
                            invLotLocLpnList.Add(newInvLotLocLpn);
                        }
                        else
                        {
                            var oldInvLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == scanShelvesDto.Loc && x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                            oldInvLotLocLpn.Qty += invQty;
                        }
                    }
                    #endregion

                    var pack = _crudRepository.Get<pack>((Guid)rd.PackSysId);
                    var uom = _crudRepository.Get<uom>((Guid)rd.UOMSysId);

                    #region InvTrans
                    var invTrans = new invtran()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        DocOrder = receipt.ReceiptOrder,
                        DocSysId = receipt.SysId,
                        DocDetailSysId = rd.SysId,
                        SkuSysId = rd.SkuSysId,
                        SkuCode = sku.SkuCode,
                        TransType = InvTransType.Inbound,
                        SourceTransType = InvSourceTransType.Shelve,
                        Qty = invQty,
                        Loc = scanShelvesDto.Loc,
                        Lot = rd.ToLot,
                        Lpn = "",
                        ToLoc = scanShelvesDto.Loc,
                        ToLot = rd.ToLot,
                        ToLpn = "",
                        Status = InvTransStatus.Ok,
                        LotAttr01 = rd.LotAttr01,
                        LotAttr02 = rd.LotAttr02,
                        LotAttr03 = rd.LotAttr03,
                        LotAttr04 = rd.LotAttr04,
                        LotAttr05 = rd.LotAttr05,
                        LotAttr06 = rd.LotAttr06,
                        LotAttr07 = rd.LotAttr07,
                        LotAttr08 = rd.LotAttr08,
                        LotAttr09 = rd.LotAttr09,
                        ExternalLot = rd.ExternalLot,
                        ProduceDate = rd.ProduceDate,
                        ExpiryDate = rd.ExpiryDate,
                        ReceivedDate = rd.ReceivedDate,
                        PackSysId = (Guid)rd.PackSysId,
                        PackCode = pack != null ? pack.PackCode : "",
                        UOMSysId = (Guid)rd.UOMSysId,
                        UOMCode = uom != null ? uom.UOMCode : "",
                        CreateBy = scanShelvesDto.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.UserId,
                        UpdateDate = DateTime.Now,
                        CreateUserName = scanShelvesDto.CurrentDisplayName
                    };
                    _crudRepository.Insert(invTrans);

                    #endregion
                }

                _crudRepository.BatchInsert(invLotList);
                _crudRepository.BatchInsert(invSkuLocList);
                _crudRepository.BatchInsert(invLotLocLpnList);

                //执行扣减库存方法(上架)
                _WMSSqlRepository.UpdateInventoryQtyByShelves(updateInventoryDtos);

                //获取入库单明细
                var receiptDetailList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receipt.SysId).ToList();
                receiptDetailList = receiptDetailList.Where(x => x.ReceivedQty != x.ShelvesQty).ToList();
                if (receiptDetailList == null || receiptDetailList.Count == 0)
                {
                    #region 组织推送拣货完成工单数据
                    if (receipt != null)
                    {
                        var mqWorkDto = new MQWorkDto()
                        {
                            WorkBusinessType = (int)WorkBusinessType.Update,
                            WorkType = (int)UserWorkType.Shelve,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            CancelWorkDto = new CancelWorkDto()
                            {
                                DocSysIds = new List<Guid>() { receipt.SysId },
                                Status = (int)WorkStatus.Finish
                            }
                        };

                        var workProcessDto = new MQProcessDto<MQWorkDto>()
                        {
                            BussinessSysId = receipt.SysId,
                            BussinessOrderNumber = receipt.ReceiptOrder,
                            Descr = "",
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            BussinessDto = mqWorkDto
                        };
                        //推送工单数据
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                    }
                    #endregion
                }

                result.IsSucess = true;
                result.Message = "上架成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 自动上架
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public RFCommResult AutoShelves(ScanShelvesDto scanShelvesDto)
        {
            _crudRepository.ChangeDB(scanShelvesDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };

            try
            {
                var receipt = _crudRepository.GetQuery<receipt>(x => x.ReceiptOrder == scanShelvesDto.ReceiptOrder && x.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (receipt == null)
                {
                    throw new Exception("收货单不存在");
                }
                if (receipt.Status != (int)ReceiptStatus.Received)
                {
                    throw new Exception("收货单状态不等于收货完成,无法进行上架");
                }
                var purchase = _crudRepository.GetQuery<purchase>(x => x.PurchaseOrder == receipt.ExternalOrder).FirstOrDefault();
                if (purchase == null)
                {
                    throw new Exception("对应采购单不存在，无法找到渠道进行自动上架");
                }

                var receiptDetail = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receipt.SysId && x.ShelvesStatus != (int)ShelvesStatus.Finish).ToList();

                if (receiptDetail.Any())
                {
                    var skuSysIdList = receiptDetail.Select(x => x.SkuSysId).Distinct().ToList();
                    var invLocs = _shelvesRepository.GetSkuLocBySkuSysIds(skuSysIdList, scanShelvesDto.WarehouseSysId, purchase.Channel);
                    if (!invLocs.Any())
                    {
                        throw new Exception("收货单商品没有历史上架记录,无法进行自动上架");
                    }

                    var updateInventoryDtos = new List<UpdateInventoryDto>();
                    var invLotList = new List<invlot>();
                    //var invSkuLocList = new List<invskuloc>();
                    var invLotLocLpnList = new List<invlotloclpn>();
                    var invtranList = new List<invtran>();
                    var updateReceiptDetailList = new List<UpdateReceiptDetailDto>();

                    var skus = _crudRepository.GetQuery<sku>(p => skuSysIdList.Contains(p.SysId)).ToList();
                    var packSysIds = receiptDetail.Select(p => p.PackSysId);
                    var packs = _crudRepository.GetQuery<pack>(p => packSysIds.Contains(p.SysId)).ToList();
                    var uomSysIds = receiptDetail.Select(p => p.UOMSysId);
                    var uoms = _crudRepository.GetQuery<uom>(p => uomSysIds.Contains(p.SysId)).ToList();

                    var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuSysIdList.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId);
                    if (frozenSkuList.Count() > 0)
                    {
                        var skuSysId = frozenSkuList.First().SkuSysId;
                        var frozenSku = skus.First(p => p.SysId == skuSysId);
                        throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能自动上架!");
                    }

                    var inventorySkus = new List<GetInventoryDto>();
                    foreach (var item in receiptDetail)
                    {
                        var sku = skus.FirstOrDefault(p => p.SysId == item.SkuSysId);
                        if (item.ToLot == null || item.ToLot == "")
                        {
                            throw new Exception(string.Format("商品：{0}，必须先采集批次才能上架", sku.SkuName));
                        }
                        var skuLoc = invLocs.Where(x => x.SkuSysId == item.SkuSysId && x.LocationStatus == (int)LocationStatus.Normal && x.Loc != PublicConst.PickingSkuLoc).ToList();
                        if (skuLoc != null && skuLoc.Count > 0)
                        {
                            var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && item.SkuSysId == p.SkuSysId.Value
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId).ToList();
                            //校验冻结: 货位商品 
                            if (frozenLocSkuList.Count > 0)
                            {
                                var locskuFrozenQuery = from T1 in skuLoc
                                                        join T2 in frozenLocSkuList on new { SkuSysId = T1.SkuSysId.Value, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                                        select T2;

                                if (locskuFrozenQuery.Count() > 0)
                                {
                                    foreach (var locsku in locskuFrozenQuery.ToList())
                                    {
                                        skuLoc.Remove(skuLoc.FirstOrDefault(p => p.SkuSysId == locsku.SkuSysId && p.Loc == locsku.Loc));
                                    }
                                }
                            }

                            if (skuLoc.Count > 0)
                            {
                                scanShelvesDto.Loc = skuLoc.First().Loc;
                            }
                            else
                            {
                                throw new Exception("收货单商品" + sku.SkuName + "历史上架货位已被冻结,无法进行自动上架.");
                            }
                        }
                        else
                        {
                            if (invLocs.FirstOrDefault(x => x.SkuSysId == item.SkuSysId && x.LocationStatus == (int)LocationStatus.Frozen) != null)
                            {
                                throw new Exception("收货单商品" + sku.SkuName + "历史上架货位已被冻结,无法进行自动上架.");
                            }
                            else
                            {
                                throw new Exception("收货单商品" + sku.SkuName + "没有历史上架记录,无法进行自动上架.");
                            }
                        }
                        inventorySkus.Add(new GetInventoryDto { SkuSysId = item.SkuSysId, Lot = item.ToLot, Loc = scanShelvesDto.Loc, Lpn = "", WareHouseSysId = scanShelvesDto.WarehouseSysId });
                    }
                    //批量获取库存信息
                    var invLots = _WMSSqlRepository.BatchGetInvLot(inventorySkus);
                    var invSkuLocs = _WMSSqlRepository.BatchGetInvSkuLoc(inventorySkus);
                    var invLotLocLpns = _WMSSqlRepository.BatchGetInvLotLocLpn(inventorySkus);

                    foreach (var info in receiptDetail)
                    {
                        var sku = skus.FirstOrDefault(p => p.SysId == info.SkuSysId);
                        var skuLoc = invSkuLocs.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                        if (skuLoc != null)
                        {
                            scanShelvesDto.Loc = skuLoc.Loc;
                        }
                        else
                        {
                            throw new Exception("收货单商品" + sku.SkuName + "没有历史上架记录,无法进行自动上架.");
                        }
                        if (info.ShelvesStatus == (int)ShelvesStatus.NotOnShelves) //未上架
                        {
                            scanShelvesDto.InputQty = Convert.ToDecimal(info.ReceivedQty);
                        }
                        else
                        {
                            scanShelvesDto.InputQty = Convert.ToDecimal(info.ReceivedQty) -
                                                      Convert.ToDecimal(info.ShelvesQty);
                        }

                        if (scanShelvesDto.InputQty == 0)
                        {
                            throw new Exception("自动上架异常,商品UPC" + sku.UPC + "待上架数量为0,无法上架!");
                        }
                        int transQty = 0;
                        var shelvesQty = Convert.ToInt32(scanShelvesDto.InputQty);

                        updateReceiptDetailList.Add(
                            new UpdateReceiptDetailDto
                            {
                                SysId = info.SysId,
                                ShelvesQty = info.ShelvesQty + shelvesQty,
                                ShelvesStatus = (int)ShelvesStatus.Finish,
                                OldTS = info.TS,
                                NewTS = Guid.NewGuid(),
                                CurrentUserId = scanShelvesDto.CurrentUserId,
                                CurrentDisplayName = scanShelvesDto.CurrentDisplayName
                            });

                        #region InvLot

                        var invLot = invLots.FirstOrDefault(x => x.Lot == info.ToLot && x.SkuSysId == info.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId);
                        if (invLot != null)
                        {
                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = new Guid(),
                                InvLotSysId = invLot.SysId,
                                InvSkuLocSysId = new Guid(),
                                Qty = shelvesQty,
                                CurrentUserId = scanShelvesDto.CurrentUserId,
                                CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                                WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            });
                        }
                        else
                        {
                            if (
                                !invLotList.Any(
                                    x =>
                                        x.SkuSysId == info.SkuSysId && x.Lot == info.ToLot &&
                                        x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                            {
                                var newInvLot = new invlot()
                                {
                                    SysId = Guid.NewGuid(),
                                    WareHouseSysId = scanShelvesDto.WarehouseSysId,
                                    Lot = info.ToLot,
                                    SkuSysId = info.SkuSysId,
                                    CaseQty = 0,
                                    InnerPackQty = 0,
                                    Qty = shelvesQty,
                                    AllocatedQty = 0,
                                    PickedQty = 0,
                                    HoldQty = 0,
                                    Status = 1,
                                    Price = info.Price != null ? (decimal)info.Price : 0,
                                    CreateBy = scanShelvesDto.UserId,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = scanShelvesDto.UserId,
                                    UpdateDate = DateTime.Now,
                                    CreateUserName = scanShelvesDto.CurrentDisplayName,
                                    LotAttr01 = info.LotAttr01,
                                    LotAttr02 = info.LotAttr02,
                                    LotAttr03 = info.LotAttr03,
                                    LotAttr04 = info.LotAttr04,
                                    LotAttr05 = info.LotAttr05,
                                    LotAttr06 = info.LotAttr06,
                                    LotAttr07 = info.LotAttr07,
                                    LotAttr08 = info.LotAttr08,
                                    LotAttr09 = info.LotAttr09,
                                    ReceiptDate = info.ReceivedDate,
                                    ProduceDate = info.ProduceDate,
                                    ExpiryDate = info.ExpiryDate,
                                    ExternalLot = info.ExternalLot
                                };
                                invLotList.Add(newInvLot);
                            }
                            else
                            {
                                var oldInvLot =
                                    invLotList.Where(
                                        x =>
                                            x.SkuSysId == info.SkuSysId && x.Lot == info.ToLot &&
                                            x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                                oldInvLot.Qty += shelvesQty;
                            }
                        }

                        #endregion

                        #region InvSkuLoc

                        var invSkuLoc = invSkuLocs.FirstOrDefault(x => x.Loc == scanShelvesDto.Loc && x.SkuSysId == info.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId);
                        if (invSkuLoc != null)
                        {
                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = new Guid(),
                                InvLotSysId = new Guid(),
                                InvSkuLocSysId = invSkuLoc.SysId,
                                Qty = shelvesQty,
                                CurrentUserId = scanShelvesDto.CurrentUserId,
                                CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                                WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            });
                        }
                        else
                        {
                            throw new Exception("收货单商品" + sku.SkuName + "没有历史上架记录,无法进行自动上架.");
                        }

                        #region 必须有货位库存记录才能自动上架，所以移除新增货位库存记录的逻辑
                        //else
                        //{
                        //    if (
                        //        !invSkuLocList.Any(
                        //            x =>
                        //                x.Loc == scanShelvesDto.Loc && x.SkuSysId == info.SkuSysId &&
                        //                x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                        //    {
                        //        var newInvSkuLoc = new invskuloc()
                        //        {
                        //            SysId = Guid.NewGuid(),
                        //            WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        //            SkuSysId = info.SkuSysId,
                        //            Loc = scanShelvesDto.Loc,
                        //            Qty = shelvesQty,
                        //            AllocatedQty = 0,
                        //            PickedQty = 0,
                        //            CreateBy = scanShelvesDto.UserId,
                        //            CreateDate = DateTime.Now,
                        //            UpdateBy = scanShelvesDto.UserId,
                        //            UpdateDate = DateTime.Now,
                        //            CreateUserName = scanShelvesDto.CurrentDisplayName
                        //        };
                        //        invSkuLocList.Add(newInvSkuLoc);
                        //    }
                        //    else
                        //    {
                        //        var oldInvSkuLoc =
                        //            invSkuLocList.Where(
                        //                x =>
                        //                    x.Loc == scanShelvesDto.Loc && x.SkuSysId == info.SkuSysId &&
                        //                    x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                        //        oldInvSkuLoc.Qty += shelvesQty;
                        //    }
                        //}
                        #endregion

                        #endregion

                        #region InvLotLocLpn

                        var invLotLocLpn = invLotLocLpns.FirstOrDefault(x => x.SkuSysId == info.SkuSysId && x.Lot == info.ToLot && x.Loc == scanShelvesDto.Loc && x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId);
                        if (invLotLocLpn != null)
                        {
                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = invLotLocLpn.SysId,
                                InvLotSysId = new Guid(),
                                InvSkuLocSysId = new Guid(),
                                Qty = shelvesQty,
                                CurrentUserId = scanShelvesDto.CurrentUserId,
                                CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                                WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            });
                        }
                        else
                        {
                            if (
                                !invLotLocLpnList.Any(
                                    x =>
                                        x.SkuSysId == info.SkuSysId && x.Lot == info.ToLot && x.Loc == scanShelvesDto.Loc &&
                                        x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId))
                            {
                                var newInvLotLocLpn = new invlotloclpn()
                                {
                                    SysId = Guid.NewGuid(),
                                    WareHouseSysId = scanShelvesDto.WarehouseSysId,
                                    SkuSysId = info.SkuSysId,
                                    Loc = scanShelvesDto.Loc,
                                    Lot = info.ToLot,
                                    Lpn = "",
                                    Qty = shelvesQty,
                                    AllocatedQty = 0,
                                    PickedQty = 0,
                                    Status = 1,
                                    CreateBy = scanShelvesDto.UserId,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = scanShelvesDto.UserId,
                                    UpdateDate = DateTime.Now,
                                    CreateUserName = scanShelvesDto.CurrentDisplayName
                                };
                                invLotLocLpnList.Add(newInvLotLocLpn);
                            }
                            else
                            {
                                var oldInvLotLocLpn =
                                    invLotLocLpnList.Where(
                                        x =>
                                            x.SkuSysId == info.SkuSysId && x.Lot == info.ToLot &&
                                            x.Loc == scanShelvesDto.Loc && x.Lpn == "" &&
                                            x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                                oldInvLotLocLpn.Qty += shelvesQty;
                            }
                        }

                        #endregion


                        var pack = packs.FirstOrDefault(p => p.SysId == info.PackSysId);
                        var uom = uoms.FirstOrDefault(p => p.SysId == info.UOMSysId);

                        #region InvTrans

                        var invTrans = new invtran()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = scanShelvesDto.WarehouseSysId,
                            DocOrder = receipt.ReceiptOrder,
                            DocSysId = receipt.SysId,
                            DocDetailSysId = info.SysId,
                            SkuSysId = info.SkuSysId,
                            SkuCode = sku.SkuCode,
                            TransType = InvTransType.Inbound,
                            SourceTransType = InvSourceTransType.Shelve,
                            Qty = shelvesQty,
                            Loc = scanShelvesDto.Loc,
                            Lot = info.ToLot,
                            Lpn = "",
                            ToLoc = scanShelvesDto.Loc,
                            ToLot = info.ToLot,
                            ToLpn = "",
                            Status = InvTransStatus.Ok,
                            LotAttr01 = info.LotAttr01,
                            LotAttr02 = info.LotAttr02,
                            LotAttr03 = info.LotAttr03,
                            LotAttr04 = info.LotAttr04,
                            LotAttr05 = info.LotAttr05,
                            LotAttr06 = info.LotAttr06,
                            LotAttr07 = info.LotAttr07,
                            LotAttr08 = info.LotAttr08,
                            LotAttr09 = info.LotAttr09,
                            ExternalLot = info.ExternalLot,
                            ProduceDate = info.ProduceDate,
                            ExpiryDate = info.ExpiryDate,
                            ReceivedDate = info.ReceivedDate,
                            PackSysId = (Guid)info.PackSysId,
                            PackCode = pack != null ? pack.PackCode : "",
                            UOMSysId = (Guid)info.UOMSysId,
                            UOMCode = uom != null ? uom.UOMCode : "",
                            CreateBy = scanShelvesDto.UserId,
                            CreateDate = DateTime.Now,
                            UpdateBy = scanShelvesDto.UserId,
                            UpdateDate = DateTime.Now,
                            CreateUserName = scanShelvesDto.CurrentDisplayName
                        };
                        invtranList.Add(invTrans);

                        #endregion
                    }
                    //_crudRepository.BatchInsert(invLotList);
                    //_crudRepository.BatchInsert(invSkuLocList);
                    //_crudRepository.BatchInsert(invLotLocLpnList);
                    _WMSSqlRepository.BatchInsertInvLot(invLotList);
                    _WMSSqlRepository.BatchInsertInvLotLocLpn(invLotLocLpnList);
                    //自动上架更新收货明细
                    _WMSSqlRepository.UpdateReceiptDetailAfterAutoShelves(updateReceiptDetailList);
                    //批量插入invtrans
                    _WMSSqlRepository.BatchInsertInvTrans(invtranList);
                    //执行扣减库存方法(上架)
                    _WMSSqlRepository.UpdateInventoryQtyByShelves(updateInventoryDtos);

                    #region 组织推送上架完成工单数据
                    if (receipt != null)
                    {
                        var mqWorkDto = new MQWorkDto()
                        {
                            WorkBusinessType = (int)WorkBusinessType.Update,
                            WorkType = (int)UserWorkType.Shelve,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            CancelWorkDto = new CancelWorkDto()
                            {
                                DocSysIds = new List<Guid>() { receipt.SysId },
                                Status = (int)WorkStatus.Finish
                            }
                        };

                        var workProcessDto = new MQProcessDto<MQWorkDto>()
                        {
                            BussinessSysId = receipt.SysId,
                            BussinessOrderNumber = receipt.ReceiptOrder,
                            Descr = "",
                            CurrentUserId = scanShelvesDto.CurrentUserId,
                            CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                            WarehouseSysId = scanShelvesDto.WarehouseSysId,
                            BussinessDto = mqWorkDto
                        };
                        //推送工单数据
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                    }
                    #endregion

                    result.IsSucess = true;
                    result.Message = "自动上架完成";
                }
                else
                {
                    throw new Exception("收货单没有收货明细,无法进行自动上架");
                }
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 获取待上架加工单列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        public Pages<RFAssemblyWaitingShelvesListDto> GetAssemblyWaitingShelvesList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            _crudRepository.ChangeDB(assemblyShelvesQuery.WarehouseSysId);
            return _shelvesRepository.GetAssemblyWaitingShelvesList(assemblyShelvesQuery);
        }

        /// <summary>
        /// 获取加工单待上架商品列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        public List<RFAssemblyWaitingShelvesSkuListDto> GetAssemblyWaitingShelvesSkuList(RFAssemblyShelvesQuery assemblyShelvesQuery)
        {
            _crudRepository.ChangeDB(assemblyShelvesQuery.WarehouseSysId);
            return _shelvesRepository.GetAssemblyWaitingShelvesSkuList(assemblyShelvesQuery);
        }

        /// <summary>
        /// 加工单成品上架校验
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        public RFCommResult CheckAssemblyWaitShelvesSku(RFAssemblyScanShelvesDto scanShelvesDto)
        {
            _crudRepository.ChangeDB(scanShelvesDto.WarehouseSysId);
            RFCommResult result = new RFCommResult { IsSucess = true };
            try
            {
                assembly assembly = _crudRepository.GetQuery<assembly>(p => p.AssemblyOrder == scanShelvesDto.AssemblyOrder && p.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (assembly == null)
                {
                    throw new Exception("加工单不存在");
                }
                sku sku = _crudRepository.GetQuery<sku>(p => p.UPC == scanShelvesDto.UPC).FirstOrDefault();
                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }
                if (!(assembly.SkuSysId == sku.SysId
                    && assembly.Status == (int)AssemblyStatus.Finished
                    && (assembly.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || assembly.ShelvesStatus == (int)ShelvesStatus.Shelves)))
                {
                    throw new Exception("加工单中没有待上架的商品");
                }
                if (scanShelvesDto.Qty > (assembly.ActualQty - assembly.ShelvesQty))
                {
                    throw new Exception("上架数量大于待上架商品数量");
                }
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                result.Message = ex.Message;
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 加工单成品扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        public RFCommResult AssemblyScanShelves(RFAssemblyScanShelvesDto scanShelvesDto)
        {
            _crudRepository.ChangeDB(scanShelvesDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = true };
            try
            {
                scanShelvesDto.Loc = scanShelvesDto.Loc.Trim();
                var loc = _crudRepository.GetQuery<location>(x => x.Loc == scanShelvesDto.Loc && x.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (loc == null)
                {
                    throw new Exception("货位不存在");
                }

                if (loc.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"货位{loc.Loc}已被冻结，不能做上架!");
                }

                var assembly = _crudRepository.GetQuery<assembly>(x => x.AssemblyOrder == scanShelvesDto.AssemblyOrder && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (assembly == null)
                {
                    throw new Exception("收货单不存在");
                }

                sku sku = null;
                if (scanShelvesDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == scanShelvesDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.UPC == scanShelvesDto.UPC).FirstOrDefault();
                }
                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                //冻结校验
                var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && p.SkuSysId.Value == sku.SysId
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (frozenSku != null)
                {
                    throw new Exception($"商品已被冻结，不能做上架!");
                }

                var frozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == sku.SysId
                              && p.Loc == scanShelvesDto.Loc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                //校验冻结: 货位商品 
                if (frozenLocSku != null)
                {
                    var skuSysId = frozenLocSku.SkuSysId;
                    var frozenSkuInfo = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSkuInfo.SkuName}'在货位'{scanShelvesDto.Loc}'已被冻结，不能做上架!");
                }

                if (!(assembly.SkuSysId == sku.SysId
                    && assembly.Status == (int)AssemblyStatus.Finished
                    && (assembly.ShelvesStatus == (int)ShelvesStatus.NotOnShelves || assembly.ShelvesStatus == (int)ShelvesStatus.Shelves)))
                {
                    throw new Exception("加工单中没有待上架的商品");
                }

                var totalWaitQty = assembly.ActualQty - assembly.ShelvesQty;

                //gavin: 此处增加单位转换数量，用于更新库存表
                int transQty = 0;
                pack transPack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(sku.SysId, scanShelvesDto.InputQty, out transQty, ref transPack) == true)
                {
                    //gavin: 单位转换更新库存后需要记录
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        DocOrder = assembly.AssemblyOrder,
                        DocSysId = assembly.SysId,
                        DocDetailSysId = Guid.Empty,
                        SkuSysId = sku.SysId,
                        FromQty = scanShelvesDto.InputQty,
                        ToQty = transQty,
                        Loc = scanShelvesDto.Loc,
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = transPack.SysId,
                        PackCode = transPack.PackCode,
                        FromUOMSysId = transPack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = transPack.FieldUom01 ?? Guid.Empty,
                        CreateBy = scanShelvesDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.Assembly,
                        SourceTransType = InvSourceTransType.AssemblyShelve
                    };
                    _crudRepository.Insert(unitTran);

                    scanShelvesDto.Qty = transQty;
                }

                if (scanShelvesDto.Qty > totalWaitQty)
                {
                    throw new Exception("上架数量大于待上架商品数量");
                }

                var invLotList = new List<invlot>();
                var invSkuLocList = new List<invskuloc>();
                var invLotLocLpnList = new List<invlotloclpn>();
                var updateInventoryDtos = new List<UpdateInventoryDto>();

                assembly.ShelvesQty += scanShelvesDto.Qty;
                assembly.ShelvesStatus = totalWaitQty > scanShelvesDto.Qty ? (int)ShelvesStatus.Shelves : (int)ShelvesStatus.Finish;
                _crudRepository.Update(assembly);

                #region InvLot


                var invLots = _crudRepository.GetQuery<invlot>(x => x.Lot == assembly.Lot && x.SkuSysId == assembly.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId);
                var invLot = new invlot();
                if (!string.IsNullOrEmpty(assembly.Channel))
                {
                    invLot = invLots.FirstOrDefault(x => x.LotAttr01 == assembly.Channel);
                }
                else
                {
                    invLot = invLots.FirstOrDefault(x => x.LotAttr01 == null || x.LotAttr01 == "");
                }
                if (invLot != null)
                {
                    updateInventoryDtos.Add(new UpdateInventoryDto()
                    {
                        InvLotLocLpnSysId = new Guid(),
                        InvLotSysId = invLot.SysId,
                        InvSkuLocSysId = new Guid(),
                        Qty = scanShelvesDto.Qty,
                        CurrentUserId = scanShelvesDto.CurrentUserId,
                        CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                        WarehouseSysId = scanShelvesDto.WarehouseSysId,
                    });
                }
                else
                {
                    var newInvLot = new invlot()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        Lot = assembly.Lot,
                        SkuSysId = assembly.SkuSysId,
                        CaseQty = 0,
                        InnerPackQty = 0,
                        Qty = scanShelvesDto.Qty,
                        AllocatedQty = 0,
                        PickedQty = 0,
                        HoldQty = 0,
                        Status = 1,
                        Price = 0,
                        LotAttr01 = assembly.Channel,
                        LotAttr09 = PublicConst.AssemblyLotAttr01,
                        CreateBy = scanShelvesDto.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.UserId,
                        UpdateDate = DateTime.Now,
                        CreateUserName = scanShelvesDto.CurrentDisplayName,
                        ProduceDate = assembly.ActualCompletionDate,
                        ReceiptDate = assembly.ActualCompletionDate
                    };
                    invLotList.Add(newInvLot);
                }
                #endregion

                #region InvSkuLoc
                var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.Loc == scanShelvesDto.Loc && x.SkuSysId == assembly.SkuSysId && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (invSkuLoc != null)
                {
                    updateInventoryDtos.Add(new UpdateInventoryDto()
                    {
                        InvLotLocLpnSysId = new Guid(),
                        InvLotSysId = new Guid(),
                        InvSkuLocSysId = invSkuLoc.SysId,
                        Qty = scanShelvesDto.Qty,
                        CurrentUserId = scanShelvesDto.CurrentUserId,
                        CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                        WarehouseSysId = scanShelvesDto.WarehouseSysId,
                    });
                }
                else
                {
                    var newInvSkuLoc = new invskuloc()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        SkuSysId = assembly.SkuSysId,
                        Loc = scanShelvesDto.Loc,
                        Qty = scanShelvesDto.Qty,
                        AllocatedQty = 0,
                        PickedQty = 0,
                        CreateBy = scanShelvesDto.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.UserId,
                        UpdateDate = DateTime.Now,
                        CreateUserName = scanShelvesDto.CurrentDisplayName
                    };
                    invSkuLocList.Add(newInvSkuLoc);
                }
                #endregion

                #region InvLotLocLpn
                var invLotLocLpn = _crudRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == assembly.SkuSysId && x.Lot == assembly.Lot && x.Loc == scanShelvesDto.Loc && x.Lpn == "" && x.WareHouseSysId == scanShelvesDto.WarehouseSysId).FirstOrDefault();
                if (invLotLocLpn != null)
                {
                    updateInventoryDtos.Add(new UpdateInventoryDto()
                    {
                        InvLotLocLpnSysId = invLotLocLpn.SysId,
                        InvLotSysId = new Guid(),
                        InvSkuLocSysId = new Guid(),
                        Qty = scanShelvesDto.Qty,
                        CurrentUserId = scanShelvesDto.CurrentUserId,
                        CurrentDisplayName = scanShelvesDto.CurrentDisplayName,
                        WarehouseSysId = scanShelvesDto.WarehouseSysId,
                    });
                }
                else
                {
                    var newInvLotLocLpn = new invlotloclpn()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = scanShelvesDto.WarehouseSysId,
                        SkuSysId = assembly.SkuSysId,
                        Loc = scanShelvesDto.Loc,
                        Lot = assembly.Lot,
                        Lpn = "",
                        Qty = scanShelvesDto.Qty,
                        AllocatedQty = 0,
                        PickedQty = 0,
                        Status = 1,
                        CreateBy = scanShelvesDto.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = scanShelvesDto.UserId,
                        UpdateDate = DateTime.Now,
                        CreateUserName = scanShelvesDto.CurrentDisplayName
                    };
                    invLotLocLpnList.Add(newInvLotLocLpn);
                }
                #endregion

                _crudRepository.BatchInsert(invLotList);
                _crudRepository.BatchInsert(invSkuLocList);
                _crudRepository.BatchInsert(invLotLocLpnList);

                pack pack = _crudRepository.GetQuery<pack>(p => p.SysId == sku.PackSysId).FirstOrDefault();
                uom uom = null;
                if (pack != null)
                {
                    uom = _crudRepository.GetQuery<uom>(p => p.SysId == pack.FieldUom01).FirstOrDefault();
                }

                #region InvTrans
                var invTrans = new invtran()
                {
                    SysId = Guid.NewGuid(),
                    WareHouseSysId = scanShelvesDto.WarehouseSysId,
                    DocOrder = assembly.AssemblyOrder,
                    DocSysId = assembly.SysId,
                    DocDetailSysId = Guid.Empty,
                    SkuSysId = assembly.SkuSysId,
                    SkuCode = sku.SkuCode,
                    TransType = InvTransType.Assembly,
                    SourceTransType = InvSourceTransType.AssemblyShelve,
                    Qty = scanShelvesDto.Qty,
                    Loc = scanShelvesDto.Loc,
                    Lot = assembly.Lot,
                    Lpn = "",
                    ToLoc = scanShelvesDto.Loc,
                    ToLot = assembly.Lot,
                    ToLpn = "",
                    Status = InvTransStatus.Ok,
                    LotAttr01 = PublicConst.AssemblyLotAttr01,
                    ProduceDate = assembly.ActualCompletionDate,
                    ReceivedDate = assembly.ActualCompletionDate,
                    PackSysId = pack != null ? pack.SysId : Guid.Empty,
                    PackCode = pack != null ? pack.PackCode : "",
                    UOMSysId = uom != null ? uom.SysId : Guid.Empty,
                    UOMCode = uom != null ? uom.UOMCode : "",
                    CreateBy = scanShelvesDto.UserId,
                    CreateDate = DateTime.Now,
                    UpdateBy = scanShelvesDto.UserId,
                    UpdateDate = DateTime.Now,
                    CreateUserName = scanShelvesDto.CurrentDisplayName
                };
                _crudRepository.Insert(invTrans);

                #endregion

                //执行扣减库存方法(加工单上架)
                _WMSSqlRepository.UpdateInventoryQtyByShelves(updateInventoryDtos);

                result.IsSucess = true;
                result.Message = "上架成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                result.Message = ex.Message;
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 取消上架
        /// </summary>
        /// <param name="cancelShelvesDto"></param>
        /// <returns></returns>
        public CommonResponse CancelShelves(CancelShelvesDto cancelShelvesDto)
        {
            _crudRepository.ChangeDB(cancelShelvesDto.WarehouseSysId);
            var rsp = new CommonResponse(false);
            try
            {
                var receipt = _crudRepository.GetQuery<receipt>(p => p.SysId == cancelShelvesDto.ReceiptSysId).FirstOrDefault();
                if (receipt == null) throw new Exception("收货单不存在");
                //收货单上架明细
                var shelvesDetails = _crudRepository.GetQuery<receiptdetail>(p => p.ReceiptSysId == cancelShelvesDto.ReceiptSysId && (p.ShelvesStatus == (int)ShelvesStatus.Shelves || p.ShelvesStatus == (int)ShelvesStatus.Finish)).ToList();
                if (shelvesDetails.Count == 0) throw new Exception("收货单没有可取消上架的商品");
                //上架交易明细
                var invTrans = _crudRepository.GetQuery<invtran>(p => p.DocSysId == receipt.SysId && p.Status == InvTransStatus.Ok && p.SourceTransType == InvSourceTransType.Shelve && p.WareHouseSysId == cancelShelvesDto.WarehouseSysId).ToList();
                var invTransSysIds = invTrans.Select(p => p.SysId).Distinct();
                //商品明细
                var skuSysIds = shelvesDetails.Select(p => p.SkuSysId).Distinct();
                var skus = _crudRepository.GetQuery<sku>(p => skuSysIds.Contains(p.SysId)).ToList();

                var updateInventoryDtos = new List<UpdateInventoryDto>();
                #region ReceiptDetail
                var updateReceiptDetails = shelvesDetails.Select(p => new UpdateReceiptDetailDto { SysId = p.SysId, ShelvesQty = 0, ShelvesStatus = (int)ShelvesStatus.NotOnShelves, OldTS = p.TS, NewTS = Guid.NewGuid() }).ToList();
                _WMSSqlRepository.UpdateReceiptDetailAfterCancelShelves(updateReceiptDetails, cancelShelvesDto.CurrentUserId, cancelShelvesDto.CurrentDisplayName);

                //foreach (var shelvesDetail in shelvesDetails)
                //{
                //    shelvesDetail.ShelvesQty = 0;
                //    shelvesDetail.ShelvesStatus = (int)ShelvesStatus.NotOnShelves;
                //    shelvesDetail.UpdateBy = cancelShelvesDto.CurrentUserId;
                //    shelvesDetail.UpdateDate = DateTime.Now;
                //    shelvesDetail.UpdateUserName = cancelShelvesDto.CurrentDisplayName;
                //    shelvesDetail.TS = Guid.NewGuid();
                //    _crudRepository.Update(shelvesDetail);
                //}
                #endregion


                //冻结校验

                var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuSysIds.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == cancelShelvesDto.WarehouseSysId).ToList();
                if (frozenSkuList != null && frozenSkuList.Count > 0)
                {
                    var sku = skus.FirstOrDefault(x => x.SysId == frozenSkuList.FirstOrDefault().SkuSysId);
                    throw new Exception($"商品{sku.SkuName}已被冻结，不能做取消上架!");
                }

                var locs = invTrans.Select(p => p.Loc).ToList();
                var locations = _crudRepository.GetQuery<location>(p => locs.Contains(p.Loc) && p.Status == (int)LocationStatus.Frozen && p.WarehouseSysId == receipt.WarehouseSysId);

                if (locations.Count() > 0)
                {
                    throw new Exception($"货位{locations.First().Loc}已被冻结，不能做取消上架!");
                }

                //货位商品级别
                var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuSysIds.Contains(p.SkuSysId.Value)
                    && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == cancelShelvesDto.WarehouseSysId).ToList();

                if (locskuList.Count > 0)
                {
                    var locskuFrozenQuery = from T1 in invTrans
                                            join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                            select T2;

                    if (locskuFrozenQuery.Count() > 0)
                    {
                        var firstFrozenLocsku = locskuFrozenQuery.First();
                        var skuSysId = firstFrozenLocsku.SkuSysId;
                        var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                        throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能做取消上架!");
                    }
                }

                var inventorySkus = new List<GetInventoryDto>();
                foreach (var invTran in invTrans)
                {
                    inventorySkus.Add(new GetInventoryDto { SkuSysId = invTran.SkuSysId, Lot = invTran.Lot, Loc = invTran.Loc, Lpn = "", WareHouseSysId = cancelShelvesDto.WarehouseSysId });
                }
                //批量获取库存信息
                var invLotList = _WMSSqlRepository.BatchGetInvLot(inventorySkus);
                var invSkuLocList = _WMSSqlRepository.BatchGetInvSkuLoc(inventorySkus);
                var invLotLocLpnList = _WMSSqlRepository.BatchGetInvLotLocLpn(inventorySkus);

                #region Inventory
                foreach (var invTran in invTrans)
                {
                    int cancelQty = invTran.Qty;
                    var sku = skus.FirstOrDefault(p => p.SysId == invTran.SkuSysId);

                    #region InvLot
                    var invLot = invLotList.FirstOrDefault(x => x.Lot == invTran.Lot && x.SkuSysId == invTran.SkuSysId && x.WareHouseSysId == cancelShelvesDto.WarehouseSysId);
                    if (invLot != null)
                    {
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = new Guid(),
                            InvLotSysId = invLot.SysId,
                            InvSkuLocSysId = new Guid(),
                            Qty = cancelQty,
                            CurrentUserId = cancelShelvesDto.CurrentUserId,
                            CurrentDisplayName = cancelShelvesDto.CurrentDisplayName,
                            WarehouseSysId = cancelShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        throw new Exception(string.Format("商品 [{0}] 在InvLot没有库存信息", sku.SkuName));
                    }
                    #endregion

                    #region InvSkuLoc
                    var invSkuLoc = invSkuLocList.FirstOrDefault(x => x.Loc == invTran.Loc && x.SkuSysId == invTran.SkuSysId && x.WareHouseSysId == cancelShelvesDto.WarehouseSysId);
                    if (invSkuLoc != null)
                    {
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = new Guid(),
                            InvLotSysId = new Guid(),
                            InvSkuLocSysId = invSkuLoc.SysId,
                            Qty = cancelQty,
                            CurrentUserId = cancelShelvesDto.CurrentUserId,
                            CurrentDisplayName = cancelShelvesDto.CurrentDisplayName,
                            WarehouseSysId = cancelShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        throw new Exception(string.Format("商品 [{0}] 在InvSkuLoc没有库存信息", sku.SkuName));
                    }
                    #endregion

                    #region InvLotLocLpn
                    var invLotLocLpn = invLotLocLpnList.FirstOrDefault(x => x.SkuSysId == invTran.SkuSysId && x.Lot == invTran.Lot && x.Loc == invTran.Loc && x.Lpn == "" && x.WareHouseSysId == cancelShelvesDto.WarehouseSysId);
                    if (invLotLocLpn != null)
                    {
                        updateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = invLotLocLpn.SysId,
                            InvLotSysId = new Guid(),
                            InvSkuLocSysId = new Guid(),
                            Qty = cancelQty,
                            CurrentUserId = cancelShelvesDto.CurrentUserId,
                            CurrentDisplayName = cancelShelvesDto.CurrentDisplayName,
                            WarehouseSysId = cancelShelvesDto.WarehouseSysId,
                        });
                    }
                    else
                    {
                        throw new Exception(string.Format("商品 [{0}] 在InvLotLocLpn没有库存信息", sku.SkuName));
                    }
                    #endregion
                }
                #endregion

                #region InvTrans
                _WMSSqlRepository.UpdateInvTransStatusAfterCancelShelves(invTransSysIds, cancelShelvesDto.CurrentUserId, cancelShelvesDto.CurrentDisplayName);
                //foreach (var invTransSysId in invTransSysIds)
                //{
                //    _crudRepository.Update<invtran>(invTransSysId, p =>
                //    {
                //        p.Status = InvTransStatus.Cancel;
                //        p.UpdateBy = cancelShelvesDto.CurrentUserId;
                //        p.UpdateDate = DateTime.Now;
                //        p.UpdateUserName = cancelShelvesDto.CurrentDisplayName;
                //    });
                //}
                #endregion

                //执行扣减库存方法(取消上架)
                _WMSSqlRepository.UpdateInventoryCancelShelves(updateInventoryDtos);

                #region 组织推送取消拣上架工单数据
                if (receipt != null)
                {
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Update,
                        WorkType = (int)UserWorkType.Shelve,
                        WarehouseSysId = cancelShelvesDto.WarehouseSysId,
                        CurrentUserId = cancelShelvesDto.CurrentUserId,
                        CurrentDisplayName = cancelShelvesDto.CurrentDisplayName,
                        CancelWorkDto = new CancelWorkDto()
                        {
                            DocSysIds = new List<Guid>() { (Guid)receipt.SysId },
                            Status = (int)WorkStatus.Cancel
                        }
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = (Guid)receipt.SysId,
                        BussinessOrderNumber = receipt.ReceiptOrder,
                        Descr = "",
                        CurrentUserId = cancelShelvesDto.CurrentUserId,
                        CurrentDisplayName = cancelShelvesDto.CurrentDisplayName,
                        WarehouseSysId = cancelShelvesDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                }
                #endregion

                rsp.IsSuccess = true;
                rsp.Message = "取消上架完成";
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                throw new Exception(ex.Message);
            }
            return rsp;
        }
    }
}

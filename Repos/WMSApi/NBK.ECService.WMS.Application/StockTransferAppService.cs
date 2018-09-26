using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    public class StockTransferAppService : WMSApplicationService, IStockTransferAppService
    {
        private IStockTransferRepository _crudRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IPackageAppService _packageAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IBaseAppService _baseAppService = null;

        public StockTransferAppService(IStockTransferRepository crudRepository, IInventoryRepository inventoryRepository, IPackageAppService packageAppService, IWMSSqlRepository wmsSqlRepository, IBaseAppService baseAppService)
        {
            this._crudRepository = crudRepository;
            _inventoryRepository = inventoryRepository;
            _packageAppService = packageAppService;
            this._wmsSqlRepository = wmsSqlRepository;
            this._baseAppService = baseAppService;
        }

        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var response = _crudRepository.GetStockTransferLotByPage(request);
            return response;
        }

        public StockTransferDto GetStockTransferBySysId(Guid sysid, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetStockTransferBySysId(sysid);

            return response;
        }

        public void CreateStockTransfer(StockTransferDto st)
        {
            _crudRepository.ChangeDB(st.WarehouseSysId);
            if (st.DisplayToQty <= 0)
            {
                throw new Exception("转移数量必须大于0!");
            }
            //单位转换:用户输入1 kg，DB存储时需要转为 1000 g
            int transQty = 0;
            pack transPack = new pack();
            if (_packageAppService.GetSkuConversiontransQty(st.FromSkuSysId, st.DisplayToQty, out transQty, ref transPack) == true)
            {
                st.ToQty = transQty;
            }
            else
            {
                st.ToQty = Convert.ToInt32(st.DisplayToQty);
            }

            if (st.DisplayToQty > st.CurrentQty)
            {
                throw new Exception("转移数量必须小于等于商品现有数量!");
            }
            st.FromQty = st.ToQty;//转移数量暂时 1:1

            if (st.FromExpiryDate == st.ToExpiryDate
                && st.FromExternalLot == st.ToExternalLot
                && st.FromLoc == st.ToLoc
                && st.FromProduceDate == st.ToProduceDate
                && st.FromLotAttr01 == st.ToLotAttr01
                && st.FromLotAttr02 == st.ToLotAttr02
                && st.FromLotAttr03 == st.ToLotAttr03
                && st.FromLotAttr04 == st.ToLotAttr04
                && st.FromLotAttr05 == st.ToLotAttr05
                && st.FromLotAttr06 == st.ToLotAttr06
                && st.FromLotAttr07 == st.ToLotAttr07
                && st.FromLotAttr08 == st.ToLotAttr08
                && st.FromLotAttr09 == st.ToLotAttr09)
            {
                throw new Exception("目标数据与来源数据完全一致,请检查!");
            }

            //if (!string.IsNullOrEmpty(st.ToLotAttr09)
            //    && st.ToQty != 1)
            //{
            //    throw new Exception("有SN号的商品，转移数量只能为1!");
            //}

            var checkExistsStockTransfer = _crudRepository.GetQuery<stocktransfer>(p => p.Status == (int)StockTransferStatus.New && p.WareHouseSysId == st.WarehouseSysId
               && p.FromSkuSysId == st.FromSkuSysId && p.ToSkuSysId == st.FromSkuSysId  //此时st.toSkuSysId 还未被赋值，因此使用st.FromSkuSysId
               && p.FromLoc == st.FromLoc && p.ToLoc == st.ToLoc && p.FromLot == st.FromLot
               && p.ToExpiryDate == st.ToExpiryDate && p.ToExternalLot == st.ToExternalLot
               && p.ToProduceDate == st.ToProduceDate && p.ToLotAttr01 == st.ToLotAttr01
               && p.ToLotAttr02 == st.ToLotAttr02 && p.ToLotAttr03 == st.ToLotAttr03
               && p.ToLotAttr04 == st.ToLotAttr04 && p.ToLotAttr05 == st.ToLotAttr05
               && p.ToLotAttr06 == st.ToLotAttr06 && p.ToLotAttr07 == st.ToLotAttr07
               && p.ToLotAttr08 == st.ToLotAttr08 && p.ToLotAttr09 == st.ToLotAttr09).FirstOrDefault();

            if (checkExistsStockTransfer != null)
            {
                throw new Exception("该商品已经存在待确认的相同货位批次转移的请求，不能重复请求!");
            }

            var stockTransfer = st.TransformTo<stocktransfer>();

            stockTransfer.SysId = Guid.NewGuid();
            stockTransfer.ToSkuSysId = stockTransfer.FromSkuSysId;
            //stockTransfer.StockTransferOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberStockTransfer);
            stockTransfer.StockTransferOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockTransfer);
            stockTransfer.Status = (int)StockTransferStatus.New;
            stockTransfer.CreateBy = st.CurrentUserId;
            stockTransfer.CreateDate = DateTime.Now;
            stockTransfer.CreateUserName = st.CurrentDisplayName;
            stockTransfer.UpdateBy = st.CurrentUserId;
            stockTransfer.UpdateDate = DateTime.Now;
            stockTransfer.UpdateUserName = st.CurrentDisplayName;

            _crudRepository.Insert(stockTransfer);
        }

        public Pages<StockTransferDto> GetStockTransferOrderByPage(StockTransferQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var response = _crudRepository.GetStockTransferOrderByPage(request);
            return response;
        }

        public void StockTransferOperation(StockTransferDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var stockTransfer = _crudRepository.FirstOrDefault<stocktransfer>(p => p.SysId == request.SysId);
            if (stockTransfer == null)
            {
                throw new Exception("请求的数据不存在!");
            }

            if (stockTransfer.Status != (int)StockTransferStatus.New)
            {
                throw new Exception("当前状态无法做转移操作,请检查!");
            }

            //校验库存冻结
            var fromFrozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && p.SkuSysId.Value == request.FromSkuSysId
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId).FirstOrDefault();
            if (fromFrozenSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == request.FromSkuSysId).FirstOrDefault();
                throw new Exception($"商品{sku.SkuName}已被冻结，不能转移!");
            }

            var fromFrozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == stockTransfer.FromSkuSysId
                                && p.Loc == stockTransfer.FromLoc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == stockTransfer.WareHouseSysId).FirstOrDefault();
            if (fromFrozenLocSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == stockTransfer.FromSkuSysId).FirstOrDefault();
                throw new Exception($"商品'{sku.SkuName}'在货位'{stockTransfer.FromLoc}'已被冻结，不能转移!");
            }

            var fromLocation = _crudRepository.GetQuery<location>(p => p.Loc == stockTransfer.FromLoc && p.WarehouseSysId == stockTransfer.WareHouseSysId).FirstOrDefault();

            if (fromLocation == null)
            {
                throw new Exception($"货位{stockTransfer.FromLoc}已经不存在，请重新创建!");
            }
            if (fromLocation.Status == (int)LocationStatus.Frozen)
            {
                throw new Exception($"货位{fromLocation.Loc}已被冻结，不能转移!");
            }

            var toFrozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == stockTransfer.ToSkuSysId
                                && p.Loc == stockTransfer.ToLoc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == stockTransfer.WareHouseSysId).FirstOrDefault();
            if (toFrozenLocSku != null)
            {
                var sku = _crudRepository.GetQuery<sku>(x => x.SysId == stockTransfer.ToSkuSysId).FirstOrDefault();
                throw new Exception($"商品'{sku.SkuName}'在货位'{stockTransfer.ToLoc}'已被冻结，不能转移!");
            }

            var toLocation = _crudRepository.GetQuery<location>(p => p.Loc == stockTransfer.ToLoc && p.WarehouseSysId == stockTransfer.WareHouseSysId).FirstOrDefault();

            if (toLocation == null)
            {
                throw new Exception($"货位{stockTransfer.ToLoc}已经不存在，请重新创建!");
            }
            if (toLocation.Status == (int)LocationStatus.Frozen)
            {
                throw new Exception($"货位{toLocation.Loc}已被冻结，不能转移!");
            }

            #region 目标批次号处理

            #region 检测是否存在相同批次 
            var checkLots = _crudRepository.GetQuery<invlot>(x =>
              x.SkuSysId == stockTransfer.ToSkuSysId &&
              x.WareHouseSysId == stockTransfer.WareHouseSysId).ToList();

            var checkLot = new List<invlot>();
            if (checkLots != null)
            {
                checkLots.ForEach(x =>
                {
                    if ((string.IsNullOrEmpty(stockTransfer.ToLotAttr01) ? (x.LotAttr01 == stockTransfer.ToLotAttr01 || x.LotAttr01 == null) : x.LotAttr01 == stockTransfer.ToLotAttr01) &&
                        (string.IsNullOrEmpty(stockTransfer.ToLotAttr02) ? (x.LotAttr02 == stockTransfer.ToLotAttr02 || x.LotAttr02 == null) : x.LotAttr02 == stockTransfer.ToLotAttr02) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr03, stockTransfer.ToLotAttr03) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr04, stockTransfer.ToLotAttr04) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr05, stockTransfer.ToLotAttr05) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr06, stockTransfer.ToLotAttr06) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr07, stockTransfer.ToLotAttr07) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr08, stockTransfer.ToLotAttr08) &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.LotAttr09, stockTransfer.ToLotAttr09) &&
                        x.ProduceDate == stockTransfer.ToProduceDate &&
                        CommonBussinessMethod.CompareStringDiffaultIgnoreNull(x.ExternalLot, stockTransfer.ToExternalLot) &&
                        x.ExpiryDate == stockTransfer.ToExpiryDate)
                    {
                        checkLot.Add(x);
                    }
                });
            }

            #endregion
            if (checkLot.Any())
            {
                stockTransfer.ToLot = checkLot.FirstOrDefault().Lot;
            }
            else
            {
                //stockTransfer.ToLot = _crudRepository.GenNextNumber(PublicConst.GenNextNumberLot);
                stockTransfer.ToLot = _baseAppService.GetNumber(PublicConst.GenNextNumberLot);
            }

            #endregion

            stockTransfer.Status = (int)StockTransferStatus.Transfer;
            stockTransfer.UpdateBy = request.CurrentUserId;
            stockTransfer.UpdateDate = DateTime.Now;
            stockTransfer.UpdateUserName = request.CurrentDisplayName;
            stockTransfer.TS = Guid.NewGuid();


            //gavin : 单位转换
            int transFromQty = stockTransfer.FromQty;
            int transToQty = stockTransfer.ToQty;
            pack transFromPack = new pack();
            pack transToPack = new pack();
            //if (_packageAppService.GetSkuConversiontransQty(stockTransfer.FromSkuSysId, stockTransfer.FromQty, out transFromQty, ref transFromPack) == true
            //    && _packageAppService.GetSkuConversiontransQty(stockTransfer.ToSkuSysId, stockTransfer.ToQty, out transToQty, ref transToPack) == true)
            //{
            //    //gavin: 单位转换更新库存后需要记录
            //    unitconversiontran unitTran = new unitconversiontran()
            //    {
            //        WareHouseSysId = stockTransfer.WareHouseSysId,
            //        DocOrder = stockTransfer.StockTransferOrder,
            //        DocSysId = stockTransfer.SysId,
            //        DocDetailSysId = stockTransfer.SysId,
            //        SkuSysId = stockTransfer.FromSkuSysId,
            //        FromQty = stockTransfer.FromQty,
            //        ToQty = transFromQty,
            //        Loc = stockTransfer.FromLoc,
            //        Lot = stockTransfer.FromLot,
            //        Lpn = "",
            //        Status = "Done",
            //        PackSysId = transFromPack.SysId,
            //        PackCode = transFromPack.PackCode,
            //        FromUOMSysId = transFromPack.FieldUom02 ?? Guid.Empty,
            //        ToUOMSysId = transFromPack.FieldUom01 ?? Guid.Empty,
            //        CreateBy = request.CurrentUserId,
            //        CreateDate = DateTime.Now,
            //        UpdateBy = request.CurrentUserId,
            //        UpdateDate = DateTime.Now,
            //        TransType = InvTransType.StockTransfer,
            //        SourceTransType = InvSourceTransType.StockTransfer
            //    };
            //    _crudRepository.Insert(unitTran);

            //    unitconversiontran unitTranTo = new unitconversiontran()
            //    {
            //        WareHouseSysId = stockTransfer.WareHouseSysId,
            //        DocOrder = stockTransfer.StockTransferOrder,
            //        DocSysId = stockTransfer.SysId,
            //        DocDetailSysId = stockTransfer.SysId,
            //        SkuSysId = stockTransfer.ToSkuSysId,
            //        FromQty = stockTransfer.ToQty,
            //        ToQty = transToQty,
            //        Loc = stockTransfer.ToLoc,
            //        Lot = stockTransfer.ToLot,
            //        Lpn = "",
            //        Status = "Done",
            //        PackSysId = transToPack.SysId,
            //        PackCode = transToPack.PackCode,
            //        FromUOMSysId = transToPack.FieldUom02 ?? Guid.Empty,
            //        ToUOMSysId = transToPack.FieldUom01 ?? Guid.Empty,
            //        CreateBy = request.CurrentUserId,
            //        CreateDate = DateTime.Now,
            //        UpdateBy = request.CurrentUserId,
            //        UpdateDate = DateTime.Now,
            //        TransType = InvTransType.StockTransfer,
            //        SourceTransType = InvSourceTransType.StockTransfer
            //    };
            //    _crudRepository.Insert(unitTranTo);
            //}

            var fromUpdateInventoryDtos = new List<UpdateInventoryDto>();
            var toUpdateInventoryDtos = new List<UpdateInventoryDto>();

            //----------------------以下处理转移前库存-------------------------
            var fromInvDto = _inventoryRepository.GetlotloclpnDto(stockTransfer.FromSkuSysId, stockTransfer.WareHouseSysId, stockTransfer.FromLot, stockTransfer.FromLoc, stockTransfer.FromLpn);
            if (fromInvDto == null)
            {
                throw new Exception("有商品库存信息不存在(fromInvDto)，请检查！");
            }
            var fromSkuInvlotloclpn = _crudRepository.Get<invlotloclpn>(fromInvDto.InvLotLocLpnSysId);

            if (fromSkuInvlotloclpn == null)
            {
                throw new Exception("有商品库存信息不存在(fromSkuInvlotloclpn)，请检查！");
            }

            if ((CommonBussinessMethod.GetAvailableQty(fromSkuInvlotloclpn.Qty, fromSkuInvlotloclpn.AllocatedQty, fromSkuInvlotloclpn.PickedQty, fromSkuInvlotloclpn.FrozenQty)) < transFromQty)
            {
                throw new Exception(" 库存不足，请检查！");
            }


            var fromSkuInvlot = _crudRepository.Get<invlot>(fromInvDto.InvLotSysId);

            if (fromSkuInvlot == null)
            {
                throw new Exception("有商品库存信息不存在(invlot)，请检查！");
            }
            if ((CommonBussinessMethod.GetAvailableQty(fromSkuInvlot.Qty, fromSkuInvlot.AllocatedQty, fromSkuInvlot.PickedQty, fromSkuInvlot.FrozenQty)) < transFromQty)
            {
                throw new Exception("库存不足，请检查！");
            }


            var fromSkuinvskuloc = _crudRepository.Get<invskuloc>(fromInvDto.InvSkuLocSysId);

            if (fromSkuinvskuloc == null)
            {
                throw new Exception("有商品库存信息不存在(invskuloc)，请检查！");
            }
            if ((CommonBussinessMethod.GetAvailableQty(fromSkuinvskuloc.Qty, fromSkuinvskuloc.AllocatedQty, fromSkuinvskuloc.PickedQty, fromSkuinvskuloc.FrozenQty)) < transFromQty)
            {
                throw new Exception("库存不足，请检查！");
            }

            fromUpdateInventoryDtos.Add(new UpdateInventoryDto()
            {
                InvLotLocLpnSysId = fromSkuInvlotloclpn.SysId,
                InvLotSysId = fromSkuInvlot.SysId,
                InvSkuLocSysId = fromSkuinvskuloc.SysId,
                Qty = transFromQty,
                CurrentUserId = request.CurrentUserId,
                CurrentDisplayName = request.CurrentDisplayName,
                WarehouseSysId = stockTransfer.WareHouseSysId,
            });

            sku fromSkuInfo = _crudRepository.FirstOrDefault<sku>(p => p.SysId == stockTransfer.FromSkuSysId);

            invtran fromInvtranInfo = new invtran()
            {
                SysId = Guid.NewGuid(),
                DocOrder = stockTransfer.StockTransferOrder,
                DocSysId = stockTransfer.SysId,
                WareHouseSysId = stockTransfer.WareHouseSysId,
                SkuSysId = stockTransfer.FromSkuSysId,
                SkuCode = fromSkuInfo.SkuCode,
                TransType = InvTransType.StockTransfer,
                SourceTransType = InvSourceTransType.StockTransfer,
                Qty = -transFromQty,
                Loc = stockTransfer.FromLoc,
                Lot = stockTransfer.FromLot,
                Lpn = stockTransfer.FromLpn,
                Status = InvTransStatus.Ok,
                CreateBy = request.CurrentUserId,
                CreateUserName = request.CurrentDisplayName,
                CreateDate = DateTime.Now,
                UpdateBy = request.CurrentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = request.CurrentDisplayName
            };

            _crudRepository.Insert(fromInvtranInfo);


            //----------------------以下处理转移后库存-------------------------
            var toSkuInvlotloclpn = new invlotloclpn() { SysId = new Guid() };
            var toSkuInvlot = new invlot() { SysId = new Guid() };
            var toSkuinvskuloc = new invskuloc() { SysId = new Guid() };
            var invlotloclpnSysId = _inventoryRepository.Getinvlotloclpn(stockTransfer.ToSkuSysId, stockTransfer.WareHouseSysId, stockTransfer.ToLot, stockTransfer.ToLoc, stockTransfer.ToLpn);
            if (invlotloclpnSysId != Guid.Empty)
            {
                toUpdateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = invlotloclpnSysId,
                    InvLotSysId = new Guid(),
                    InvSkuLocSysId = new Guid(),
                    Qty = transToQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = stockTransfer.WareHouseSysId,
                });
            }
            else
            {
                toSkuInvlotloclpn = new invlotloclpn()
                {
                    SysId = Guid.NewGuid(),
                    WareHouseSysId = stockTransfer.WareHouseSysId,
                    SkuSysId = stockTransfer.ToSkuSysId,
                    Loc = stockTransfer.ToLoc,
                    Lot = stockTransfer.ToLot,
                    Lpn = stockTransfer.ToLpn ?? string.Empty,
                    Qty = transToQty,
                    Status = 1,
                    CreateBy = request.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = request.CurrentDisplayName,
                    UpdateBy = request.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = request.CurrentDisplayName
                };
            }

            var invlotSysId = _inventoryRepository.Getinvlot(stockTransfer.ToSkuSysId, stockTransfer.WareHouseSysId, stockTransfer.ToLot);
            if (invlotSysId != Guid.Empty)
            {
                toUpdateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = new Guid(),
                    InvLotSysId = invlotSysId,
                    InvSkuLocSysId = new Guid(),
                    Qty = transToQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = stockTransfer.WareHouseSysId,
                });
            }
            else
            {
                toSkuInvlot = new invlot()
                {
                    SysId = Guid.NewGuid(),
                    WareHouseSysId = stockTransfer.WareHouseSysId,
                    Lot = stockTransfer.ToLot,
                    SkuSysId = stockTransfer.ToSkuSysId,
                    Qty = transToQty,
                    Status = 1,
                    ProduceDate = stockTransfer.ToProduceDate,
                    ExpiryDate = stockTransfer.ToExpiryDate,
                    ExternalLot = stockTransfer.ToExternalLot,
                    ReceiptDate = fromSkuInvlot.ReceiptDate,
                    LotAttr01 = stockTransfer.ToLotAttr01,
                    LotAttr02 = stockTransfer.ToLotAttr02,
                    LotAttr03 = stockTransfer.ToLotAttr03,
                    LotAttr04 = stockTransfer.ToLotAttr04,
                    LotAttr05 = stockTransfer.ToLotAttr05,
                    LotAttr06 = stockTransfer.ToLotAttr06,
                    LotAttr07 = stockTransfer.ToLotAttr07,
                    LotAttr08 = stockTransfer.ToLotAttr08,
                    LotAttr09 = stockTransfer.ToLotAttr09,
                    CreateBy = request.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = request.CurrentDisplayName,
                    UpdateBy = request.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = request.CurrentDisplayName
                };
            }

            var invskulocSysId = _inventoryRepository.Getinvskuloc(stockTransfer.ToSkuSysId, stockTransfer.WareHouseSysId, stockTransfer.ToLoc);
            if (invskulocSysId != Guid.Empty)
            {
                toUpdateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = new Guid(),
                    InvLotSysId = new Guid(),
                    InvSkuLocSysId = invskulocSysId,
                    Qty = transToQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = stockTransfer.WareHouseSysId,
                });
            }
            else
            {
                toSkuinvskuloc = new invskuloc()
                {
                    SysId = Guid.NewGuid(),
                    WareHouseSysId = stockTransfer.WareHouseSysId,
                    SkuSysId = stockTransfer.ToSkuSysId,
                    Loc = stockTransfer.ToLoc,
                    Qty = transToQty,
                    CreateBy = request.CurrentUserId,
                    CreateDate = DateTime.Now,
                    CreateUserName = request.CurrentDisplayName,
                    UpdateBy = request.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    UpdateUserName = request.CurrentDisplayName
                };
            }

            sku toSkuInfo = _crudRepository.FirstOrDefault<sku>(p => p.SysId == stockTransfer.ToSkuSysId);

            invtran toInvtranInfo = new invtran()
            {
                SysId = Guid.NewGuid(),
                DocOrder = stockTransfer.StockTransferOrder,
                DocSysId = stockTransfer.SysId,
                WareHouseSysId = stockTransfer.WareHouseSysId,
                SkuSysId = stockTransfer.ToSkuSysId,
                SkuCode = toSkuInfo.SkuCode,
                TransType = InvTransType.StockTransfer,
                SourceTransType = InvSourceTransType.StockTransfer,
                Qty = transToQty,
                Loc = stockTransfer.ToLoc,
                Lot = stockTransfer.ToLot,
                Lpn = stockTransfer.ToLpn,
                Status = InvTransStatus.Ok,
                CreateBy = request.CurrentUserId,
                CreateUserName = request.CurrentDisplayName,
                CreateDate = DateTime.Now,
                UpdateBy = request.CurrentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = request.CurrentDisplayName
            };

            _crudRepository.Insert(toInvtranInfo);

            _crudRepository.Update(stockTransfer);

            //调用sql方法修改库存
            _wmsSqlRepository.UpdateInventoryQtyByFromStockTransfer(fromUpdateInventoryDtos);
            _wmsSqlRepository.UpdateInventoryQtyByToStockTransfer(toUpdateInventoryDtos);
            _wmsSqlRepository.AddInventoryQtyByToStockTransfer(toSkuInvlotloclpn, toSkuInvlot, toSkuinvskuloc);
        }

        public void StockTransferCancel(StockTransferDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var stockTransfer = _crudRepository.FirstOrDefault<stocktransfer>(p => p.SysId == request.SysId);
            if (stockTransfer == null)
            {
                throw new Exception("请求的数据不存在!");
            }

            if (stockTransfer.Status != (int)StockTransferStatus.New)
            {
                throw new Exception("当前状态无法做作废操作,请检查!");
            }

            stockTransfer.Status = (int)StockTransferStatus.Cancel;
            stockTransfer.UpdateBy = request.CurrentUserId;
            stockTransfer.UpdateDate = DateTime.Now;
            stockTransfer.UpdateUserName = request.CurrentDisplayName;

            _crudRepository.Update(stockTransfer);
        }

        public StockTransferDto GetStockTransferOrderBySysId(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetStockTransferOrderBySysId(sysId);
        }

        /// <summary>
        /// 根据批次属性库存转移
        /// </summary>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        public bool StockTransferByLotAttr(StockTransferDto stockTransferDto)
        {
            //_crudRepository.ChangeDB(stockTransferDto.WarehouseSysId);
            var result = false;
            try
            {
                var invLotLocLpnList = _inventoryRepository.GetlotloclpnDtoList(stockTransferDto.FromSkuSysId, stockTransferDto.WarehouseSysId, stockTransferDto);
                if (invLotLocLpnList != null && invLotLocLpnList.Count > 0)
                {
                    var remainQty = stockTransferDto.FromQty;
                    var invQty = 0;
                    var fromUpdateInventoryDtos = new List<UpdateInventoryDto>();
                    var toUpdateInventoryDtos = new List<UpdateInventoryDto>();
                    var toInvLotList = new List<invlot>();
                    var toInvLotLocLpnList = new List<invlotloclpn>();

                    foreach (var info in invLotLocLpnList)
                    {
                        if (remainQty <= 0)
                        {
                            break;
                        }

                        var fromInvlotloclpn = _crudRepository.Get<invlotloclpn>(info.InvLotLocLpnSysId);
                        if (fromInvlotloclpn == null)
                        {
                            throw new Exception("商品库存信息不存在(invlotloclpn)，请检查！");
                        }

                        var fromInvLotLocLpnQty = CommonBussinessMethod.GetAvailableQty(fromInvlotloclpn.Qty, fromInvlotloclpn.AllocatedQty, fromInvlotloclpn.PickedQty, fromInvlotloclpn.FrozenQty);
                        //如果库存为0，则进行下一条库存
                        if (fromInvLotLocLpnQty <= 0)
                        {
                            continue;
                        }

                        if (remainQty <= fromInvLotLocLpnQty)
                        {
                            invQty = remainQty;
                            remainQty = 0;
                        }
                        else
                        {
                            invQty = fromInvLotLocLpnQty;
                            remainQty = remainQty - fromInvLotLocLpnQty;
                        }

                        var fromInvlot = _crudRepository.Get<invlot>(info.InvLotSysId);
                        if (fromInvlot == null)
                        {
                            throw new Exception("商品库存信息不存在(invlot)，请检查！");
                        }

                        fromUpdateInventoryDtos.Add(new UpdateInventoryDto()
                        {
                            InvLotLocLpnSysId = info.InvLotLocLpnSysId,
                            InvLotSysId = info.InvLotSysId,
                            InvSkuLocSysId = info.InvSkuLocSysId,
                            Qty = invQty,
                            CurrentUserId = stockTransferDto.CurrentUserId,
                            CurrentDisplayName = stockTransferDto.CurrentDisplayName
                        });

                        #region 检测目标批次是否存在相同批次 

                        var checkLot = toInvLotList.Where(x =>
                          x.SkuSysId == stockTransferDto.ToSkuSysId &&
                          x.WareHouseSysId == stockTransferDto.WarehouseSysId &&
                          (string.IsNullOrEmpty(stockTransferDto.ToLotAttr01) ? (x.LotAttr01 == stockTransferDto.ToLotAttr01 || x.LotAttr01 == null) : x.LotAttr01 == stockTransferDto.ToLotAttr01) &&
                          (string.IsNullOrEmpty(stockTransferDto.ToLotAttr02) ? (x.LotAttr02 == stockTransferDto.ToLotAttr02 || x.LotAttr02 == null) : x.LotAttr02 == stockTransferDto.ToLotAttr02) &&
                          x.LotAttr03 == fromInvlot.LotAttr03 &&
                          x.LotAttr04 == fromInvlot.LotAttr04 &&
                          x.LotAttr05 == fromInvlot.LotAttr05 &&
                          x.LotAttr06 == fromInvlot.LotAttr06 &&
                          x.LotAttr07 == fromInvlot.LotAttr07 &&
                          x.LotAttr08 == fromInvlot.LotAttr08 &&
                          x.LotAttr09 == fromInvlot.LotAttr09 &&
                          x.ProduceDate == fromInvlot.ProduceDate &&
                          x.ExternalLot == fromInvlot.ExternalLot &&
                          x.ExpiryDate == fromInvlot.ExpiryDate).FirstOrDefault();

                        if (checkLot == null)
                        {
                            checkLot = _crudRepository.GetQuery<invlot>(x =>
                              x.SkuSysId == stockTransferDto.ToSkuSysId &&
                              x.WareHouseSysId == stockTransferDto.WarehouseSysId &&
                              (string.IsNullOrEmpty(stockTransferDto.ToLotAttr01) ? (x.LotAttr01 == stockTransferDto.ToLotAttr01 || x.LotAttr01 == null) : x.LotAttr01 == stockTransferDto.ToLotAttr01) &&
                              (string.IsNullOrEmpty(stockTransferDto.ToLotAttr02) ? (x.LotAttr02 == stockTransferDto.ToLotAttr02 || x.LotAttr02 == null) : x.LotAttr02 == stockTransferDto.ToLotAttr02) &&
                              x.LotAttr03 == fromInvlot.LotAttr03 &&
                              x.LotAttr04 == fromInvlot.LotAttr04 &&
                              x.LotAttr05 == fromInvlot.LotAttr05 &&
                              x.LotAttr06 == fromInvlot.LotAttr06 &&
                              x.LotAttr07 == fromInvlot.LotAttr07 &&
                              x.LotAttr08 == fromInvlot.LotAttr08 &&
                              x.LotAttr09 == fromInvlot.LotAttr09 &&
                              x.ProduceDate == fromInvlot.ProduceDate &&
                              x.ExternalLot == fromInvlot.ExternalLot &&
                              x.ExpiryDate == fromInvlot.ExpiryDate).FirstOrDefault();
                        }

                        if (checkLot != null)
                        {
                            stockTransferDto.ToLot = checkLot.Lot;
                        }
                        else
                        {
                            //stockTransferDto.ToLot = _crudRepository.GenNextNumber(PublicConst.GenNextNumberLot);
                            stockTransferDto.ToLot = _baseAppService.GetNumber(PublicConst.GenNextNumberLot);
                        }
                        #endregion

                        #region toInvLot
                        var toInvLot = _crudRepository.GetQuery<invlot>(x => x.Lot == stockTransferDto.ToLot && x.SkuSysId == stockTransferDto.ToSkuSysId && x.WareHouseSysId == stockTransferDto.WarehouseSysId).FirstOrDefault();
                        if (toInvLot != null)
                        {
                            toUpdateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = new Guid(),
                                InvLotSysId = toInvLot.SysId,
                                InvSkuLocSysId = new Guid(),
                                Qty = invQty,
                                CurrentUserId = stockTransferDto.CurrentUserId,
                                CurrentDisplayName = stockTransferDto.CurrentDisplayName,
                                WarehouseSysId = stockTransferDto.WarehouseSysId,
                            });
                        }
                        else
                        {
                            if (!toInvLotList.Any(x => x.Lot == stockTransferDto.ToLot && x.SkuSysId == stockTransferDto.ToSkuSysId && x.WareHouseSysId == stockTransferDto.WarehouseSysId))
                            {
                                var newInvLot = new invlot()
                                {
                                    SysId = Guid.NewGuid(),
                                    WareHouseSysId = stockTransferDto.WarehouseSysId,
                                    Lot = stockTransferDto.ToLot,
                                    SkuSysId = stockTransferDto.ToSkuSysId,
                                    CaseQty = 0,
                                    InnerPackQty = 0,
                                    Qty = invQty,
                                    AllocatedQty = 0,
                                    PickedQty = 0,
                                    HoldQty = 0,
                                    Status = 1,
                                    Price = 0,
                                    CreateBy = stockTransferDto.CurrentUserId,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = stockTransferDto.CurrentUserId,
                                    UpdateDate = DateTime.Now,
                                    CreateUserName = stockTransferDto.CurrentDisplayName,
                                    LotAttr01 = stockTransferDto.ToLotAttr01,
                                    LotAttr02 = stockTransferDto.ToLotAttr02,
                                    LotAttr03 = fromInvlot.LotAttr03,
                                    LotAttr04 = fromInvlot.LotAttr04,
                                    LotAttr05 = fromInvlot.LotAttr05,
                                    LotAttr06 = fromInvlot.LotAttr06,
                                    LotAttr07 = fromInvlot.LotAttr07,
                                    LotAttr08 = fromInvlot.LotAttr08,   //因为现在有在用，所以赋值回去
                                    LotAttr09 = fromInvlot.LotAttr09,   //因为现在有在用，所以赋值回去
                                    ReceiptDate = fromInvlot.ReceiptDate,   //因为现在有在用，所以赋值回去
                                    ProduceDate = fromInvlot.ProduceDate,   //因为现在有在用，所以赋值回去
                                    ExpiryDate = fromInvlot.ExpiryDate,   //因为现在有在用，所以赋值回去
                                    ExternalLot = fromInvlot.ExternalLot   //因为现在有在用，所以赋值回去
                                };
                                toInvLotList.Add(newInvLot);
                            }
                            else
                            {
                                var oldInvLot = toInvLotList.Where(x => x.SkuSysId == stockTransferDto.ToSkuSysId && x.Lot == stockTransferDto.ToLot && x.WareHouseSysId == stockTransferDto.WarehouseSysId).FirstOrDefault();
                                oldInvLot.Qty += invQty;
                            }
                        }
                        #endregion

                        #region toInvLotLocLpn
                        var toInvLotLocLpn = _crudRepository.GetQuery<invlotloclpn>(x => x.SkuSysId == stockTransferDto.ToSkuSysId && x.Lot == stockTransferDto.ToLot && x.Loc == fromInvlotloclpn.Loc && x.Lpn == fromInvlotloclpn.Lpn && x.WareHouseSysId == stockTransferDto.WarehouseSysId).FirstOrDefault();
                        if (toInvLotLocLpn != null)
                        {
                            toUpdateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = toInvLotLocLpn.SysId,
                                InvLotSysId = new Guid(),
                                InvSkuLocSysId = new Guid(),
                                Qty = invQty,
                                CurrentUserId = stockTransferDto.CurrentUserId,
                                CurrentDisplayName = stockTransferDto.CurrentDisplayName,
                                WarehouseSysId = stockTransferDto.WarehouseSysId,
                            });
                        }
                        else
                        {
                            if (!toInvLotLocLpnList.Any(x => x.SkuSysId == stockTransferDto.ToSkuSysId && x.Lot == stockTransferDto.ToLot && x.Loc == fromInvlotloclpn.Loc && x.Lpn == fromInvlotloclpn.Lpn && x.WareHouseSysId == stockTransferDto.WarehouseSysId))
                            {
                                var newInvLotLocLpn = new invlotloclpn()
                                {
                                    SysId = Guid.NewGuid(),
                                    WareHouseSysId = stockTransferDto.WarehouseSysId,
                                    SkuSysId = stockTransferDto.ToSkuSysId,
                                    Loc = fromInvlotloclpn.Loc,
                                    Lot = stockTransferDto.ToLot,
                                    Lpn = fromInvlotloclpn.Lpn,
                                    Qty = invQty,
                                    AllocatedQty = 0,
                                    PickedQty = 0,
                                    Status = 1,
                                    CreateBy = stockTransferDto.CurrentUserId,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = stockTransferDto.CurrentUserId,
                                    UpdateDate = DateTime.Now,
                                    CreateUserName = stockTransferDto.CurrentDisplayName
                                };
                                toInvLotLocLpnList.Add(newInvLotLocLpn);
                            }
                            else
                            {
                                var oldInvLotLocLpn = toInvLotLocLpnList.Where(x => x.SkuSysId == stockTransferDto.ToSkuSysId && x.Lot == stockTransferDto.ToLot && x.Loc == fromInvlotloclpn.Loc && x.Lpn == fromInvlotloclpn.Lpn && x.WareHouseSysId == stockTransferDto.WarehouseSysId).FirstOrDefault();
                                oldInvLotLocLpn.Qty += invQty;
                            }
                        }

                        #endregion

                        #region 库存转移记录 StockTransfer
                        var stf = new stocktransfer()
                        {
                            SysId = Guid.NewGuid(),
                            //StockTransferOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberStockTransfer),
                            StockTransferOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberStockTransfer),
                            WareHouseSysId = stockTransferDto.WarehouseSysId,
                            Status = (int)StockTransferStatus.Transfer,
                            Descr = "ECC调用生成",
                            FromSkuSysId = stockTransferDto.FromSkuSysId,
                            ToSkuSysId = stockTransferDto.ToSkuSysId,
                            FromLot = fromInvlotloclpn.Lot,
                            ToLot = stockTransferDto.ToLot,
                            FromLoc = fromInvlotloclpn.Loc,
                            ToLoc = fromInvlotloclpn.Loc,
                            FromQty = invQty,
                            ToQty = invQty,
                            FromLpn = fromInvlotloclpn.Lpn,
                            ToLpn = fromInvlotloclpn.Lpn,
                            FromLotAttr01 = stockTransferDto.FromLotAttr01,
                            FromLotAttr02 = stockTransferDto.FromLotAttr02,
                            FromLotAttr03 = stockTransferDto.FromLotAttr03,
                            FromLotAttr04 = stockTransferDto.FromLotAttr04,
                            FromLotAttr05 = stockTransferDto.FromLotAttr05,
                            FromLotAttr06 = stockTransferDto.FromLotAttr06,
                            FromLotAttr07 = stockTransferDto.FromLotAttr07,
                            FromLotAttr08 = fromInvlot.LotAttr08,   //因为现在有在用，所以赋值回去
                            FromLotAttr09 = fromInvlot.LotAttr09,   //因为现在有在用，所以赋值回去
                            FromProduceDate = fromInvlot.ProduceDate,  //因为现在有在用，所以赋值回去
                            FromExternalLot = fromInvlot.ExternalLot,   //因为现在有在用，所以赋值回去
                            FromExpiryDate = fromInvlot.ExpiryDate,  //因为现在有在用，所以赋值回去
                            ToLotAttr01 = stockTransferDto.ToLotAttr01,
                            ToLotAttr02 = stockTransferDto.ToLotAttr02,
                            ToLotAttr03 = stockTransferDto.ToLotAttr03,
                            ToLotAttr04 = stockTransferDto.ToLotAttr04,
                            ToLotAttr05 = stockTransferDto.ToLotAttr05,
                            ToLotAttr06 = stockTransferDto.ToLotAttr06,
                            ToLotAttr07 = stockTransferDto.ToLotAttr07,
                            ToLotAttr08 = fromInvlot.LotAttr08,  //因为现在有在用，所以赋值回去
                            ToLotAttr09 = fromInvlot.LotAttr09,  //因为现在有在用，所以赋值回去
                            ToProduceDate = fromInvlot.ProduceDate,  //因为现在有在用，所以赋值回去
                            ToExternalLot = fromInvlot.ExternalLot,  //因为现在有在用，所以赋值回去
                            ToExpiryDate = fromInvlot.ExpiryDate,  //因为现在有在用，所以赋值回去
                            CreateBy = stockTransferDto.CurrentUserId,
                            CreateDate = DateTime.Now,
                            CreateUserName = stockTransferDto.CurrentDisplayName,
                            UpdateBy = stockTransferDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = stockTransferDto.CurrentDisplayName
                        };

                        _crudRepository.Insert(stf);
                        #endregion

                        #region 来源交易
                        invtran fromInvtranInfo = new invtran()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = stockTransferDto.WarehouseSysId,
                            DocOrder = stf.StockTransferOrder,
                            DocSysId = stf.SysId,
                            DocDetailSysId = stf.SysId,
                            SkuSysId = stockTransferDto.FromSkuSysId,
                            SkuCode = stockTransferDto.SkuCode,
                            TransType = InvTransType.StockTransfer,
                            SourceTransType = InvSourceTransType.StockTransfer,
                            Qty = -invQty,
                            Loc = fromInvlotloclpn.Loc,
                            Lot = fromInvlotloclpn.Lot,
                            Lpn = fromInvlotloclpn.Lpn,
                            LotAttr01 = stockTransferDto.FromLotAttr01,
                            LotAttr02 = stockTransferDto.FromLotAttr02,
                            LotAttr03 = stockTransferDto.FromLotAttr03,
                            LotAttr04 = stockTransferDto.FromLotAttr04,
                            LotAttr05 = stockTransferDto.FromLotAttr05,
                            LotAttr06 = stockTransferDto.FromLotAttr06,
                            LotAttr07 = stockTransferDto.FromLotAttr07,
                            LotAttr08 = stockTransferDto.FromLotAttr08,
                            LotAttr09 = stockTransferDto.FromLotAttr09,
                            ExternalLot = stockTransferDto.FromExternalLot,
                            ProduceDate = stockTransferDto.FromProduceDate,
                            ExpiryDate = stockTransferDto.FromExpiryDate,
                            Status = InvTransStatus.Ok,
                            CreateBy = stockTransferDto.CurrentUserId,
                            CreateUserName = stockTransferDto.CurrentDisplayName,
                            CreateDate = DateTime.Now,
                            UpdateBy = stockTransferDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = stockTransferDto.CurrentDisplayName
                        };

                        _crudRepository.Insert(fromInvtranInfo);
                        #endregion

                        #region 目标交易
                        invtran toInvtranInfo = new invtran()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = stockTransferDto.WarehouseSysId,
                            DocOrder = stf.StockTransferOrder,
                            DocSysId = stf.SysId,
                            DocDetailSysId = stf.SysId,
                            SkuSysId = stockTransferDto.ToSkuSysId,
                            SkuCode = stockTransferDto.SkuCode,
                            TransType = InvTransType.StockTransfer,
                            SourceTransType = InvSourceTransType.StockTransfer,
                            Qty = invQty,
                            Loc = fromInvlotloclpn.Loc,
                            Lot = stockTransferDto.ToLot,
                            Lpn = fromInvlotloclpn.Lpn,
                            LotAttr01 = stockTransferDto.ToLotAttr01,
                            LotAttr02 = stockTransferDto.ToLotAttr02,
                            LotAttr03 = stockTransferDto.ToLotAttr03,
                            LotAttr04 = stockTransferDto.ToLotAttr04,
                            LotAttr05 = stockTransferDto.ToLotAttr05,
                            LotAttr06 = stockTransferDto.ToLotAttr06,
                            LotAttr07 = stockTransferDto.ToLotAttr07,
                            LotAttr08 = stockTransferDto.ToLotAttr08,
                            LotAttr09 = stockTransferDto.ToLotAttr09,
                            ExternalLot = stockTransferDto.ToExternalLot,
                            ProduceDate = stockTransferDto.ToProduceDate,
                            ExpiryDate = stockTransferDto.ToExpiryDate,
                            Status = InvTransStatus.Ok,
                            CreateBy = stockTransferDto.CurrentUserId,
                            CreateUserName = stockTransferDto.CurrentDisplayName,
                            CreateDate = DateTime.Now,
                            UpdateBy = stockTransferDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = stockTransferDto.CurrentDisplayName
                        };

                        _crudRepository.Insert(toInvtranInfo);
                        #endregion
                    }

                    if (remainQty > 0)
                    {
                        throw new Exception("库存不足,无法进行转移！");
                    }

                    //插入库存
                    _crudRepository.BatchInsert(toInvLotList);
                    _crudRepository.BatchInsert(toInvLotLocLpnList);

                    //调用sql方法修改库存
                    _wmsSqlRepository.UpdateInventoryQtyByStockTransfer(fromUpdateInventoryDtos);
                    _wmsSqlRepository.UpdateInventoryAddQtyByStockTransfer(toUpdateInventoryDtos);
                }
                else
                {
                    throw new Exception("未找到对应库存数据");
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}

using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application
{
    /// <summary>
    /// RF拣货
    /// </summary>
    public class RFPickDetailAppService : WMSApplicationService, IRFPickDetailAppService
    {
        private ICrudRepository _crudRepository = null;
        private IRFPickDetailRepository _rfPickDetailRepository = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IPackageAppService _packageAppService = null;
        private IBaseAppService _baseAppService = null;

        public RFPickDetailAppService(ICrudRepository crudRepository, IRFPickDetailRepository rfPickDetailRepository, IWMSSqlRepository wmsSqlRepository, IPackageAppService packageAppService, IBaseAppService baseAppService)
        {
            this._crudRepository = crudRepository;
            _rfPickDetailRepository = rfPickDetailRepository;
            this._WMSSqlRepository = wmsSqlRepository;
            _packageAppService = packageAppService;
            this._baseAppService = baseAppService;
        }

        /// <summary>
        /// 获取待拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingPickListDto> GetWaitingPickOutboundList(RFPickQuery pickQuery)
        {
            _rfPickDetailRepository.ChangeDB(pickQuery.WarehouseSysId);
            return _rfPickDetailRepository.GetWaitingPickOutboundList(pickQuery);
        }

        /// <summary>
        /// 获取待容器拣货的出库单
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public Pages<RFContainerPickingListDto> GetWaitingContainerPickingListByPaging(RFPickQuery pickQuery)
        {
            _rfPickDetailRepository.ChangeDB(pickQuery.WarehouseSysId);
            return _rfPickDetailRepository.GetWaitingContainerPickingListByPaging(pickQuery);
        }

        /// <summary>
        /// 获取某个出库单的待拣货商品
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public List<RFWaitingPickSkuListDto> GetWaitingPickSkuList(RFPickQuery pickQuery)
        {
            _rfPickDetailRepository.ChangeDB(pickQuery.WarehouseSysId);
            return _rfPickDetailRepository.GetWaitingPickSkuList(pickQuery);
        }

        /// <summary>
        /// 获取出库单容器拣货明细
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public RFContainerPickingDto GetContainerPickingDetailList(RFPickQuery pickQuery)
        {
            RFContainerPickingDto rsp = new RFContainerPickingDto();
            var key = string.Format(RedisSourceKey.RedisRFPicking, pickQuery.OutboundOrder, pickQuery.WarehouseSysId);
            rsp.PickingDetails = RedisWMS.GetRedisList<List<RFContainerPickingDetailListDto>>(key);
            if (rsp.PickingDetails == null || !rsp.PickingDetails.Any())
            {
                _rfPickDetailRepository.ChangeDB(pickQuery.WarehouseSysId);
                rsp = _rfPickDetailRepository.GetContainerPickingDetailList(pickQuery);
            }
            else
            {
                rsp.GroupedPickingDetails = rsp.PickingDetails.GroupBy(p => p.SkuSysId).Select(p => new RFContainerPickingDetailListDto
                {
                    SkuSysId = p.Key,
                    UPC = p.FirstOrDefault().UPC,
                    SkuName = p.FirstOrDefault().SkuName,
                    UPC01 = p.FirstOrDefault().UPC01,
                    UPC02 = p.FirstOrDefault().UPC02,
                    UPC03 = p.FirstOrDefault().UPC03,
                    UPC04 = p.FirstOrDefault().UPC04,
                    UPC05 = p.FirstOrDefault().UPC05,
                    FieldValue01 = p.FirstOrDefault().FieldValue01,
                    FieldValue02 = p.FirstOrDefault().FieldValue02,
                    FieldValue03 = p.FirstOrDefault().FieldValue03,
                    FieldValue04 = p.FirstOrDefault().FieldValue04,
                    FieldValue05 = p.FirstOrDefault().FieldValue05
                }).ToList();
            }
            return rsp;
        }

        /// <summary>
        /// 检查商品是否存在于出库单明细中
        /// </summary>
        /// <returns></returns>
        public RFCommResult CheckOutboundDetailSku(RFPickDetailDto rFPickDetailDto)
        {
            _rfPickDetailRepository.ChangeDB(rFPickDetailDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var outbound = _crudRepository.GetQuery<outbound>(x => x.OutboundOrder == rFPickDetailDto.OutboundOrder && x.WareHouseSysId == rFPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("出库单不存在");
                }

                sku sku = null;
                if (rFPickDetailDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == rFPickDetailDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == rFPickDetailDto.UPC).ToList();

                    var query = from a in outbound.outbounddetails
                                join b in skuList on a.SkuSysId equals b.SysId
                                select b;

                    sku = query.FirstOrDefault();
                }

                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                var odList = _crudRepository.GetQuery<outbounddetail>(x => x.OutboundSysId == outbound.SysId && x.SkuSysId == sku.SysId
                && (x.Status == (int)OutboundDetailStatus.New || x.Status == (int)OutboundDetailStatus.PartAllocation)).ToList();

                if (!odList.Any())
                {
                    throw new Exception("出库单中没有待拣货的此商品");
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
        /// 扫描拣货
        /// </summary>
        /// <param name="pickDetailDto"></param>
        /// <returns></returns>
        public RFCommResult ScanPickDetail(RFPickDetailDto rFPickDetailDto)
        {
            _rfPickDetailRepository.ChangeDB(rFPickDetailDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                #region 验证是否可拣货
                var outbound = _crudRepository.GetQuery<outbound>(x => x.OutboundOrder == rFPickDetailDto.OutboundOrder && x.WareHouseSysId == rFPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("出库单不存在");
                }

                sku sku = null;
                if (rFPickDetailDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == rFPickDetailDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == rFPickDetailDto.UPC).ToList();

                    var query = from a in outbound.outbounddetails
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
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rFPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (frozenSku != null)
                {
                    throw new Exception($"商品已被冻结，不能做拣货!");
                }

                var location = _crudRepository.GetQuery<location>(p => p.Loc == rFPickDetailDto.Loc && p.WarehouseSysId == rFPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (location == null)
                {
                    throw new Exception($"货位{rFPickDetailDto.Loc}已经不存在，请重新创建!");
                }
                if (location.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"货位{location.Loc}已被冻结，不能做拣货!");
                }

                var frozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == sku.SysId
                                && p.Loc == rFPickDetailDto.Loc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rFPickDetailDto.WarehouseSysId).FirstOrDefault();
                //校验冻结: 货位商品 
                if (frozenLocSku != null)
                {
                    var skuSysId = frozenLocSku.SkuSysId;
                    var frozenSkuInfo = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSkuInfo.SkuName}'在货位'{rFPickDetailDto.Loc}'已被冻结，不能做拣货!");
                }


                var odList = _crudRepository.GetQuery<outbounddetail>(x => x.OutboundSysId == outbound.SysId && x.SkuSysId == sku.SysId
                && (x.Status == (int)OutboundDetailStatus.New || x.Status == (int)OutboundDetailStatus.PartAllocation)).ToList();

                if (!odList.Any())
                {
                    throw new Exception("出库单中没有待拣货的此商品");
                }

                var totalWaitPickQty = odList.Sum(x => x.Qty - (x.AllocatedQty == null ? 0 : x.AllocatedQty) - (x.PickedQty == null ? 0 : x.PickedQty) - (x.ShippedQty == null ? 0 : x.ShippedQty));
                if (rFPickDetailDto.Qty > totalWaitPickQty)
                {
                    throw new Exception("拣货数量大于待拣货商品数量");
                }
                #endregion

                var updateInventoryDtos = new List<UpdateInventoryDto>();

                #region invSkuLoc
                var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.Loc == rFPickDetailDto.Loc && x.SkuSysId == sku.SysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                if (invSkuLoc != null)
                {
                    //可用数量
                    var invSkuLocTotalQty = CommonBussinessMethod.GetAvailableQty(invSkuLoc.Qty, invSkuLoc.AllocatedQty, invSkuLoc.PickedQty, invSkuLoc.FrozenQty);
                    if (rFPickDetailDto.Qty > invSkuLocTotalQty)
                    {
                        throw new Exception("库存数量不足");
                    }

                    //invSkuLoc.AllocatedQty += pickQty;
                    //invSkuLoc.UpdateBy = rFPickDetailDto.CurrentUserId;
                    //invSkuLoc.UpdateDate = DateTime.Now;
                    //invSkuLoc.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                    //_crudRepository.Update(invSkuLoc);
                }
                else
                {
                    throw new Exception("未找到匹配库存");
                }
                #endregion

                #region PickDetailOrder
                var oldPickDetail = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == outbound.SysId && x.Status != (int)PickDetailStatus.Cancel).FirstOrDefault();
                //var pickDetailOrder = oldPickDetail != null ? oldPickDetail.PickDetailOrder : _crudRepository.GenNextNumber(PublicConst.GenNextNumberPickDetail);
                var pickDetailOrder = oldPickDetail != null ? oldPickDetail.PickDetailOrder : _baseAppService.GetNumber(PublicConst.GenNextNumberPickDetail);
                #endregion

                #region invLotLocLpnList
                var invlotLocLpnList = _crudRepository.GetQuery<invlotloclpn>(x => x.Loc == rFPickDetailDto.Loc && x.Lpn == "" && x.SkuSysId == sku.SysId && x.WareHouseSysId == outbound.WareHouseSysId);
                #endregion

                var odPickQty = rFPickDetailDto.Qty;
                foreach (var item in odList)
                {
                    #region outboundDetail
                    var od = _crudRepository.Get<outbounddetail>(item.SysId);
                    if (odPickQty <= 0)
                    {
                        break;
                    }

                    //待拣货数量
                    var waitPickQty = od.Qty - (od.AllocatedQty == null ? 0 : od.AllocatedQty) - (od.PickedQty == null ? 0 : od.PickedQty) - (od.ShippedQty == null ? 0 : od.ShippedQty);
                    var invQty = 0;
                    if (waitPickQty > odPickQty)
                    {
                        od.AllocatedQty = (od.AllocatedQty ?? 0) + odPickQty;
                        od.Status = (int)OutboundDetailStatus.PartAllocation;
                        invQty = odPickQty;
                        odPickQty = 0;
                    }
                    else
                    {
                        od.AllocatedQty = (od.AllocatedQty ?? 0) + waitPickQty;
                        od.Status = (int)OutboundDetailStatus.Allocation;
                        invQty = (int)waitPickQty;
                        odPickQty -= (int)waitPickQty;
                    }
                    od.UpdateBy = rFPickDetailDto.CurrentUserId;
                    od.UpdateDate = DateTime.Now;
                    od.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                    _crudRepository.Update(od);
                    #endregion

                    #region invLotLocLpn
                    if (invlotLocLpnList != null && invlotLocLpnList.Count() > 0)
                    {
                        var pickQty = invQty;
                        //可用数量
                        var invLotLocLpnTotalQty = invlotLocLpnList.Sum(x => x.Qty - x.AllocatedQty - x.PickedQty - x.FrozenQty);
                        if (pickQty > invLotLocLpnTotalQty)
                        {
                            throw new Exception("库存数量不足");
                        }

                        foreach (var illl in invlotLocLpnList)
                        {
                            if (pickQty <= 0)
                            {
                                break;
                            }

                            //var invlotloclpn = _crudRepository.Get<invlotloclpn>(illl.SysId);
                            var invLotLocLpnQty = CommonBussinessMethod.GetAvailableQty(illl.Qty, illl.AllocatedQty, illl.PickedQty, illl.FrozenQty);

                            if (invLotLocLpnQty <= 0)
                            {
                                continue;
                            }

                            var illlQty = 0;
                            if (invLotLocLpnQty > pickQty)
                            {
                                //invlotloclpn.AllocatedQty += pickQty;
                                //invlotloclpn.UpdateBy = rFPickDetailDto.CurrentUserId;
                                //invlotloclpn.UpdateDate = DateTime.Now;
                                //invlotloclpn.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                                illlQty = pickQty;
                                pickQty = 0;
                            }
                            else
                            {
                                //invlotloclpn.AllocatedQty += invLotLocLpnQty;
                                //invlotloclpn.UpdateBy = rFPickDetailDto.CurrentUserId;
                                //invlotloclpn.UpdateDate = DateTime.Now;
                                //invlotloclpn.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                                illlQty = invLotLocLpnQty;
                                pickQty -= invLotLocLpnQty;
                            }
                            //_crudRepository.Update(invlotloclpn);

                            #region invLot 
                            var invLot = _crudRepository.GetQuery<invlot>(x => x.Lot == illl.Lot && x.SkuSysId == sku.SysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                            if (invLot != null)
                            {
                                //可用数量
                                var invLotTotalQty = CommonBussinessMethod.GetAvailableQty(invLot.Qty, invLot.AllocatedQty, invLot.PickedQty, invLot.FrozenQty);
                                if (illlQty > invLotTotalQty)
                                {
                                    throw new Exception("库存数量不足");
                                }

                                //invLot.AllocatedQty += illlQty;
                                //invLot.UpdateBy = rFPickDetailDto.CurrentUserId;
                                //invLot.UpdateDate = DateTime.Now;
                                //invLot.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                                //_crudRepository.Update(invLot);
                            }
                            else
                            {
                                throw new Exception("未找到匹配库存");
                            }
                            #endregion

                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = illl.SysId,
                                InvLotSysId = invLot.SysId,
                                InvSkuLocSysId = invSkuLoc.SysId,
                                Qty = illlQty,
                                CurrentUserId = rFPickDetailDto.CurrentUserId,
                                CurrentDisplayName = rFPickDetailDto.CurrentDisplayName,
                                WarehouseSysId = rFPickDetailDto.WarehouseSysId,
                            });

                            #region pickdetail
                            var pickDetail = new pickdetail()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = outbound.WareHouseSysId,
                                OutboundSysId = outbound.SysId,
                                OutboundDetailSysId = item.SysId,
                                PickDetailOrder = pickDetailOrder,
                                Status = (int)PickDetailStatus.New,
                                SkuSysId = item.SkuSysId,
                                UOMSysId = item.UOMSysId,
                                PackSysId = item.PackSysId,
                                Loc = illl.Loc,
                                Lot = illl.Lot,
                                Lpn = illl.Lpn,
                                Qty = illlQty,
                                CreateBy = rFPickDetailDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = rFPickDetailDto.CurrentDisplayName,
                                UpdateBy = rFPickDetailDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = rFPickDetailDto.CurrentDisplayName
                            };
                            _crudRepository.Insert(pickDetail);
                            #endregion
                        }
                    }
                    else
                    {
                        throw new Exception("未找到匹配库存");
                    }
                    #endregion
                }

                #region outbound
                var odTotalQty = 0;
                var outbounddetailList = _crudRepository.GetQuery<outbounddetail>(x => x.OutboundSysId == outbound.SysId);
                if (outbounddetailList != null && outbounddetailList.Count() > 0)
                {
                    foreach (var item in outbounddetailList)
                    {
                        var odd = _crudRepository.Get<outbounddetail>(item.SysId);
                        odTotalQty += (odd.Qty ?? 0) - (odd.AllocatedQty ?? 0) - (odd.PickedQty ?? 0) - (odd.ShippedQty ?? 0);
                    }
                }

                outbound.Status = odTotalQty > 0 ? (int)OutboundStatus.PartAllocation : (int)OutboundStatus.Allocation;
                outbound.TotalAllocatedQty = (outbound.TotalAllocatedQty ?? 0) + rFPickDetailDto.Qty;
                outbound.UpdateBy = rFPickDetailDto.CurrentUserId;
                outbound.UpdateDate = DateTime.Now;
                outbound.UpdateUserName = rFPickDetailDto.CurrentDisplayName;
                _crudRepository.Update(outbound);
                #endregion

                //执行扣减库存方法 
                _WMSSqlRepository.UpdateInventoryAllocatedQty(updateInventoryDtos);

                result.IsSucess = true;
                result.Message = "拣货成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 判断扫描的单号是否待容器拣货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RFCommResult CheckContainerPickingOutboundOrder(RFOutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            var rsp = new RFCommResult { IsSucess = true };
            var outbound = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == outboundQuery.OutboundOrder
                && p.WareHouseSysId == outboundQuery.WarehouseSysId
                && p.Status == (int)OutboundStatus.Allocation).FirstOrDefault();
            if (outbound == null)
            {
                var pickDetail = _crudRepository.GetQuery<pickdetail>(p => p.PickDetailOrder == outboundQuery.OutboundOrder
                    && p.WareHouseSysId == outboundQuery.WarehouseSysId
                    && p.Status == (int)PickDetailStatus.New
                    && p.Qty != p.PickedQty).FirstOrDefault();
                if (pickDetail == null)
                {
                    rsp.IsSucess = false;
                    rsp.Message = "待拣货单据中不存在此单据号";
                }
                else
                {
                    var pickDetailOutbound = _crudRepository.GetQuery<outbound>(p => p.SysId == pickDetail.OutboundSysId).FirstOrDefault();
                    rsp.Message = pickDetailOutbound == null ? string.Empty : pickDetailOutbound.OutboundOrder;
                }
            }
            else
            {
                rsp.Message = outbound.OutboundOrder;
            }
            return rsp;
        }

        /// <summary>
        /// 判断容器是否可用
        /// </summary>
        /// <param name="storageCase"></param>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public RFCommResult CheckContainerIsAvailable(string storageCase, string outboundOrder, Guid warehouseSysId)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _crudRepository.ChangeDB(warehouseSysId);
                var container = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == storageCase && p.WareHouseSysId == warehouseSysId).FirstOrDefault();
                if (container == null)
                {
                    throw new Exception("容器不存在");
                }
                if (container.Status != (int)PreBulkPackStatus.New && container.OutboundOrder != outboundOrder)
                {
                    throw new Exception($"容器正在被出库单 [{container.OutboundOrder}] 使用，请重新扫描");
                }
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                rsp.Message = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// 容器拣货扫描
        /// </summary>
        /// <param name="pickingDetailDto"></param>
        /// <returns></returns>
        public RFCommResult GenerateContainerPickingDetail(RFGenerateContainerPickingDetailDto pickingDetailDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _rfPickDetailRepository.ChangeDB(pickingDetailDto.WarehouseSysId);
                var outboundSysId = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == pickingDetailDto.OutboundOrder && p.WareHouseSysId == pickingDetailDto.WarehouseSysId).FirstOrDefault().SysId;
                //更新pickdetail表的PickedQty
                var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outboundSysId && p.Status == (int)PickDetailStatus.New && p.SkuSysId == pickingDetailDto.SkuSysId && p.Loc == pickingDetailDto.Loc).ToList();
                if (pickDetails == null || !pickDetails.Any())
                {
                    throw new Exception("商品在该库位不存在拣货信息");
                }
                var availablePickQty = pickDetails.Sum(p => p.Qty.GetValueOrDefault() - p.PickedQty);
                if (availablePickQty < pickingDetailDto.PickedQty)
                {
                    throw new Exception("商品在该库位可拣货数量不足，无法拣货");
                }

                var toPickQty = pickingDetailDto.PickedQty;
                List<UpdatePickDetailDto> updatePickDetailList = new List<UpdatePickDetailDto>();
                foreach (var pickDetail in pickDetails)
                {
                    if (toPickQty == 0) break;
                    int pickedQty = 0;
                    if ((pickDetail.Qty - pickDetail.PickedQty).GetValueOrDefault() > toPickQty)
                    {
                        pickedQty = toPickQty;
                        toPickQty -= toPickQty;
                    }
                    else
                    {
                        pickedQty = (pickDetail.Qty - pickDetail.PickedQty).GetValueOrDefault();
                        toPickQty -= (pickDetail.Qty - pickDetail.PickedQty).GetValueOrDefault();
                    }
                    updatePickDetailList.Add(new UpdatePickDetailDto
                    {
                        SysId = pickDetail.SysId,
                        SkuSysId = pickDetail.SkuSysId,
                        PickedQty = pickedQty,
                        Loc = pickDetail.Loc,
                        Lot = pickDetail.Lot,
                        CurrentUserId = pickingDetailDto.CurrentUserId,
                        CurrentDisplayName = pickingDetailDto.CurrentDisplayName
                    });
                }
                if (toPickQty > 0)
                {
                    throw new Exception("商品在该库位可拣货数量不足，无法拣货");
                }
                _WMSSqlRepository.UpdatePickDetailRFContainerPicking(updatePickDetailList);

                //更新prebulkpack绑定的出库单
                var container = _crudRepository.GetQuery<prebulkpack>(p => p.StorageCase == pickingDetailDto.StorageCase && p.WareHouseSysId == pickingDetailDto.WarehouseSysId).FirstOrDefault();
                if (container != null)
                {
                    if (container.Status == (int)PreBulkPackStatus.PrePack && (container.OutboundOrder != pickingDetailDto.OutboundOrder || container.OutboundSysId != outboundSysId))
                    {
                        throw new Exception($"容器正在被出库单 [{container.OutboundOrder}] 使用，请重新扫描");
                    }
                    var containerDetailCount = _crudRepository.GetQuery<prebulkpackdetail>(p => p.PreBulkPackSysId == container.SysId).Count();
                    if (container.Status == (int)PreBulkPackStatus.New && containerDetailCount == 0 && updatePickDetailList.Count > 0)
                    {
                        container.Status = (int)PreBulkPackStatus.PrePack;
                        container.OutboundOrder = pickingDetailDto.OutboundOrder;
                        container.OutboundSysId = outboundSysId;
                        _crudRepository.Update(container);
                    }

                    //插入/更新prebulkpackdetail
                    var outboundDetailSysId = pickDetails.FirstOrDefault().OutboundDetailSysId;
                    var outboundDetail = _crudRepository.GetQuery<outbounddetail>(p => p.SysId == outboundDetailSysId).FirstOrDefault();
                    foreach (var pickDetail in updatePickDetailList)
                    {
                        var containerDetail = _crudRepository.GetQuery<prebulkpackdetail>(p => p.PreBulkPackSysId == container.SysId && p.SkuSysId == pickingDetailDto.SkuSysId && p.Loc == pickDetail.Loc && p.Lot == pickDetail.Lot).FirstOrDefault();
                        if (containerDetail == null)
                        {
                            _crudRepository.Insert(new prebulkpackdetail
                            {
                                SysId = Guid.NewGuid(),
                                PreBulkPackSysId = container.SysId,
                                SkuSysId = pickDetail.SkuSysId,
                                UOMSysId = outboundDetail.UOMSysId,
                                PackSysId = outboundDetail.PackSysId,
                                Loc = pickDetail.Loc,
                                Lot = pickDetail.Lot,
                                Qty = pickDetail.PickedQty,
                                CreateBy = pickingDetailDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = pickingDetailDto.CurrentDisplayName,
                                UpdateBy = pickingDetailDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = pickingDetailDto.CurrentDisplayName
                            });
                        }
                        else
                        {
                            _crudRepository.Update<prebulkpackdetail>(containerDetail.SysId, p =>
                            {
                                p.Qty += pickDetail.PickedQty;
                                p.UpdateBy = pickingDetailDto.CurrentUserId;
                                p.UpdateDate = DateTime.Now;
                                p.UpdateUserName = pickingDetailDto.CurrentDisplayName;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                rsp.Message = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// RF拣货记录缓存
        /// </summary>
        /// <param name="setRedisDto"></param>
        /// <returns></returns>
        public RFCommResult RFSetPickingRedis(RFPickFinishDto setRedisDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                var key = string.Format(RedisSourceKey.RedisRFPicking, setRedisDto.OutboundOrder, setRedisDto.WarehouseSysId);
                //RedisWMS.CleanRedis<List<RFContainerPickingDetailListDto>>(key);
                RedisWMS.SetRedis(setRedisDto.PickingDetailList, key);
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                rsp.Message = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// RF拣货完成
        /// </summary>
        /// <param name="pickFinishDto"></param>
        /// <returns></returns>
        public RFCommResult RFPickFinish(RFPickFinishDto pickFinishDto)
        {
            RFCommResult rsp = new RFCommResult { IsSucess = true };
            try
            {
                _rfPickDetailRepository.ChangeDB(pickFinishDto.WarehouseSysId);
                var outboundSysId = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == pickFinishDto.OutboundOrder && p.WareHouseSysId == pickFinishDto.WarehouseSysId).FirstOrDefault().SysId;
                if (pickFinishDto.PickingDetailList == null || !pickFinishDto.PickingDetailList.Any())
                {
                    throw new Exception("未找到拣货明细，请重新拣货");
                }
                var exceptionSku = pickFinishDto.PickingDetailList.FirstOrDefault(p => p.ContainerInfos.Sum(x => x.ContainerQty) != p.CurrentPickedQty);
                if (exceptionSku != null)
                {
                    throw new Exception($"商品 [{exceptionSku.SkuName}] 拣货数量异常");
                }
                var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outboundSysId && p.Status == (int)PickDetailStatus.New).ToList();
                //更新pickdetail表的PickedQty
                List<UpdatePickDetailDto> updatePickDetailList = new List<UpdatePickDetailDto>();
                foreach (var pickingDetail in pickFinishDto.PickingDetailList)
                {
                    var pickDetail = pickDetails.FirstOrDefault(p => p.SysId == pickingDetail.SysId);
                    if (pickDetail != null)
                    {
                        if (pickingDetail.CurrentPickedQty != 0)
                        {
                            updatePickDetailList.Add(new UpdatePickDetailDto
                            {
                                SysId = pickingDetail.SysId,
                                SkuSysId = pickingDetail.SkuSysId,
                                PickedQty = pickingDetail.CurrentPickedQty,
                                Loc = pickingDetail.Loc,
                                Lot = pickDetail.Lot,
                                ContainerInfos = pickingDetail.ContainerInfos,
                                CurrentUserId = pickFinishDto.CurrentUserId,
                                CurrentDisplayName = pickFinishDto.CurrentDisplayName
                            });
                        }
                    }
                    else
                    {
                        throw new Exception("未找到拣货信息");
                    }
                }
                if (updatePickDetailList.Count > 0)
                {
                    _WMSSqlRepository.UpdatePickDetailRFContainerPicking(updatePickDetailList);
                }

                List<string> storageCaseList = new List<string>();
                foreach (var pickingDetailList in pickFinishDto.PickingDetailList)
                {
                    storageCaseList.AddRange(pickingDetailList.ContainerInfos.Select(p => p.StorageCase));
                }
                var containers = _crudRepository.GetQuery<prebulkpack>(p => storageCaseList.Contains(p.StorageCase) && p.WareHouseSysId == pickFinishDto.WarehouseSysId).ToList();
                if (containers != null && containers.Any())
                {
                    var inUseContainer = containers.FirstOrDefault(p => p.Status == (int)PreBulkPackStatus.PrePack && (p.OutboundOrder != pickFinishDto.OutboundOrder || p.OutboundSysId != outboundSysId));
                    if (inUseContainer != null)
                    {
                        throw new Exception($"容器 [{inUseContainer.StorageCase}] 正在被出库单 [{inUseContainer.OutboundOrder}] 使用，请重新扫描");
                    }
                    //更新prebulkpack绑定的出库单
                    foreach (var container in containers)
                    {
                        container.Status = (int)PreBulkPackStatus.RFPicking;
                        container.OutboundOrder = pickFinishDto.OutboundOrder;
                        container.OutboundSysId = outboundSysId;
                        _crudRepository.Update(container);
                    }
                }

                //插入/更新prebulkpackdetail
                var outbound = _crudRepository.GetQuery<outbound>(p => p.OutboundOrder == pickFinishDto.OutboundOrder && p.WareHouseSysId == pickFinishDto.WarehouseSysId).FirstOrDefault();
                var outboundDetails = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outbound.SysId).ToList();
                foreach (var pickDetail in updatePickDetailList)
                {
                    var outboundDetail = outboundDetails.FirstOrDefault(p => p.SkuSysId == pickDetail.SkuSysId);
                    pickDetail.ContainerInfos = pickDetail.ContainerInfos.GroupBy(p => new { p.StorageCase }).Select(p => new ContainerInfo { StorageCase = p.Key.StorageCase, ContainerQty = p.Sum(x => x.ContainerQty) }).ToList();
                    foreach (var containerInfo in pickDetail.ContainerInfos)
                    {
                        if (string.IsNullOrEmpty(containerInfo.StorageCase)) continue;
                        var container = containers.FirstOrDefault(p => string.Compare(p.StorageCase, containerInfo.StorageCase, StringComparison.OrdinalIgnoreCase) == 0);
                        var containerDetail = _crudRepository.GetQuery<prebulkpackdetail>(p => p.PreBulkPackSysId == container.SysId && p.SkuSysId == pickDetail.SkuSysId && p.Loc == pickDetail.Loc && p.Lot == pickDetail.Lot).FirstOrDefault();
                        if (containerDetail == null)
                        {
                            _crudRepository.Insert(new prebulkpackdetail
                            {
                                SysId = Guid.NewGuid(),
                                PreBulkPackSysId = container.SysId,
                                SkuSysId = pickDetail.SkuSysId,
                                UOMSysId = outboundDetail.UOMSysId,
                                PackSysId = outboundDetail.PackSysId,
                                Loc = pickDetail.Loc,
                                Lot = pickDetail.Lot,
                                Qty = containerInfo.ContainerQty,
                                CreateBy = pickFinishDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = pickFinishDto.CurrentDisplayName,
                                UpdateBy = pickFinishDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = pickFinishDto.CurrentDisplayName
                            });
                        }
                        else
                        {
                            _crudRepository.Update<prebulkpackdetail>(containerDetail.SysId, p =>
                            {
                                p.Qty += containerInfo.ContainerQty;
                                p.UpdateBy = pickFinishDto.CurrentUserId;
                                p.UpdateDate = DateTime.Now;
                                p.UpdateUserName = pickFinishDto.CurrentDisplayName;
                            });
                        }
                    }
                }
                //清除拣货缓存
                var key = string.Format(RedisSourceKey.RedisRFPicking, pickFinishDto.OutboundOrder, pickFinishDto.WarehouseSysId);
                RedisWMS.CleanRedis<List<RFContainerPickingDetailListDto>>(key);
            }
            catch (Exception ex)
            {
                rsp.IsSucess = false;
                rsp.Message = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// 拣货完成结果
        /// </summary>
        /// <param name="pickQuery"></param>
        /// <returns></returns>
        public RFPickResultDto GetPickResult(RFPickQuery pickQuery)
        {
            RFPickResultDto rsp = new RFPickResultDto();
            _rfPickDetailRepository.ChangeDB(pickQuery.WarehouseSysId);
            var pickDetails = _rfPickDetailRepository.GetContainerPickingDetailList(pickQuery).PickingDetails;
            rsp.PickedList = pickDetails.Where(p => p.PickedQty != 0).ToList();
            rsp.ToPickList = pickDetails.Where(p => p.PickedQty == 0).ToList();
            rsp.PickDiffList = pickDetails.Where(p => p.Qty != p.PickedQty).ToList();
            return rsp;
        }

        #region 加工单拣货
        /// <summary>
        /// 获取待拣货的加工单
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        public Pages<RFWaitingAssemblyPickListDto> GetWaitingAssemblyList(RFAssemblyPickQuery assemblyPickQuery)
        {
            _rfPickDetailRepository.ChangeDB(assemblyPickQuery.WarehouseSysId);
            return _rfPickDetailRepository.GetWaitingAssemblyList(assemblyPickQuery);
        }

        /// <summary>
        /// 获取加工单中的待拣货商品
        /// </summary>
        /// <param name="assemblyPickQuery"></param>
        /// <returns></returns>
        public List<RFWaitingAssemblyPickSkuListDto> GetWaitingAssemblyPickSkuList(RFAssemblyPickQuery assemblyPickQuery)
        {
            _rfPickDetailRepository.ChangeDB(assemblyPickQuery.WarehouseSysId);
            var response = _rfPickDetailRepository.GetWaitingAssemblyPickSkuList(assemblyPickQuery);
            return response;
        }

        /// <summary>
        /// 检查商品是否存在于加工单明细中
        /// </summary>
        /// <returns></returns>
        public RFCommResult CheckAssemblyDetailSku(RFAssemblyPickDetailDto rfAssemblyPickDetailDto)
        {
            _rfPickDetailRepository.ChangeDB(rfAssemblyPickDetailDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                var assembly = _crudRepository.GetQuery<assembly>(x => x.AssemblyOrder == rfAssemblyPickDetailDto.AssemblyOrder && x.WareHouseSysId == rfAssemblyPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (assembly == null)
                {
                    throw new Exception("加工单不存在");
                }

                var sku = _crudRepository.GetQuery<sku>(x => x.UPC == rfAssemblyPickDetailDto.UPC).FirstOrDefault();
                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                var adList = _crudRepository.GetQuery<assemblydetail>(x => x.AssemblySysId == assembly.SysId && x.SkuSysId == sku.SysId
                && (x.Status == (int)AssemblyDetailStatus.New || x.Status == (int)AssemblyDetailStatus.PartPicking)).ToList();

                if (!adList.Any())
                {
                    throw new Exception("加工单中没有待拣货的此商品");
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
        /// 加工单扫描拣货
        /// </summary>
        /// <param name="pickDetailDto"></param>
        /// <returns></returns>
        public RFCommResult AssemblyScanPickDetail(RFAssemblyPickDetailDto rfAssemblyPickDetailDto)
        {
            _rfPickDetailRepository.ChangeDB(rfAssemblyPickDetailDto.WarehouseSysId);
            var result = new RFCommResult() { IsSucess = false };
            try
            {
                #region 验证是否可拣货
                var assembly = _crudRepository.GetQuery<assembly>(x => x.AssemblyOrder == rfAssemblyPickDetailDto.AssemblyOrder && x.WareHouseSysId == rfAssemblyPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (assembly == null)
                {
                    throw new Exception("加工单不存在");
                }
                var assembRule = _crudRepository.FirstOrDefault<assemblyrule>(x => x.WarehouseSysId == rfAssemblyPickDetailDto.WarehouseSysId);
                sku sku = null;
                if (rfAssemblyPickDetailDto.SkuSysId.HasValue)
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.SysId == rfAssemblyPickDetailDto.SkuSysId.Value).FirstOrDefault();
                }
                else
                {
                    sku = _crudRepository.GetQuery<sku>(x => x.UPC == rfAssemblyPickDetailDto.UPC).FirstOrDefault();
                }
                if (sku == null)
                {
                    throw new Exception("商品不存在");
                }

                //冻结校验
                var frozenSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && p.SkuSysId.Value == sku.SysId
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rfAssemblyPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (frozenSku != null)
                {
                    throw new Exception($"商品已被冻结，不能做拣货!");
                }

                var location = _crudRepository.GetQuery<location>(p => p.Loc == rfAssemblyPickDetailDto.Loc && p.WarehouseSysId == rfAssemblyPickDetailDto.WarehouseSysId).FirstOrDefault();
                if (location == null)
                {
                    throw new Exception($"货位{rfAssemblyPickDetailDto.Loc}已经不存在，请重新创建!");
                }
                if (location.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"货位{location.Loc}已被冻结，不能做拣货!");
                }

                var frozenLocSku = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && p.SkuSysId.Value == sku.SysId
                               && p.Loc == rfAssemblyPickDetailDto.Loc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == rfAssemblyPickDetailDto.WarehouseSysId).FirstOrDefault();
                //校验冻结: 货位商品 
                if (frozenLocSku != null)
                {
                    var skuSysId = frozenLocSku.SkuSysId;
                    var frozenSkuInfo = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSkuInfo.SkuName}'在货位'{rfAssemblyPickDetailDto.Loc}'已被冻结，不能做拣货!");
                }


                var adList = _crudRepository.GetQuery<assemblydetail>(x => x.AssemblySysId == assembly.SysId && x.SkuSysId == sku.SysId
                && (x.Status == (int)AssemblyDetailStatus.New || x.Status == (int)AssemblyDetailStatus.PartPicking)).ToList();

                if (!adList.Any())
                {
                    throw new Exception("加工单中没有待拣货的此商品");
                }

                var totalWaitPickQty = adList.Sum(x => x.Qty - x.AllocatedQty - x.PickedQty);

                #region 单位数量转换
                int qty = 0;
                pack pack = new pack();
                if (_packageAppService.GetSkuConversiontransQty(sku.SysId, rfAssemblyPickDetailDto.DisplayQty, out qty, ref pack) == true)
                {
                    unitconversiontran unitTran = new unitconversiontran()
                    {
                        WareHouseSysId = rfAssemblyPickDetailDto.WarehouseSysId,
                        DocOrder = assembly.AssemblyOrder,
                        DocSysId = assembly.SysId,
                        DocDetailSysId = Guid.Empty,
                        SkuSysId = sku.SysId,
                        FromQty = rfAssemblyPickDetailDto.DisplayQty,
                        ToQty = qty,
                        Loc = rfAssemblyPickDetailDto.Loc,
                        Lot = "",
                        Lpn = "",
                        Status = "Done",
                        PackSysId = pack.SysId,
                        PackCode = pack.PackCode,
                        FromUOMSysId = pack.FieldUom02 ?? Guid.Empty,
                        ToUOMSysId = pack.FieldUom01 ?? Guid.Empty,
                        CreateBy = rfAssemblyPickDetailDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = rfAssemblyPickDetailDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        TransType = InvTransType.Assembly,
                        SourceTransType = InvSourceTransType.AssemblyPicking
                    };
                    _crudRepository.Insert(unitTran);

                    rfAssemblyPickDetailDto.Qty = qty;
                }
                else
                {
                    rfAssemblyPickDetailDto.Qty = (int)rfAssemblyPickDetailDto.DisplayQty;
                }
                #endregion

                if (rfAssemblyPickDetailDto.Qty > totalWaitPickQty)
                {
                    throw new Exception("拣货数量大于待拣货商品数量");
                }
                #endregion

                var updateInventoryDtos = new List<UpdateInventoryDto>();

                #region invSkuLoc
                var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.Loc == rfAssemblyPickDetailDto.Loc && x.SkuSysId == sku.SysId && x.WareHouseSysId == assembly.WareHouseSysId).FirstOrDefault();
                if (invSkuLoc != null)
                {
                    //可用数量
                    var invSkuLocTotalQty = CommonBussinessMethod.GetAvailableQty(invSkuLoc.Qty, invSkuLoc.AllocatedQty, invSkuLoc.PickedQty, invSkuLoc.FrozenQty);
                    if (rfAssemblyPickDetailDto.Qty > invSkuLocTotalQty)
                    {
                        throw new Exception("库存数量不足");
                    }
                }
                else
                {
                    throw new Exception("未找到匹配库存");
                }
                #endregion

                #region PickDetailOrder
                var oldPickDetail = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == assembly.SysId && x.Status != (int)PickDetailStatus.Cancel).FirstOrDefault();
                //var pickDetailOrder = oldPickDetail != null ? oldPickDetail.PickDetailOrder : _crudRepository.GenNextNumber(PublicConst.GenNextNumberPickDetail);
                var pickDetailOrder = oldPickDetail != null ? oldPickDetail.PickDetailOrder : _baseAppService.GetNumber(PublicConst.GenNextNumberPickDetail);
                #endregion

                #region invLotLocLpnList
                //注销原有查询
                //var invlotLocLpnList = _crudRepository.GetQuery<invlotloclpn>(x => x.Loc == rfAssemblyPickDetailDto.Loc && x.Lpn == "" && x.SkuSysId == sku.SysId && x.WareHouseSysId == assembly.WareHouseSysId);

                var invlotLocLpnList = _rfPickDetailRepository.GetInvlotloclpnList(rfAssemblyPickDetailDto.Loc, sku.SysId, assembly.WareHouseSysId, assembly.Channel, assembRule);

                #endregion

                var adPickQty = rfAssemblyPickDetailDto.Qty;
                foreach (var item in adList)
                {
                    #region assemblyDetail
                    var ad = _crudRepository.Get<assemblydetail>(item.SysId);
                    if (adPickQty <= 0)
                    {
                        break;
                    }

                    //待拣货数量
                    var waitPickQty = ad.Qty - ad.AllocatedQty - ad.PickedQty;
                    var invQty = 0;
                    if (waitPickQty > adPickQty)
                    {
                        ad.PickedQty = ad.PickedQty + adPickQty;
                        ad.Status = (int)AssemblyDetailStatus.PartPicking;
                        invQty = adPickQty;
                        adPickQty = 0;
                    }
                    else
                    {
                        ad.PickedQty = ad.PickedQty + waitPickQty;
                        ad.Status = (int)AssemblyDetailStatus.Picking;
                        invQty = (int)waitPickQty;
                        adPickQty -= (int)waitPickQty;
                    }
                    ad.UpdateBy = rfAssemblyPickDetailDto.CurrentUserId;
                    ad.UpdateDate = DateTime.Now;
                    ad.UpdateUserName = rfAssemblyPickDetailDto.CurrentDisplayName;
                    _crudRepository.Update(ad);
                    #endregion

                    #region invLotLocLpn
                    if (invlotLocLpnList != null && invlotLocLpnList.Count() > 0)
                    {
                        var pickQty = invQty;
                        //可用数量
                        var invLotLocLpnTotalQty = invlotLocLpnList.Sum(x => x.Qty - x.AllocatedQty - x.PickedQty - x.FrozenQty);
                        if (pickQty > invLotLocLpnTotalQty)
                        {
                            throw new Exception("库存数量不足");
                        }

                        foreach (var illl in invlotLocLpnList)
                        {
                            if (pickQty <= 0)
                            {
                                break;
                            }

                            var invLotLocLpnQty = CommonBussinessMethod.GetAvailableQty(illl.Qty, illl.AllocatedQty, illl.PickedQty, illl.FrozenQty);

                            if (invLotLocLpnQty <= 0)
                            {
                                continue;
                            }

                            var illlQty = 0;
                            if (invLotLocLpnQty > pickQty)
                            {
                                illlQty = pickQty;
                                pickQty = 0;
                            }
                            else
                            {
                                illlQty = invLotLocLpnQty;
                                pickQty -= invLotLocLpnQty;
                            }

                            #region invLot 
                            var invLot = _crudRepository.GetQuery<invlot>(x => x.Lot == illl.Lot && x.SkuSysId == sku.SysId && x.WareHouseSysId == assembly.WareHouseSysId).FirstOrDefault();

                            if (invLot != null)
                            {
                                //可用数量
                                var invLotTotalQty = CommonBussinessMethod.GetAvailableQty(invLot.Qty, invLot.AllocatedQty, invLot.PickedQty, invLot.FrozenQty);
                                if (illlQty > invLotTotalQty)
                                {
                                    throw new Exception("库存数量不足");
                                }
                            }
                            else
                            {
                                throw new Exception("未找到匹配库存");
                            }
                            #endregion

                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = illl.SysId,
                                InvLotSysId = invLot.SysId,
                                InvSkuLocSysId = invSkuLoc.SysId,
                                Qty = illlQty,
                                CurrentUserId = rfAssemblyPickDetailDto.CurrentUserId,
                                CurrentDisplayName = rfAssemblyPickDetailDto.CurrentDisplayName,
                                WarehouseSysId = rfAssemblyPickDetailDto.WarehouseSysId,
                            });

                            #region pickdetail
                            var pickDetail = new pickdetail()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = assembly.WareHouseSysId,
                                OutboundSysId = assembly.SysId,
                                OutboundDetailSysId = item.SysId,
                                PickDetailOrder = pickDetailOrder,
                                PickDate = DateTime.Now,
                                Status = (int)PickDetailStatus.Finish,
                                SkuSysId = item.SkuSysId,
                                UOMSysId = pack != null ? pack.FieldUom01 : null,
                                PackSysId = sku.PackSysId,
                                Loc = illl.Loc,
                                Lot = illl.Lot,
                                Lpn = illl.Lpn,
                                Qty = illlQty,
                                CreateBy = rfAssemblyPickDetailDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = rfAssemblyPickDetailDto.CurrentDisplayName,
                                UpdateBy = rfAssemblyPickDetailDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = rfAssemblyPickDetailDto.CurrentDisplayName,
                                SourceType = (int)PickDetailSourceType.Assembly
                            };
                            _crudRepository.Insert(pickDetail);
                            #endregion
                        }
                    }
                    else
                    {
                        throw new Exception("未找到匹配库存");
                    }
                    #endregion
                }

                //执行扣减库存方法 
                _WMSSqlRepository.UpdateInventoryAssemblyPickedQty(updateInventoryDtos);

                result.IsSucess = true;
                result.Message = "拣货成功";
            }
            catch (Exception ex)
            {
                result.IsSucess = false;
                throw new Exception(ex.Message);
            }
            return result;
        }
        #endregion
    }
}

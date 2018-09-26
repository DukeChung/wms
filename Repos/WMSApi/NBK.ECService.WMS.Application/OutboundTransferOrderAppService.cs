using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;

namespace NBK.ECService.WMS.Application
{
    public class OutboundTransferOrderAppService : WMSApplicationService, IOutboundTransferOrderAppService
    {
        private IOutboundTransferOrderRepository _crudRepository = null;

        public OutboundTransferOrderAppService(IOutboundTransferOrderRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundTransferOrderDto> GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetOutboundTransferOrderByPage(request);
        }

        public OutboundTransferOrderDto GetDataBySysId(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetDataBySysId(sysId);
        }

        /// <summary>
        /// 根据出库单ID更新所有交接单状态
        /// </summary>
        public void UpdateOutboundTransferOrder(OutboundTransferOrderQueryDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            _crudRepository.UpdateOutboundTransferOrder(dto);
        }


        /// <summary>
        /// 根据出库单ID更新所有交接单状态
        /// </summary>
        public void DeleteOutboundTransferOrder(OutboundTransferOrderQueryDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            _crudRepository.DeleteOutboundTransferOrder(dto);
        }

        public List<OutboundTransferPrintDto> GetOutboundTransferBox(List<Guid> request, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var list = _crudRepository.GetOutboundTransferBox(request);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.ConsigneeArea = !string.IsNullOrEmpty(item.ConsigneeArea) ? item.ConsigneeArea : "";
                    item.ConsigneeTown = !string.IsNullOrEmpty(item.ConsigneeTown) ? item.ConsigneeTown : "";

                    if (item.OutboundType == (int)OutboundType.TransferInventory)
                    {
                        item.ServiceStationName = item.ToWareHouseName;
                        item.OutboundChildType = "移仓";
                        item.ConsigneeArea = "";
                        item.ConsigneeTown = "";
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 根据出库单获取所有交接单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<OutboundTransferPrintDto> GetOutboundTransferOrder(OutboundTransferOrderQuery dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            return _crudRepository.GetOutboundTransferOrder(dto);
        }

        /// <summary>
        /// 更新交接明细内容
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse UpdateTransferOrderSku(OutboundTransferOrderMoveDto dto)
        {
            var result = new CommonResponse() { IsSuccess = true };
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            try
            {
                #region 操作移出交接单
                var transferOrderDetail = _crudRepository.GetQuery<outboundtransferorderdetail>(x => x.SysId == dto.OutboundTransferOrderDetailSyId).FirstOrDefault();
                if (transferOrderDetail == null)
                {
                    throw new Exception("要移出的交接单明细不存在");
                }
                if (transferOrderDetail.Qty < dto.MoveSkuQty)
                {
                    throw new Exception("移出数量不能大于交接单明细数量");
                }

                if (transferOrderDetail.Qty - dto.MoveSkuQty == 0)
                {
                    _crudRepository.Delete<outboundtransferorderdetail>(transferOrderDetail.SysId);

                    #region 如果该交接箱中仅有一条记录，更新主表状态到新建,箱型到未标记
                    var list = _crudRepository.GetQuery<outboundtransferorderdetail>(x => x.OutboundTransferOrderSysId == transferOrderDetail.OutboundTransferOrderSysId).ToList();
                    if (list.Count == 1)
                    {
                        _crudRepository.Update<outboundtransferorder>(transferOrderDetail.OutboundTransferOrderSysId, p =>
                        {
                            p.TransferType = (int)OutboundTransferOrderType.Unmark;
                            p.Status = (int)OutboundTransferOrderStatus.New;
                            p.UpdateDate = DateTime.Now;
                            p.UpdateBy = dto.CurrentUserId;
                            p.UpdateUserName = dto.CurrentDisplayName;
                            p.ReviewBy = null;
                            p.ReviewDate = null;
                            p.ReviewUserName = null;
                        });
                    }
                    #endregion
                }
                else
                {
                    transferOrderDetail.Qty = transferOrderDetail.Qty - dto.MoveSkuQty;
                    transferOrderDetail.UpdateBy = dto.CurrentUserId;
                    transferOrderDetail.UpdateDate = DateTime.Now;
                    transferOrderDetail.UpdateUserName = dto.CurrentDisplayName;
                    _crudRepository.Update<outboundtransferorderdetail>(transferOrderDetail);
                }
                #endregion


                #region 操作移入交接单内容
                var outboundTransferOrder = _crudRepository.GetQuery<outboundtransferorder>(x => x.SysId == dto.ToOutboundTransferOrderSysId).FirstOrDefault();
                if (outboundTransferOrder == null)
                {
                    throw new Exception("要移入的交接单不存在");
                }
                if (outboundTransferOrder.Status != (int)OutboundTransferOrderStatus.New && outboundTransferOrder.Status != (int)OutboundTransferOrderStatus.PrePack)
                {
                    throw new Exception("要移入的交接单状态不为新增或者进行中");
                }

                var outboundTransferOrderDetail = _crudRepository.GetQuery<outboundtransferorderdetail>(x => x.OutboundTransferOrderSysId == outboundTransferOrder.SysId && x.SkuSysId == dto.SkuSysId).FirstOrDefault();

                if (outboundTransferOrderDetail == null)
                {
                    var newTransferOrderDetail = new outboundtransferorderdetail()
                    {
                        OutboundTransferOrderSysId = outboundTransferOrder.SysId,
                        SkuSysId = transferOrderDetail.SkuSysId,
                        UOMSysId = transferOrderDetail.UOMSysId,
                        PackSysId = transferOrderDetail.PackSysId,
                        Loc = transferOrderDetail.Loc,
                        Lot = transferOrderDetail.Lot,
                        Lpn = transferOrderDetail.Lpn,
                        ExternalLot = transferOrderDetail.ExternalLot,
                        ProduceDate = transferOrderDetail.ProduceDate,
                        ExpiryDate = transferOrderDetail.ExpiryDate,
                        Qty = dto.MoveSkuQty,
                        CreateBy = dto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        CreateUserName = dto.CurrentDisplayName,
                        UpdateBy = dto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = dto.CurrentDisplayName

                    };
                    _crudRepository.Insert<outboundtransferorderdetail>(newTransferOrderDetail);

                    if (outboundTransferOrder.Status == (int)OutboundTransferOrderStatus.New)
                    {
                        outboundTransferOrder.Status = (int)OutboundTransferOrderStatus.PrePack;
                        outboundTransferOrder.TransferType = (int)OutboundTransferOrderType.Scattered;
                    }
                }
                else
                {
                    outboundTransferOrderDetail.Qty = outboundTransferOrderDetail.Qty + dto.MoveSkuQty;
                    outboundTransferOrderDetail.UpdateBy = dto.CurrentUserId;
                    outboundTransferOrderDetail.UpdateDate = DateTime.Now;
                    outboundTransferOrderDetail.UpdateUserName = dto.CurrentDisplayName;
                    _crudRepository.Update<outboundtransferorderdetail>(outboundTransferOrderDetail);
                }

                outboundTransferOrder.UpdateBy = dto.CurrentUserId;
                outboundTransferOrder.UpdateDate = DateTime.Now;
                outboundTransferOrder.UpdateUserName = dto.CurrentDisplayName;

                outboundTransferOrder.ReviewBy = dto.CurrentUserId;
                outboundTransferOrder.ReviewDate = DateTime.Now;
                outboundTransferOrder.ReviewUserName = dto.CurrentDisplayName;

                _crudRepository.Update<outboundtransferorder>(outboundTransferOrder);
                #endregion 
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}

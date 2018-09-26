using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBK.ECService.WMS.Application
{
    public class TransferInventoryAppService : WMSApplicationService, ITransferInventoryAppService
    {
        private ICrudRepository _crudRepository = null;
        private ITransferInventoryRepository _transferInventoryRepository = null;
        private IPackageAppService _packageAppService = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IBaseAppService _baseAppService = null;

        public TransferInventoryAppService(ICrudRepository crudRepository, ITransferInventoryRepository transferInventoryRepository, IPackageAppService packageAppService, IThirdPartyAppService thirdPartyAppService, IBaseAppService baseAppService)
        {
            _crudRepository = crudRepository;
            _transferInventoryRepository = transferInventoryRepository;
            _thirdPartyAppService = thirdPartyAppService;
            _packageAppService = packageAppService;
            this._baseAppService = baseAppService;
        }

        #region 移仓单创建出库单
        /// <summary>
        /// 移仓单创建出库单
        /// </summary>
        /// <param name="transferInventoryDto"></param>
        /// <returns></returns>
        public CommonResponse AddOutboundByTransferInventory(MQTransferInventoryDto transferInventoryDto)
        {
            _crudRepository.ChangeDB(transferInventoryDto.WarehouseSysId);
            var response = new CommonResponse() { IsSuccess = false };
            try
            {
                if (transferInventoryDto != null)
                {
                    var transferInventory = _crudRepository.GetQuery<transferinventory>(x => x.TransferInventoryOrder == transferInventoryDto.TransferInventoryOrder).FirstOrDefault();
                    if (transferInventory != null)
                    {
                        var warehouse = _crudRepository.Get<warehouse>(transferInventory.ToWareHouseSysId);
                        if (warehouse == null)
                        {
                            throw new Exception("仓库不存在");
                        }

                        var transferInventoryDetails = _crudRepository.GetQuery<transferinventorydetail>(x => x.TransferInventorySysId == transferInventory.SysId).ToList();

                        var outbound = new outbound()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = transferInventory.FromWareHouseSysId,
                            //OutboundOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberOutbound),
                            OutboundOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberOutbound),
                            OutboundType = (int)OutboundType.TransferInventory,
                            Status = (int)OutboundStatus.New,
                            OutboundDate = DateTime.Now,
                            Remark = transferInventory.Remark,
                            ConsigneeName = warehouse.Contacts,
                            ConsigneeAddress = warehouse.Address,
                            ConsigneePhone = warehouse.Telephone,
                            TotalShippedQty = 0,
                            TotalAllocatedQty = 0,
                            TotalPickedQty = 0,
                            TotalQty = transferInventoryDetails.Sum(x => x.Qty),
                            TotalPrice = 0,
                            ExternOrderDate = transferInventory.ExternOrderDate,
                            ExternOrderId = transferInventory.TransferInventoryOrder,
                            ShippingMethod = transferInventory.ShippingMethod,
                            Freight = transferInventory.Freight,
                            Source = PublicConst.ThirdPartySourceERP,
                            AuditingBy = transferInventory.AuditingBy,
                            AuditingDate = transferInventory.AuditingDate,
                            AuditingName = transferInventory.AuditingName,
                            CreateBy = transferInventory.CreateBy,
                            CreateDate = DateTime.Now,
                            CreateUserName = transferInventory.CreateUserName,
                            UpdateBy = transferInventory.UpdateBy,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = transferInventory.UpdateUserName
                        };
                        _crudRepository.Insert(outbound);

                        transferInventory.TransferOutboundSysId = outbound.SysId;
                        transferInventory.TransferOutboundOrder = outbound.OutboundOrder;
                        transferInventory.UpdateBy = transferInventory.UpdateBy;
                        transferInventory.UpdateDate = DateTime.Now;
                        transferInventory.UpdateUserName = transferInventory.UpdateUserName;
                        _crudRepository.Update(transferInventory);

                        foreach (var detail in transferInventoryDetails)
                        {
                            var outboundDetail = new outbounddetail()
                            {
                                SysId = Guid.NewGuid(),
                                OutboundSysId = outbound.SysId,
                                SkuSysId = detail.SkuSysId,
                                Status = (int)OutboundDetailStatus.New,
                                UOMSysId = detail.UOMSysId,
                                PackSysId = detail.PackSysId,
                                Loc = detail.Loc,
                                Lot = detail.Lot,
                                Lpn = detail.Lpn,
                                LotAttr01 = detail.LotAttr01,
                                LotAttr02 = detail.LotAttr02,
                                LotAttr03 = detail.LotAttr03,
                                LotAttr04 = detail.LotAttr04,
                                LotAttr05 = detail.LotAttr05,
                                LotAttr06 = detail.LotAttr06,
                                LotAttr07 = detail.LotAttr07,
                                LotAttr08 = detail.LotAttr08,
                                LotAttr09 = detail.LotAttr09,
                                ExternalLot = detail.ExternalLot,
                                ProduceDate = detail.ProduceDate,
                                ExpiryDate = detail.ExpiryDate,
                                Qty = detail.Qty,
                                ShippedQty = 0,
                                AllocatedQty = 0,
                                PickedQty = 0,
                                Price = 0,
                                PackFactor = detail.PackFactor,
                                CreateBy = detail.CreateBy,
                                CreateDate = DateTime.Now,
                                CreateUserName = detail.CreateUserName,
                                UpdateBy = detail.UpdateBy,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = detail.UpdateUserName
                            };
                            _crudRepository.Insert(outboundDetail);
                        }
                    }
                    else
                    {
                        throw new Exception("移仓单不存在");
                    }
                }
                else
                {
                    throw new Exception("输入参数为空,调用失败");
                }
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            return response;
        }
        #endregion

        /// <summary>
        /// 移仓单创建入库单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommonResponse CreateTransferInventoryReceipt(MQTransferInventoryDto transferInv)
        {
            _crudRepository.ChangeDB(transferInv.ToWareHouseSysId);
            var response = new CommonResponse();
            try
            {
                //var transferInv = _crudRepository.GetQuery<transferinventory>(p => p.TransferInventoryOrder == transferInventoryDto.TransferInventoryOrder).FirstOrDefault();
                //var outbound = _crudRepository.GetQuery<outbound>(p => p.SysId == transferInv.TransferOutboundSysId).FirstOrDefault();
                //if (outbound.Status != (int)OutboundStatus.Delivery)
                //{
                //    throw new Exception("出库单未完成");
                //}

                var purchase = new purchase();
                purchase.SysId = transferInv.PurchaseSysId;
                //purchase.PurchaseOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPurchase);
                //purchase.PurchaseOrder= _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase);
                purchase.PurchaseOrder = transferInv.PurchaseOrder;
                purchase.DeliveryDate = DateTime.Now;
                purchase.ExternalOrder = transferInv.TransferInventoryOrder;
                purchase.VendorSysId = _crudRepository.GetQuery<vendor>(p => p.VendorName == null).FirstOrDefault().SysId;
                purchase.Descr = "移仓单";
                purchase.PurchaseDate = transferInv.TransferOutboundDate;
                purchase.AuditingDate = transferInv.AuditingDate;
                purchase.AuditingBy = transferInv.AuditingBy;
                purchase.AuditingName = transferInv.AuditingName;
                purchase.Status = (int)PurchaseStatus.New;
                purchase.Type = (int)PurchaseType.TransferInventory;
                purchase.Source = PublicConst.ThirdPartySourceERP;
                purchase.WarehouseSysId = transferInv.ToWareHouseSysId;
                purchase.CreateBy = transferInv.CurrentUserId;
                purchase.CreateDate = DateTime.Now;
                purchase.CreateUserName = transferInv.CurrentDisplayName;
                purchase.UpdateBy = transferInv.CurrentUserId;
                purchase.UpdateDate = DateTime.Now;
                purchase.UpdateUserName = transferInv.CurrentDisplayName;

                //移仓入库单增加渠道
                purchase.Channel = transferInv.Channel;

                if (transferInv.FromWareHouseSysId != new Guid())
                {
                    purchase.FromWareHouseSysId = transferInv.FromWareHouseSysId;
                }
                List<purchasedetail> purchaseDetails = new List<purchasedetail>();
                if (transferInv.transferinventorydetails.Any())
                {
                    var skuSysIds = transferInv.transferinventorydetails.Select(x => x.SkuSysId);
                    var skus = _crudRepository.GetQuery<sku>(p => skuSysIds.Contains(p.SysId)).ToList();
                    var skuClassSysIds = skus.Select(x => x.SkuClassSysId);
                    var skuClasses = _crudRepository.GetQuery<skuclass>(p => skuClassSysIds.Contains(p.SysId)).ToList();
                    var packSysIds = transferInv.transferinventorydetails.Select(x => x.PackSysId);
                    var packs = _crudRepository.GetQuery<pack>(p => packSysIds.Contains(p.SysId)).ToList();
                    var uomSysIds = transferInv.transferinventorydetails.Select(x => x.UOMSysId);
                    var uoms = _crudRepository.GetQuery<uom>(p => uomSysIds.Contains(p.SysId)).ToList();
                    foreach (var item in transferInv.transferinventorydetails)
                    {
                        sku sku = skus.FirstOrDefault(p => p.SysId == item.SkuSysId);
                        skuclass skuClass = skuClasses.FirstOrDefault(p => p.SysId == sku.SkuClassSysId);
                        pack pack = packs.FirstOrDefault(p => p.SysId == item.PackSysId);
                        uom uom = uoms.FirstOrDefault(p => p.SysId == item.UOMSysId);
                        purchasedetail purchaseDetail = new purchasedetail
                        {
                            SysId = Guid.NewGuid(),
                            PurchaseSysId = purchase.SysId,
                            SkuSysId = (Guid)item.SkuSysId,
                            SkuClassSysId = skuClass == null ? new Guid?() : skuClass.SysId,
                            UomCode = uom == null ? string.Empty : uom.UOMCode,
                            UOMSysId = item.UOMSysId.Value,
                            PackSysId = item.PackSysId,
                            PackCode = pack == null ? string.Empty : pack.PackCode,
                            Qty = item.Qty.Value,
                            ReceivedQty = 0,
                            RejectedQty = 0,
                            Remark = item.Remark,
                            OtherSkuId = sku.OtherId,
                            PackFactor = item.PackFactor,
                            UpdateBy = transferInv.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = transferInv.CurrentDisplayName
                        };
                        purchaseDetails.Add(purchaseDetail);
                    }
                }

                _crudRepository.Insert(purchase);
                _crudRepository.BatchInsert(purchaseDetails);

                if (transferInv.Transferinventoryreceiptextends.Any())
                {
                    transferInv.Transferinventoryreceiptextends.ForEach(p =>
                    {
                        p.PurchaseSysId = transferInv.PurchaseSysId;
                        p.WarehouseSysId = transferInv.ToWareHouseSysId;
                    });

                    //移仓单生成对应入库仓的批次信息扩展记录表
                    _transferInventoryRepository.BatchInsertTransferinventoryReceiptExtend(transferInv.Transferinventoryreceiptextends);
                }


                //transferInv.TransferPurchaseSysId = purchase.SysId;
                //transferInv.TransferPurchaseOrder = purchase.PurchaseOrder;
                //transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                //transferInv.UpdateDate = DateTime.Now;
                //transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                //_crudRepository.Update(transferInv);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                throw new Exception(ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 更新调拨单状态
        /// </summary>
        /// <returns></returns>
        public CommonResponse UpdateTransferInventoryStatus(MQTransferInventoryDto transferInventoryDto)
        {
            _crudRepository.ChangeDB(transferInventoryDto.FromWareHouseSysId);
            var commonResponse = new CommonResponse();

            var transferInv = _crudRepository.GetQuery<transferinventory>(p => p.TransferInventoryOrder == transferInventoryDto.TransferInventoryOrder).FirstOrDefault();
            if (transferInventoryDto.Status == (int)TransferInventoryStatus.Delivery)
            {
                transferInv.TransferOutboundDate = DateTime.Now;
                transferInv.Status = (int)TransferInventoryStatus.Delivery;
                transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                transferInv.UpdateDate = DateTime.Now;
                _crudRepository.Update(transferInv);

                //调用ECC接口
                commonResponse = _thirdPartyAppService.WriteBackTransferInventoryByOutbound(transferInv);
            }
            else if (transferInventoryDto.Status == (int)TransferInventoryStatus.PartReceipt || transferInventoryDto.Status == (int)TransferInventoryStatus.ReceiptFinish)
            {
                //入库单部分收货和收货完成都需要调用ECC接口
                transferInv.TransferInboundDate = DateTime.Now;
                transferInv.Status = transferInventoryDto.Status;
                transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                transferInv.UpdateDate = DateTime.Now;
                _crudRepository.Update(transferInv);

                //调用ECC接口
                commonResponse = _thirdPartyAppService.WriteBackTransferInventoryByInbound(transferInv, transferInventoryDto.PurchaseDetailViewDto);
            }
            return commonResponse;
        }

        #region 分页获取移仓单
        public Pages<TransferinventoryListDto> GetTransferinventoryByPage(TransferinventoryQuery transferinventoryQuery)
        {
            _crudRepository.ChangeDB(transferinventoryQuery.WarehouseSysId);
            try
            {
                return _transferInventoryRepository.GetTransferinventoryByPage(transferinventoryQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion 分页获取移仓单

        #region 获取移仓单
        public TransferInventoryViewDto GetTransferinventoryBySysId(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            try
            {
                var tf = _transferInventoryRepository.GetTransferinventoryBySysId(sysId);
                TransferInventoryViewDto response = tf.JTransformTo<TransferInventoryViewDto>();
                response.TransferInventoryDetailDto = _transferInventoryRepository.GetTransferInventoryDetail(sysId);

                //单位转换
                //if (response.TransferInventoryDetailDto != null && response.TransferInventoryDetailDto.Count > 0)
                //{
                //    foreach (var item in response.TransferInventoryDetailDto)
                //    {
                //        if(item.SkuSysId == null)
                //        {
                //            continue;
                //        }
                //        decimal transQty = 0m;
                //        uom uom = new uom();
                //        if (_packageAppService.GetSkuDeconversiontransQty((Guid)item.SkuSysId, (int)item.Qty, out transQty, ref uom) == true)
                //        {
                //            item.DisplayQty = transQty;
                //            item.UomCode = uom.UOMCode;
                //        }
                //        else
                //        {
                //            item.DisplayQty = item.Qty;
                //        }

                //        transQty = 0m;
                //        if (item.ShippedQty != null && item.ShippedQty != 0 && _packageAppService.GetSkuDeconversiontransQty((Guid)item.SkuSysId, (int)item.ShippedQty, out transQty) == true)
                //        {
                //            item.DisplayShippedQty = transQty;
                //        }
                //        else
                //        {
                //            item.DisplayShippedQty = item.ShippedQty;
                //        }

                //        transQty = 0m;
                //        if (item.ReceivedQty != null && item.ReceivedQty != 0 && _packageAppService.GetSkuDeconversiontransQty((Guid)item.SkuSysId, (int)item.ReceivedQty, out transQty) == true)
                //        {
                //            item.DisplayReceivedQty = transQty;
                //        }
                //        else
                //        {
                //            item.DisplayReceivedQty = item.ReceivedQty;
                //        }

                //        transQty = 0m;
                //        if (item.RejectedQty != null && item.RejectedQty != 0 && _packageAppService.GetSkuDeconversiontransQty((Guid)item.SkuSysId, (int)item.RejectedQty, out transQty) == true)
                //        {
                //            item.DisplayRejectedQty = transQty;
                //        }
                //        else
                //        {
                //            item.DisplayRejectedQty = item.RejectedQty;
                //        }
                //    }
                //}
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public bool ObsoleteTransferinventory(TransferinventoryUpdateQuery dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var result = false;
            try
            {
                var transf = _crudRepository.Get<transferinventory>(dto.SysId);
                if (transf == null)
                {
                    throw new Exception("移仓单信息获取异常");
                }
                if (transf.Status != (int)TransferInventoryStatus.New)
                {
                    throw new Exception("移仓单不是新建状态不能作废");
                }
                transf.Status = (int)TransferInventoryStatus.Cancel;
                transf.UpdateBy = dto.CurrentUserId;
                transf.UpdateDate = DateTime.Now;
                transf.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Update<transferinventory>(transf);

                var outbound = _crudRepository.Get<outbound>((Guid)transf.TransferOutboundSysId);
                if (outbound == null)
                {
                    throw new Exception("移仓单对应的出库单：" + transf.TransferOutboundOrder + " 获取异常");
                }
                if (outbound.Status != (int)OutboundStatus.New)
                {
                    throw new Exception("移仓单对应的出库单：" + transf.TransferOutboundOrder + " 不是新建，不能作废");
                }
                outbound.Status = (int)OutboundStatus.Cancel;
                outbound.UpdateBy = dto.CurrentUserId;
                outbound.UpdateDate = DateTime.Now;
                outbound.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Update<outbound>(outbound);

                #region 移仓单作废完成通知tms
                var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                {
                    OutboundSysId = outbound.SysId,
                    OutboundOrder = outbound.OutboundOrder,
                    OrderId = transf.ExternOrderId,
                    Status = (int)TMSStatus.Close,
                    UpdateDate = DateTime.Now,
                    EditUserName = dto.CurrentDisplayName,
                    UserId = dto.CurrentUserId,
                    OrderType = (int)TMSOrderType.TransferOrder
                };
                _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
                #endregion

                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}

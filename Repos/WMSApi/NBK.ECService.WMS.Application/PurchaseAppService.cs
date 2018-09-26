using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;

namespace NBK.ECService.WMS.Application
{
    public class PurchaseAppService : WMSApplicationService, IPurchaseAppService
    {
        private IPurchaseRepository _crudRepository = null;
        private IPackageAppService _packageAppService = null;
        private IWMSSqlRepository _WMSSqlRepository = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IBaseAppService _baseAppService = null;

        public PurchaseAppService(IPurchaseRepository crudRepository, IPackageAppService packageAppService, IWMSSqlRepository wmsSqlRepository, IThirdPartyAppService thirdPartyAppService, IBaseAppService baseAppService)
        {
            this._crudRepository = crudRepository;
            _packageAppService = packageAppService;
            this._WMSSqlRepository = wmsSqlRepository;
            this._thirdPartyAppService = thirdPartyAppService;
            this._baseAppService = baseAppService;
        }


        /// <summary>
        /// 根据SysCodeId 获取相关数据
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        public PurchaseViewDto GetPurchaseViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var purchaseViewDto = _crudRepository.GetPurchaseViewDtoBySysId(purchaseSysId);
            purchaseViewDto.PurchaseDetailViewDto = _crudRepository.GetPurchaseDetailViewBySysId(purchaseSysId);
            var receipt = _crudRepository.GetQuery<receipt>(x => x.ExternalOrder == purchaseViewDto.PurchaseOrder).ToList();
            purchaseViewDto.ReceiptPurchaseDto = receipt.JTransformTo<ReceiptPurchaseDto>();

            return purchaseViewDto;
        }

        /// <summary>
        /// 新增订单号
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        public bool InsertPurchase(PurchaseDto purchaseDto)
        {
            _crudRepository.ChangeDB(purchaseDto.WarehouseSysId);
            if (purchaseDto == null || purchaseDto.PurchaseDetailDto == null)
            {
                throw new Exception("传入采购订单无效,缺失主要数据源");
            }
            var checkePurchase = _crudRepository.GetQuery<purchase>(x => x.ExternalOrder == purchaseDto.PurchaseOrder);
            if (checkePurchase.Any())
            {
                throw new Exception("订单号：" + purchaseDto.PurchaseOrder + "已经存在,无法重复推送");
            }

            var purchase = purchaseDto.TransformTo<purchase>();
            purchase.SysId = Guid.NewGuid();
            purchase.PurchaseOrder = "订单号生成";
            try
            {
                _crudRepository.Insert(purchase);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public bool BatchInsertPurchase(List<PurchaseDto> purchaseDtos, Guid warehouseSysId)
        {
            //_crudRepository.ChangeDB(warehouseSysId);
            if (purchaseDtos.Any())
            {
                try
                {
                    purchaseDtos.ForEach(item =>
                    {
                        InsertPurchase(item);
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("传入采购订单无效,缺失主要数据源");
            }
        }

        /// <summary>
        /// 取消采购订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CancelPurchase(string orderId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            if (!string.IsNullOrEmpty(orderId))
            {
                var purchaseList = _crudRepository.GetQuery<purchase>(x => x.ExternalOrder == orderId);
                if (purchaseList.Any())
                {
                    var purchase = purchaseList.FirstOrDefault();
                    if (purchase.Status == (int)PurchaseStatus.New)
                    {
                        purchase.Status = (int)PurchaseStatus.Void;

                    }
                    else
                    {
                        purchase.Status = (int)PurchaseStatus.StopReceipt;
                    }
                    purchase.UpdateDate = DateTime.Now;
                    _crudRepository.Update(purchase);
                    return true;
                }
                else
                {
                    throw new Exception("无法找到对应的采购订单！");
                }
            }
            else
            {
                throw new Exception("请传入有效单据号！");
            }

        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery)
        {
            _crudRepository.ChangeDB(purchaseQuery.WarehouseSysId);
            return _crudRepository.GetPurchaseDtoListByPageInfo(purchaseQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<PurchaseDetailSkuDto> GetPurchaseDetailSkuByUpcIsNull(Guid purchaseSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetPurchaseDetailSkuByUpcIsNull(purchaseSysId);
        }

        /// <summary>
        /// 更新相关 SKU 属性
        /// </summary>
        /// <param name="purchaseDetailSkuDto"></param>
        /// <returns></returns>
        public void SavePurchaseDetailSkuStyle(PurchaseDetailSkuDto purchaseDetailSkuDto)
        {
            _crudRepository.ChangeDB(purchaseDetailSkuDto.WarehouseSysId);
            //验证UPC是否重复
            var oldSku = _crudRepository.GetQuery<sku>(x => x.UPC == purchaseDetailSkuDto.UPC).FirstOrDefault();
            if (oldSku != null)
            {
                throw new Exception("UPC已经存在");
            }
            var sku = _crudRepository.Get<sku>(purchaseDetailSkuDto.SkuSysId.Value);
            sku.Length = purchaseDetailSkuDto.Length;
            sku.Width = purchaseDetailSkuDto.Width;
            sku.Height = purchaseDetailSkuDto.Height;
            sku.Cube = purchaseDetailSkuDto.Length * purchaseDetailSkuDto.Width * purchaseDetailSkuDto.Height;
            sku.DaysToExpire = purchaseDetailSkuDto.DaysToExpire;
            sku.UPC = purchaseDetailSkuDto.UPC;
            sku.NetWeight = purchaseDetailSkuDto.NetWeight;
            _crudRepository.Update(sku);
        }

        /// <summary>
        /// 生成采购单并收货
        /// </summary>
        /// <param name="purchaseBatchDto"></param>
        /// <returns></returns>
        public bool SaveBatchPurchaseAndReceipt(PurchaseBatchDto purchaseBatchDto)
        {
            _crudRepository.ChangeDB(purchaseBatchDto.WarehouseSysId);
            try
            {
                if (purchaseBatchDto != null)
                {
                    var vendor = _crudRepository.Get<vendor>(purchaseBatchDto.VendorSysId);
                    if (vendor == null)
                    {
                        throw new Exception("供应商不存在");
                    }

                    #region 组织采购单
                    var purchaseModel = new purchase()
                    {
                        SysId = Guid.NewGuid(),
                        //PurchaseOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberPurchase),
                        PurchaseOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase),
                        VendorSysId = purchaseBatchDto.VendorSysId,
                        PurchaseDate = purchaseBatchDto.PurchaseDate,
                        LastReceiptDate = DateTime.Now,
                        CreateBy = purchaseBatchDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        Status = (int)PurchaseStatus.Finish,
                        Type = (int)PurchaseType.FIFO,
                        Source = "WMS",
                        PoGroup = purchaseBatchDto.PoGroup,
                        WarehouseSysId = purchaseBatchDto.WarehouseSysId,
                        UpdateBy = purchaseBatchDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        CreateUserName = purchaseBatchDto.CurrentDisplayName,
                        UpdateUserName = purchaseBatchDto.CurrentDisplayName,
                        AuditingDate = DateTime.Now
                    };

                    _crudRepository.Insert(purchaseModel);
                    #endregion

                    #region 组织采购明细
                    var purchaseDetailList = new List<purchasedetail>();

                    if (purchaseBatchDto.PurchaseDetailBatchDto != null && purchaseBatchDto.PurchaseDetailBatchDto.Count > 0)
                    {
                        var skus = (from p in purchaseBatchDto.PurchaseDetailBatchDto group p by p.SkuSysId into g select g.Key).ToList();

                        var skuList = _crudRepository.GetQuery<sku>(x => skus.Contains(x.SysId)).ToList();
                        var packList = _crudRepository.GetAllList<pack>().ToList();
                        var uomList = _crudRepository.GetAllList<uom>().ToList();

                        foreach (var detail in purchaseBatchDto.PurchaseDetailBatchDto)
                        {
                            var detailSku = skuList.Find(x => x.SysId == detail.SkuSysId);

                            pack detailPack = null;
                            if (detailSku != null)
                            {
                                detailPack = packList.Find(x => x.SysId == detailSku.PackSysId);
                            }
                            if (detailPack == null)
                            {
                                throw new Exception("商品:" + detailSku.SkuName + ",包装代码不存在");
                            }

                            uom detailUom = null;
                            if (detailPack != null)
                            {
                                detailUom = uomList.Find(x => x.SysId == Guid.Parse(detailPack.FieldUom01.ToString()));
                            }
                            if (detailUom == null)
                            {
                                throw new Exception("包装:" + detailPack.PackCode + ",单位不存在");
                            }

                            var purchaseDetailModel = new purchasedetail()
                            {
                                SysId = Guid.NewGuid(),
                                PurchaseSysId = purchaseModel.SysId,
                                SkuSysId = detail.SkuSysId,
                                UomCode = detailUom.UOMCode,
                                UOMSysId = detailUom.SysId,
                                PackSysId = detailPack.SysId,
                                PackCode = detailPack.PackCode,
                                Qty = detail.Qty,
                                ReceivedQty = detail.Qty,
                                RejectedQty = 0,
                                LastPrice = detailSku.CostPrice
                            };
                            purchaseDetailList.Add(purchaseDetailModel);
                        }
                        _crudRepository.BatchInsert(purchaseDetailList);
                    }
                    #endregion

                    #region 组织收货单
                    var expectedQty = purchaseDetailList.Sum(x => x.Qty);
                    var receiptModel = new receipt()
                    {
                        SysId = Guid.NewGuid(),
                        //ReceiptOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberReceipt),
                        ReceiptOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberReceipt),
                        ExternalOrder = purchaseModel.PurchaseOrder,
                        ReceiptType = (int)ReceiptType.FIFO,
                        WarehouseSysId = purchaseBatchDto.WarehouseSysId,
                        VendorSysId = purchaseModel.VendorSysId,
                        ReceiptDate = DateTime.Now,
                        Status = (int)ReceiptStatus.Received,
                        CreateBy = purchaseBatchDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = purchaseBatchDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        IsActive = true,
                        TotalExpectedQty = expectedQty,
                        TotalReceivedQty = expectedQty,
                        TotalRejectedQty = 0,
                        CreateUserName = purchaseBatchDto.CurrentDisplayName,
                        UpdateUserName = purchaseBatchDto.CurrentDisplayName
                    };
                    _crudRepository.Insert(receiptModel);
                    #endregion

                    #region 组织收货单明细
                    if (purchaseDetailList != null && purchaseDetailList.Count > 0)
                    {
                        var receiptDetailList = new List<receiptdetail>();
                        //var lots = _crudRepository.BatchGenNextNumber(PublicConst.GenNextNumberBatchLot, purchaseDetailList.Count);
                        var lots = _baseAppService.GetNumber(PublicConst.GenNextNumberBatchLot, purchaseDetailList.Count);
                        for (var i = 0; i < purchaseDetailList.Count; i++)
                        {
                            var detail = purchaseDetailList[i];
                            var receiptDetailModel = new receiptdetail()
                            {
                                SysId = Guid.NewGuid(),
                                ReceiptSysId = receiptModel.SysId,
                                SkuSysId = detail.SkuSysId,
                                Status = (int)ReceiptDetailStatus.Received,
                                ExpectedQty = detail.Qty,
                                ReceivedQty = detail.ReceivedQty,
                                RejectedQty = 0,
                                CreateBy = purchaseBatchDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                UpdateBy = purchaseBatchDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UOMSysId = detail.UOMSysId,
                                PackSysId = detail.PackSysId,
                                ToLoc = PublicConst.FIFOLoc,
                                ToLot = lots[i],
                                ToLpn = "",
                                ReceivedDate = DateTime.Now,
                                Price = detail.LastPrice,
                                ShelvesQty = 0,
                                ShelvesStatus = (int)ShelvesStatus.NotOnShelves,
                                CreateUserName = purchaseBatchDto.CurrentDisplayName,
                                UpdateUserName = purchaseBatchDto.CurrentDisplayName
                            };
                            receiptDetailList.Add(receiptDetailModel);
                        }
                        _crudRepository.BatchInsert(receiptDetailList);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 指定入库批号
        /// </summary>
        /// <param name="sysId"></param>
        public void AppointBatchNumber(List<Guid> sysId, string batchNumber, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            _crudRepository.UpdatePurchaseBatchNumberBySysId(sysId, batchNumber);
        }

        /// <summary>
        /// 作废采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        public bool ObsoletePurchase(PurchaseOperateDto purchaseDto)
        {
            _crudRepository.ChangeDB(purchaseDto.WarehouseSysId);
            var result = false;
            try
            {
                var purchase = _crudRepository.Get<purchase>(purchaseDto.SysId);

                if (purchase == null)
                {
                    throw new Exception("采购单不存在");
                }

                if (purchase.Status != (int)PurchaseStatus.New)
                {
                    throw new Exception("只能作废状态为新建的采购单");
                }

                purchase.Status = (int)PurchaseStatus.Void;
                purchase.UpdateBy = purchaseDto.CurrentUserId;
                purchase.UpdateUserName = purchaseDto.CurrentDisplayName;
                purchase.UpdateDate = DateTime.Now;

                if (purchase.FromWareHouseSysId.HasValue && purchase.OutboundSysId.HasValue)
                {
                    PurchaseForReturnDto purchaseReturn = purchase.JTransformTo<PurchaseForReturnDto>();
                    purchaseReturn.UpdateBy = purchaseDto.CurrentUserId;
                    purchaseReturn.UpdateUserName = purchaseDto.CurrentDisplayName;
                    var response = ApiClient.Post(PublicConst.WmsApiUrl, "/Outbound/CancelOutboundReturnByPurchase", new CoreQuery(), purchaseReturn);

                    if (!response.Success)
                    {
                        throw new Exception($"作废入库单失败(CancelOutboundReturnByPurchase):{response.ApiMessage.ErrorMessage}");
                    }
                }

                _crudRepository.Update(purchase);
                                
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 关闭采购订单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        public bool ClosePurchase(PurchaseOperateDto purchaseDto)
        {
            _crudRepository.ChangeDB(purchaseDto.WarehouseSysId);
            var result = false;
            try
            {
                var purchase = _crudRepository.Get<purchase>(purchaseDto.SysId);

                if (purchase == null)
                {
                    throw new Exception("采购单不存在");
                }

                if (purchase.Status == (int)PurchaseStatus.Finish)
                {
                    throw new Exception("已入库的采购单不能关闭");
                }

                purchase.Status = (int)PurchaseStatus.Close;
                purchase.UpdateBy = purchaseDto.CurrentUserId;
                purchase.UpdateUserName = purchaseDto.CurrentDisplayName;
                purchase.UpdateDate = DateTime.Now;
                _crudRepository.Update(purchase);

                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 自动收货上架
        /// </summary>
        /// <param name="purchaseDto"></param>
        public void AutoShelves(PurchaseOperateDto purchaseDto)
        {
            _crudRepository.ChangeDB(purchaseDto.WarehouseSysId);
            var purchase = _crudRepository.Get<purchase>(purchaseDto.SysId);

            if (purchase == null)
            {
                throw new Exception("入库单不存在");
            }

            if (purchase.Status != (int)PurchaseStatus.New)
            {
                throw new Exception("只有待入库的入库单可以自动收货上架");
            }

            if (!(purchase.WarehouseSysId == purchase.FromWareHouseSysId))
            {
                throw new Exception("只有当前仓库出库单产生的退货入库单可以自动收货上架");
            }

            var existsReceipt = _crudRepository.GetQuery<receipt>(p => p.ExternalOrder == purchase.PurchaseOrder && p.Status != (int)ReceiptStatus.Cancel);
            if (existsReceipt.Count() > 0)
            {
                throw new Exception("该入库单已存在有效收货记录，不能自动收货上架");
            }

            if (purchase.Type != (int)PurchaseType.Return)
            {
                throw new Exception("只有退货类型的入库单可以自动收货上架");
            }

            if (purchase.purchasedetails == null || purchase.purchasedetails.Count() == 0)
            {
                throw new Exception("入库单明细信息不存在");
            }

            purchase.Status = (int)PurchaseStatus.Finish;
            purchase.UpdateBy = purchaseDto.CurrentUserId;
            purchase.UpdateDate = DateTime.Now;
            purchase.UpdateUserName = purchaseDto.CurrentDisplayName;
            purchase.LastReceiptDate = DateTime.Now;
            var newPurchaseDetail = new List<PurchaseDetailDto>();
            purchase.purchasedetails.ToList().ForEach(p =>
            {
                p.ReceivedQty = p.Qty;

                newPurchaseDetail.Add(new PurchaseDetailDto()
                {
                    OtherSkuId = p.OtherSkuId,
                    ReceivedQty = p.ReceivedQty,
                    RejectedQty = p.RejectedQty,
                    GiftQty = 0,
                    RejectedGiftQty = 0,
                    Remark = p.Remark
                });
            });

            #region 创建收货单

            var historyReceipt = _crudRepository.GetQuery<receipt>(p => p.ExternalOrder == purchase.PurchaseOrder);
            int orderCount = 0;
            if (historyReceipt != null && historyReceipt.Count() > 0)
            {
                orderCount = historyReceipt.Count();
            }

            var receipt = new receipt();
            receipt.SysId = Guid.NewGuid();
            receipt.ReceiptOrder = $"{purchase.PurchaseOrder}-{orderCount + 1}";
            receipt.ExternalOrder = purchase.PurchaseOrder;
            receipt.ReceiptType = (int)ReceiptType.Return;
            receipt.ReceiptDate = DateTime.Now;
            receipt.WarehouseSysId = purchase.WarehouseSysId.Value;
            receipt.ExpectedReceiptDate = purchase.DeliveryDate;
            receipt.ArrivalDate = DateTime.Now;
            receipt.Status = (int)ReceiptStatus.Received;
            receipt.CreateBy = purchaseDto.CurrentUserId;
            receipt.CreateUserName = purchaseDto.CurrentDisplayName;
            receipt.UpdateUserName = purchaseDto.CurrentDisplayName;
            receipt.CreateDate = DateTime.Now;
            receipt.UpdateBy = purchaseDto.CurrentUserId;
            receipt.UpdateDate = DateTime.Now;
            receipt.VendorSysId = purchase.VendorSysId;
            receipt.TotalReceivedQty = purchase.purchasedetails.Sum(p => p.ReceivedQty);
            receipt.TotalExpectedQty = receipt.TotalReceivedQty;
            receipt.TotalRejectedQty = 0;

            if (!purchase.OutboundSysId.HasValue)
            {
                throw new Exception("未找到有效的源出库单信息");
            }
            var outbound = _crudRepository.Get<outbound>(purchase.OutboundSysId.Value);
            if (outbound == null)
            {
                throw new Exception("未找到有效的源出库单信息");
            }

            var pickdetailInfo = _crudRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PickDetailStatus.Finish).ToList();
            if (pickdetailInfo == null || pickdetailInfo.Count == 0)
            {
                throw new Exception("未找到有效的源出库拣货信息");
            }


            //生成收货明细
            var receiptdetailList = new List<ReceiptdetailAutoShelvesDto>();

            if (outbound.IsReturn == (int)OutboundReturnStatus.PartReturn || outbound.IsReturn == (int)OutboundReturnStatus.B2CReturn)
            {//部分退货入库 & B2C退货入库 
                receiptdetailList = AutoShelvesPartReturn(purchaseDto, pickdetailInfo, purchase, outbound, receipt); 
            }
            else if (outbound.IsReturn == (int)OutboundReturnStatus.AllReturn )
            {//全部退货入库 
                receiptdetailList = AutoShelvesAllReturn(purchaseDto, pickdetailInfo, purchase, outbound, receipt);
            }

            #endregion


            var invLotList = _crudRepository.GetInvlotForAutoShelves(outbound.SysId);
            var invSkuLocList = _crudRepository.GetInvskulocForAutoShelves(outbound.SysId);
            var invLotLocLpnList = _crudRepository.GetInvlotloclpnForAutoShelves(outbound.SysId);
            var skuinfoList = _crudRepository.GetSkuForAutoShelves(outbound.SysId);

            var updateInventoryDtos = new List<UpdateInventoryDto>();

            foreach (var rd in receiptdetailList)
            {
                rd.SysId = Guid.NewGuid();
                //入库存数量
                var invQty = rd.ReceivedQty.Value;

                //若是有部分退货入库的出库单商品的数量应该取入库单的数量而不是拣货单的数量
                if (outbound.IsReturn == (int)OutboundReturnStatus.PartReturn)
                {
                    invQty = purchase.purchasedetails.Where(o => o.SkuSysId == rd.SkuSysId).Select(o => o.Qty).FirstOrDefault();
                    rd.ReceivedQty = invQty;
                    rd.ShelvesQty = invQty;
                    rd.ExpectedQty = invQty;
                }

                var invLot = invLotList.Where(x => x.Lot == rd.ToLot && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == purchaseDto.WarehouseSysId).FirstOrDefault();
                //updateInventoryDtos.Add(new UpdateInventoryDto()
                //{
                //    InvLotLocLpnSysId = new Guid(),
                //    InvLotSysId = invLot.SysId,
                //    InvSkuLocSysId = new Guid(),
                //    Qty = invQty,
                //    CurrentUserId = purchaseDto.CurrentUserId,
                //    CurrentDisplayName = purchaseDto.CurrentDisplayName,
                //    WarehouseSysId = purchase.WarehouseSysId.Value,
                //});

                var invSkuLoc = invSkuLocList.Where(x => x.Loc == rd.ToLoc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == purchaseDto.WarehouseSysId).FirstOrDefault();


                var invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == rd.ToLoc && x.Lpn == rd.ToLpn && x.WareHouseSysId == purchaseDto.WarehouseSysId).FirstOrDefault();
                updateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = invLotLocLpn.SysId,
                    InvLotSysId = invLot.SysId,
                    InvSkuLocSysId = invSkuLoc.SysId,
                    Qty = invQty,
                    CurrentUserId = purchaseDto.CurrentUserId,
                    CurrentDisplayName = purchaseDto.CurrentDisplayName,
                    WarehouseSysId = purchase.WarehouseSysId.Value,
                });

                var skuInfo = skuinfoList.FirstOrDefault(p => p.PickDetailSysId == rd.PickDetailSysId);

                #region InvTrans
                var invTrans = new invtran()
                {
                    SysId = Guid.NewGuid(),
                    WareHouseSysId = purchase.WarehouseSysId.Value,
                    DocOrder = receipt.ReceiptOrder,
                    DocSysId = receipt.SysId,
                    DocDetailSysId = rd.SysId.Value,
                    SkuSysId = rd.SkuSysId,
                    SkuCode = skuInfo.SkuCode,
                    TransType = InvTransType.Inbound,
                    SourceTransType = InvSourceTransType.Shelve,
                    Qty = invQty,
                    Loc = rd.ToLoc,
                    Lot = rd.ToLot,
                    Lpn = rd.ToLpn,
                    ToLoc = rd.ToLoc,
                    ToLot = rd.ToLot,
                    ToLpn = rd.ToLpn,
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
                    PackSysId = skuInfo.PackSysId,
                    PackCode = skuInfo.PackCode,
                    UOMSysId = skuInfo.UOMSysId,
                    UOMCode = skuInfo.UOMCode,
                    CreateBy = purchaseDto.CurrentUserId,
                    CreateDate = DateTime.Now,
                    UpdateBy = purchaseDto.CurrentUserId,
                    UpdateDate = DateTime.Now,
                    CreateUserName = purchaseDto.CurrentDisplayName,
                    UpdateUserName = purchaseDto.CurrentDisplayName
                };
                _crudRepository.Insert(invTrans);

                #endregion
            }

            _crudRepository.Insert(receipt);   //收货单
            var receiptdetails = receiptdetailList.JTransformTo<receiptdetail>();
            _crudRepository.BatchInsert(receiptdetails);    //收货明细

            _WMSSqlRepository.UpdateInventoryForCancelOutbound(updateInventoryDtos);

            var crpe = new CommonResponse();
            try
            {
                crpe = _thirdPartyAppService.InsertInStock(purchaseDto.CurrentUserId, purchaseDto.CurrentDisplayName, purchase.SysId, newPurchaseDetail);
                _crudRepository.SetOperationLog(OperationLogType.Write, crpe, "采购单" + purchase.SysId);
            }
            catch (Exception ex)
            {
                _crudRepository.SetOperationLog(OperationLogType.Abnormal, crpe, "采购单" + purchase.SysId + " 异常:" + ex.Message);
            }
        }


        private List<ReceiptdetailAutoShelvesDto> AutoShelvesAllReturn(PurchaseOperateDto purchaseDto, List<pickdetail> pickdetailInfo, purchase purchase, outbound outbound, receipt receipt)
        {
            var locs = pickdetailInfo.Select(q => q.Loc).ToList();
            //校验库存冻结
            var frozenLoc = _crudRepository.GetQuery<location>(p => locs.Contains(p.Loc) && p.WarehouseSysId == purchase.WarehouseSysId.Value && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();
            //货位级别
            if (frozenLoc != null)
            {
                throw new Exception($"货位{frozenLoc.Loc}已被冻结，不能自动收货上架!");
            }
            var skuList = pickdetailInfo.Select(q => q.SkuSysId);
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == purchase.WarehouseSysId.Value);
            //商品级别
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能自动收货上架!");
            }

            //货位商品级别
            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == purchase.WarehouseSysId).ToList();

            if (locskuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in pickdetailInfo
                                        join T2 in locskuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能自动收货上架!");
                }
            }
            return _crudRepository.GetReceiptdetailForAutoShelves(outbound.SysId, receipt.SysId, purchaseDto.CurrentUserId, purchaseDto.CurrentDisplayName);
        }

        private List<ReceiptdetailAutoShelvesDto> AutoShelvesPartReturn(PurchaseOperateDto purchaseDto, List<pickdetail> pickdetailInfo, purchase purchase, outbound outbound, receipt receipt)
        {
            List<ReceiptdetailAutoShelvesDto> receiptdetailList = _crudRepository.GetReceiptdetailForAutoShelves(outbound.SysId, receipt.SysId, purchaseDto.CurrentUserId, purchaseDto.CurrentDisplayName, purchase.purchasedetails.Select(o => o.SkuSysId).ToList());
            var locs = receiptdetailList.Select(q => q.ToLoc).ToList();
            //校验库存冻结
            var frozenLoc = _crudRepository.GetQuery<location>(p => locs.Contains(p.Loc) && p.WarehouseSysId == purchase.WarehouseSysId.Value && p.Status == (int)LocationStatus.Frozen).ToList();
            //货位级别
            if (frozenLoc != null && frozenLoc.Count > 0)
            {
                for (int i = receiptdetailList.Count - 1; i >= 0; i--)
                {
                    if (frozenLoc.Select(o => o.Loc).Contains(receiptdetailList[i].ToLoc))
                    {
                        receiptdetailList.RemoveAt(i);
                    }
                }
                if (receiptdetailList.Count == 0)
                {
                    throw new Exception($"相关货位已被冻结，不能自动收货上架!");
                }
            }
            var skuList = receiptdetailList.Select(q => q.SkuSysId);
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuList.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == purchase.WarehouseSysId.Value);
            //商品级别
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能自动收货上架!");
            }

            //货位商品级别
            var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuList.Contains(p.SkuSysId.Value)
                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == purchase.WarehouseSysId).ToList();

            if (locskuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in receiptdetailList
                                        join T2 in locskuList on new { T1.SkuSysId, Loc = T1.ToLoc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    for (int i = receiptdetailList.Count - 1; i >= 0; i--)
                    {
                        if (locskuFrozenQuery.FirstOrDefault(o => o.Loc == receiptdetailList[i].ToLoc && o.SkuSysId == receiptdetailList[i].SkuSysId) != null)
                        {
                            receiptdetailList.RemoveAt(i);
                        }
                    }
                    if (receiptdetailList.Count == 0)
                    {
                        throw new Exception("相关商品货位已被冻结，不能自动收货上架!");
                    }
                }
            }

            var existSkuSysIds = new List<Guid>();
            if (receiptdetailList != null && receiptdetailList.Count > 0)
            {
                for (int i = receiptdetailList.Count - 1; i >= 0; i--)
                {
                    if (existSkuSysIds.Contains(receiptdetailList[i].SkuSysId))
                    {
                        receiptdetailList.RemoveAt(i);
                    }
                    else
                    {
                        existSkuSysIds.Add(receiptdetailList[i].SkuSysId);
                    }
                }
            }
            return receiptdetailList;
        }

        /// <summary>
        /// 入库单生成质检单
        /// </summary>
        /// <param name="purchaseQcDto"></param>
        /// <returns></returns>
        public bool GenerateQcOrderByPurchase(PurchaseQcDto purchaseQcDto)
        {
            _crudRepository.ChangeDB(purchaseQcDto.WarehouseSysId);
            var result = false;
            try
            {
                if (purchaseQcDto != null)
                {
                    var purchase = _crudRepository.Get<purchase>(purchaseQcDto.PurchaseSysId);
                    if (purchase == null)
                    {
                        throw new Exception("入库单不存在，请检查");
                    }

                    var oldQc = _crudRepository.GetQuery<qualitycontrol>(x => x.DocOrder == purchase.PurchaseOrder).FirstOrDefault();
                    if (oldQc != null)
                    {
                        throw new Exception("质检单已生成");
                    }

                    var qc = new qualitycontrol()
                    {
                        SysId = Guid.NewGuid(),
                        QCOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberQC),
                        WareHouseSysId = purchaseQcDto.WarehouseSysId,
                        Status = (int)QCStatus.New,
                        QCType = (int)QCType.PurchaseQC,
                        DocOrder = purchase.PurchaseOrder,
                        ExternOrderId = purchase.ExternalOrder,
                        CreateBy = purchaseQcDto.CurrentUserId,
                        CreateUserName = purchaseQcDto.CurrentDisplayName,
                        CreateDate = DateTime.Now,
                        UpdateBy = purchaseQcDto.CurrentUserId,
                        UpdateUserName = purchaseQcDto.CurrentDisplayName,
                        UpdateDate = DateTime.Now
                    };
                    _crudRepository.Insert(qc);
                }
                else
                {
                    throw new Exception("传入的参数有误，请检查");
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysId"></param>
        public bool UpdatePurchaseBusinessTypeBySysId(List<Guid> sysId, string businessType, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.UpdatePurchaseBusinessTypeBySysId(sysId, businessType);
        }

        public void InsertPurchaseAndDetailsByReturnOutbound(PurchaseForReturnDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId.Value);
            var purchase = request.JTransformTo<purchase>();
            var purchaseDetails = request.purchasedetails.JTransformTo<purchasedetail>();
            var purchaseExtend = request.PurchaseExtend.JTransformTo<purchaseextend>();
            purchaseExtend.PurchaseSysId = purchase.SysId;
            purchaseExtend.CreateBy = purchase.CreateBy;
            purchaseExtend.CreateDate = purchase.CreateDate;
            purchaseExtend.CreateUserName = purchase.CreateUserName;
            purchaseExtend.UpdateBy = purchase.CreateBy;
            purchaseExtend.UpdateDate = purchase.CreateDate;
            purchaseExtend.UpdateUserName = purchase.CreateUserName;

            _WMSSqlRepository.BatchInsertPurchaseAndDetails(purchase, purchaseDetails);
            _crudRepository.Insert(purchaseExtend);
        }
    }
}
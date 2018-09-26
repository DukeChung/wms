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
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.Utility.RabbitMQ;
using System.Data.Entity.Infrastructure;
using Abp.Domain.Uow;
using System.Transactions;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO.Receipt;

namespace NBK.ECService.WMS.Application
{
    public class ReceiptAppService : WMSApplicationService, IReceiptAppService
    {
        private IReceiptRepository _crudRepository = null;
        private IPurchaseRepository _purchaseRepository = null;
        private IPackageRepository _packageRepository = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IPackageAppService _packageAppService = null;
        private IBaseAppService _baseAppService = null;
        private ITransferInventoryAppService _transferInventoryAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;

        public ReceiptAppService(IReceiptRepository crudRepository, IPurchaseRepository purchaseRepository, IThirdPartyAppService thirdPartyAppService, IPackageAppService packageAppService, IBaseAppService baseAppService, ITransferInventoryAppService transferInventoryAppService, IPackageRepository packageRepository, IWMSSqlRepository wmsSqlRepository)
        {
            this._crudRepository = crudRepository;
            this._purchaseRepository = purchaseRepository;
            this._packageRepository = packageRepository;
            this._thirdPartyAppService = thirdPartyAppService;
            this._packageAppService = packageAppService;
            this._baseAppService = baseAppService;
            this._transferInventoryAppService = transferInventoryAppService;
            this._wmsSqlRepository = wmsSqlRepository;
        }

        /// <summary>
        /// 获取收货信息
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ReceiptOperationDto GetReceiptOperationByOrderNumber(string orderNumber, string currentUserName, int currentUserId, Guid currentWarehouseSysId)
        {
            _crudRepository.ChangeDB(currentWarehouseSysId);
            var receiptList = _crudRepository.GetQuery<receipt>(x => x.ReceiptOrder == orderNumber && x.WarehouseSysId == currentWarehouseSysId).ToList();
            if (!receiptList.Any())
            {
                throw new Exception("无效的入库单号!");
            }

            var receipt = receiptList.FirstOrDefault();

            var purchase = _crudRepository.GetQuery<purchase>(x => x.PurchaseOrder == receipt.ExternalOrder).FirstOrDefault();
            if (purchase != null)
            {
                if (purchase.Status == (int)PurchaseStatus.Void)
                {
                    throw new Exception("采购单已作废，无法收货!");
                }

                if (purchase.Status == (int)PurchaseStatus.Close)
                {
                    throw new Exception("采购单已关闭，无法收货!");
                }
            }
            else
            {
                throw new Exception("采购单不存在!");
            }

            if (receipt.Status == (int)ReceiptStatus.Received)
            {
                throw new Exception("入库单已经收货完成，无法重复收货!");
            }

            if (receipt.Status == (int)ReceiptStatus.Init || receipt.Status == (int)ReceiptStatus.New)
            {
                UpdateReceiptStatus(receipt.SysId, ReceiptStatus.Receiving, currentUserName, currentUserId, currentWarehouseSysId);
            }
            var receiptOperationDto = receipt.JTransformTo<ReceiptOperationDto>();
            if (receipt.VendorSysId.HasValue)
            {
                var vendor = _crudRepository.Get<vendor>(receipt.VendorSysId.Value);
                receiptOperationDto.VendorName = vendor.VendorName;
            }
            if (!string.IsNullOrEmpty(receipt.ExternalOrder))
            {
                var po = _crudRepository.GetQuery<purchase>(x => x.PurchaseOrder == receipt.ExternalOrder).FirstOrDefault();
                receiptOperationDto.PurcharType = po.Type;
                receiptOperationDto.PurchaseSysId = po.SysId;
                receiptOperationDto.PurchaseDescr = po.Descr;
                receiptOperationDto.PurchaseDetailViewDto = _purchaseRepository.GetPurchaseDetailViewBySysId(po.SysId);

                receiptOperationDto.PurchaseDetailSkuDto = _purchaseRepository.GetPurchaseDetailSkuByUpcIsNull(po.SysId);
            }

            receiptOperationDto.ReceiptDetailOperationDto = new List<ReceiptDetailOperationDto>();
            //receiptOperationDto.ReceiptSNDto = _crudRepository.GetQuery<receiptsn>(x => x.ReceiptSysId == receipt.SysId).ToList().JTransformTo<ReceiptSNDto>();

            return receiptOperationDto;
        }

        /// <summary>
        /// 创建收货信息
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ReceiptOperationDto CreateReceiptByPoOrder(string orderNumber, string currentUserName, int currentUserId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var receiptOperationDto = new ReceiptOperationDto();
            try
            {
                var poList = _crudRepository.GetQuery<purchase>(x => x.PurchaseOrder == orderNumber).ToList();
                if (!poList.Any())
                {
                    throw new Exception("无效的采购订单号!");
                }
                var po = poList.FirstOrDefault();
                if (po.Status == (int)PurchaseStatus.Void)
                {
                    throw new Exception("采购订单已经作废,无法进行收货!");
                }
                if (po.Status == (int)PurchaseStatus.Close)
                {
                    throw new Exception("采购订单已经关闭,无法进行收货!");
                }
                if (po.Status == (int)PurchaseStatus.StopReceipt)
                {
                    throw new Exception("采购订单已经终止收货!");
                }
                var receipt = new receipt();
                receipt.SysId = Guid.NewGuid();
                //receipt.ReceiptOrder = _crudRepository.GenNextNumber(PublicConst.GenNextNumberReceipt);

                //生成新的展示业务外部单据号， 业务单号-自增编号
                var existsExternalOrder = _crudRepository.GetQuery<receipt>(p => p.ExternalOrder == po.PurchaseOrder);
                receipt.ReceiptOrder = $"{po.PurchaseOrder}-{existsExternalOrder.Count() + 1}";

                receipt.ExternalOrder = po.PurchaseOrder;
                switch (po.Type)
                {
                    case (int)PurchaseType.Purchase:
                        receipt.ReceiptType = (int)ReceiptType.PO;
                        break;
                    case (int)PurchaseType.Return:
                        receipt.ReceiptType = (int)ReceiptType.Return;
                        break;
                    case (int)PurchaseType.Material:
                        receipt.ReceiptType = (int)ReceiptType.Material;
                        break;
                    case (int)PurchaseType.TransferInventory:
                        receipt.ReceiptType = (int)ReceiptType.TransferInventory;
                        break;
                    case (int)PurchaseType.Fertilizer:
                        receipt.ReceiptType = (int)ReceiptType.Fertilizer;
                        break;
                    default:
                        receipt.ReceiptType = (int)ReceiptType.PO;
                        break;
                }
                //receipt.WarehouseSysId = PublicConst.WareHouseSysId.ToGuid();
                receipt.WarehouseSysId = Guid.Parse(po.WarehouseSysId.ToString());
                receipt.ExpectedReceiptDate = po.DeliveryDate;
                receipt.ArrivalDate = DateTime.Now;
                receipt.Status = (int)ReceiptStatus.Init;
                receipt.CreateBy = currentUserId;
                receipt.CreateUserName = currentUserName;
                receipt.UpdateUserName = currentUserName;
                receipt.CreateDate = DateTime.Now;
                receipt.UpdateBy = currentUserId;
                receipt.UpdateDate = DateTime.Now;
                receipt.VendorSysId = po.VendorSysId;
                receipt.TotalExpectedQty = 0;
                receipt.TotalReceivedQty = 0;
                receipt.TotalRejectedQty = 0;
                _crudRepository.Insert(receipt);
                //_crudRepository.CreateReceipt(receipt);
                receiptOperationDto = receipt.JTransformTo<ReceiptOperationDto>();
                var vendor = _crudRepository.FirstOrDefault<vendor>(x => x.SysId == po.VendorSysId);
                if (vendor != null)
                {
                    receiptOperationDto.VendorName = vendor.VendorName;
                    receiptOperationDto.VendorContacts = vendor.VendorContacts;
                    receiptOperationDto.VendorPhone = vendor.VendorPhone;
                }
                receiptOperationDto.PurchaseDetailViewDto = _purchaseRepository.GetPurchaseDetailViewBySysId(po.SysId);
                //SN已弃用
                //CheckReceiptSnByDetail(receipt.SysId, receiptOperationDto.PurchaseDetailViewDto);

                #region 组织工单数据
                var workRule = _crudRepository.GetQuery<workrule>(x => x.WarehouseSysId == warehouseSysId).FirstOrDefault();
                if (workRule != null && workRule.Status == true && workRule.ReceiptWork == true)
                {
                    var workDetailList = new List<WorkDetailDto>();
                    if (receipt != null)
                    {
                        var workDetail = new WorkDetailDto()
                        {
                            Status = (int)WorkStatus.Working,
                            WorkType = (int)UserWorkType.Receipt,
                            Priority = 1,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now,
                            WorkTime = DateTime.Now,
                            Source = "收货",
                            DocSysId = receipt.SysId,
                            DocOrder = receipt.ReceiptOrder,
                            WarehouseSysId = warehouseSysId,
                            CurrentUserId = currentUserId,
                            CurrentDisplayName = currentUserName
                        };
                        workDetailList.Add(workDetail);
                    }

                    #region 组织推送工单到MQ
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Insert,
                        WorkType = (int)UserWorkType.Receipt,
                        WorkDetailDtoList = workDetailList,
                        WarehouseSysId = warehouseSysId,
                        CurrentUserId = currentUserId,
                        CurrentDisplayName = currentUserName
                    };

                    var processDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = receipt.SysId,
                        BussinessOrderNumber = receipt.ReceiptOrder,
                        Descr = "",
                        CurrentUserId = currentUserId,
                        CurrentDisplayName = currentUserName,
                        WarehouseSysId = warehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, processDto);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                _crudRepository.SetOperationLog(OperationLogType.Abnormal, receiptOperationDto, "创建收货信息异常" + ex.Message);
                throw new Exception(ex.Message);
            }
            return receiptOperationDto;
        }

        /// <summary>
        /// 更新收货状态
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        public void UpdateReceiptStatus(Guid sysId, ReceiptStatus status, string currentUserName, int currentUserId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var receipt = _crudRepository.Get<receipt>(sysId);
            receipt.Status = (int)status;
            if (ReceiptStatus.Print == status)
            {
                //如果状态等于打印,更新到达日期、打印日期
                receipt.ArrivalDate = DateTime.Now;
            }

            receipt.UpdateBy = currentUserId;
            receipt.CreateUserName = currentUserName;
            receipt.UpdateDate = DateTime.Now;
            _crudRepository.Update(receipt);
        }

        /// <summary>
        /// 保存收货单明细信息
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        public string SaveReceiptOperation(ReceiptOperationDto receiptOperationDto)
        {
            _crudRepository.ChangeDB(receiptOperationDto.WarehouseSysId);
            var receipt = _crudRepository.Get<receipt>(receiptOperationDto.SysId);
            if (receipt == null || receipt.Status == (int)ReceiptStatus.Received || receipt.Status == (int)ReceiptStatus.Cancel)
            {
                throw new Exception(string.Format("无法完成收货单，单据状态为{0}", ((ReceiptStatus)receipt.Status).ToDescription()));
            }

            var purchase = _crudRepository.Get<purchase>(receiptOperationDto.PurchaseSysId);
            if (purchase != null)
            {
                if (purchase.Status == (int)PurchaseStatus.Void)
                {
                    throw new Exception("采购单已作废，无法收货!");
                }

                if (purchase.Status == (int)PurchaseStatus.Close)
                {
                    throw new Exception("采购单已关闭，无法收货!");
                }
            }
            else
            {
                throw new Exception("采购单不存在!");
            }

            var purchaseDetailList = _purchaseRepository.GetPurchaseDetailSku(receiptOperationDto.PurchaseSysId).ToList();
            //统计本次收货数量
            int totalReceivedQty = 0;
            int totalRejectedQty = 0;


            if (purchase.Type == (int)PurchaseType.TransferInventory

                //注:这个条件仅用于新老移仓批次逻辑并行时，只有 transferinventoryreceiptextend 表存在数据时，才会走新移仓批次收货逻辑
                && _crudRepository.GetQuery<transferinventoryreceiptextend>(p => p.PurchaseSysId == purchase.SysId && p.WarehouseSysId == purchase.WarehouseSysId).Count() > 0)
            {
                //移仓收货 保存明细
                SaveReceiptDetailByTransferInventoryLot(receiptOperationDto, purchaseDetailList, purchase);
            }
            else
            {
                //根据批次保存入库明细信息
                SaveReceiptDetailByLot(receiptOperationDto, purchaseDetailList, purchase);
            }

            #region 采购明细业务
            var purchaseFinish = true;   //采购单状态控制
            var newPurchaseDetail = new List<PurchaseDetailDto>();
            var receiptUpdatePurchaseList = new List<ReceiptOperationUpdatePurchaseDto>();
            var receiptDataRecordList = new List<receiptdatarecord>();
            if (receiptOperationDto.PurchaseDetailViewDto.Any())
            {
                var skuPackList = _packageRepository.GetSkuPackageList(receiptOperationDto.PurchaseDetailViewDto.Select(x => (Guid)x.SkuSysId).ToList());
                foreach (var info in receiptOperationDto.PurchaseDetailViewDto)
                {
                    var rejected = receiptOperationDto.ReceiptDetailOperationDto.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                    var receivedQty = 0;
                    var rejectedQty = 0;
                    var rejectedGiftQty = 0;
                    var giftQty = 0;
                    var adjustmentQty = 0;

                    #region 原材料单位数量转换
                    var requestQty = receiptOperationDto.LotTemplateValueDtos.Where(x => x.SkuSysId == info.SkuSysId)
                           .Sum(x => x.Qty);

                    var skuInfo = skuPackList.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                    if (skuInfo == null)
                    {
                        throw new Exception("Id为：" + info.SkuSysId + "商品信息不存在");
                    }
                    skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                    skuInfo.UnitQty = requestQty;
                    _packageAppService.GetSkuConversionQty(ref skuInfo);
                    if (skuInfo.result)
                    {
                        totalReceivedQty += skuInfo.BaseQty;
                        info.ReceivedQty = skuInfo.BaseQty;
                        receivedQty = skuInfo.BaseQty;
                        if (rejected != null)
                        {
                            skuInfo.UnitQty = rejected.RejectedQty;
                            _packageAppService.GetSkuConversionQty(ref skuInfo);
                            totalRejectedQty += skuInfo.BaseQty;
                            info.RejectedQty = skuInfo.BaseQty;
                            rejectedQty = skuInfo.BaseQty;

                            //拒收赠品数量原材料转换
                            skuInfo.UnitQty = rejected.RejectedGiftQty;
                            _packageAppService.GetSkuConversionQty(ref skuInfo);
                            rejectedGiftQty = skuInfo.BaseQty;

                            skuInfo.UnitQty = Convert.ToInt32(rejected.GiftQty);
                            _packageAppService.GetSkuConversionQty(ref skuInfo);
                            giftQty = skuInfo.BaseQty;

                            //破损数量转化
                            skuInfo.UnitQty = rejected.AdjustmentQty;
                            _packageAppService.GetSkuConversionQty(ref skuInfo);
                            adjustmentQty = skuInfo.BaseQty;
                        }
                    }
                    else
                    {
                        receivedQty = Convert.ToInt32(receiptOperationDto.LotTemplateValueDtos.Where(x => x.SkuSysId == info.SkuSysId).Sum(x => x.Qty));
                        totalReceivedQty += receivedQty;
                        info.ReceivedQty = receivedQty;
                        if (rejected != null)
                        {
                            rejectedQty = Convert.ToInt32(rejected.RejectedQty);
                            rejectedGiftQty = Convert.ToInt32(rejected.RejectedGiftQty);
                            totalRejectedQty += rejectedQty;
                            info.RejectedQty = rejectedQty;
                            giftQty = Convert.ToInt32(rejected.GiftQty);
                            adjustmentQty = rejected.AdjustmentQty;
                        }
                    }
                    #endregion

                    if(rejected != null)
                    {
                        receiptUpdatePurchaseList.Add(new ReceiptOperationUpdatePurchaseDto
                        {
                            PurchaseDetailSysId = info.SysId.Value,
                            SkuUPC = info.SkuUPC,
                            ReceivedQty = receivedQty,
                            RejectedQty = rejectedQty,
                            Rejected = rejected,
                            GiftQty = giftQty,
                            RejectedGiftQty = rejectedGiftQty,
                            //破损数量
                            DamagedQuantity = adjustmentQty
                        });
                    }

                    if (receivedQty > 0)
                    {
                        receiptDataRecordList.Add(new receiptdatarecord
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = receiptOperationDto.WarehouseSysId,
                            ReceiptSysId = receiptOperationDto.SysId,
                            ReceiptOrder = receiptOperationDto.ReceiptOrder,
                            SkuSysId = new Guid(Convert.ToString(info.SkuSysId)),
                            Qty = receivedQty,
                            GiftQty = giftQty,
                            RejectedQty = Convert.ToInt32(info.RejectedQty),
                            GiftRejectedQty = rejectedGiftQty,
                            Remark = string.Empty,
                            CreateBy = receiptOperationDto.CurrentUserId,
                            CreateDate = DateTime.Now,
                            UpdateBy = receiptOperationDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            CreateUserName = receiptOperationDto.CurrentDisplayName,
                            UpdateUserName = receiptOperationDto.CurrentDisplayName,
                            //增加化肥破损数量
                            AdjustmentQty = adjustmentQty
                        });
                    }
                }

                //更新入库单明细数量
                if (receiptUpdatePurchaseList.Any())
                {
                    var updatePurchaseDetailList = new List<UpdatePurchaseDetailDto>();
                    var purchaseDetailSysIds = receiptUpdatePurchaseList.Select(p => p.PurchaseDetailSysId);
                    var purchaseDetails = _crudRepository.GetQuery<purchasedetail>(p => purchaseDetailSysIds.Contains(p.SysId)).ToList();
                    foreach (var purchaseDetail in purchaseDetails)
                    {
                        var receiptUpdatePurchase = receiptUpdatePurchaseList.FirstOrDefault(p => p.PurchaseDetailSysId == purchaseDetail.SysId);
                        ValidateReceiptData(receiptUpdatePurchase, purchaseDetail);
                        if ((receiptUpdatePurchase.ReceivedQty + (receiptUpdatePurchase.Rejected != null ? receiptUpdatePurchase.RejectedQty : 0)) <= (purchaseDetail.Qty - purchaseDetail.ReceivedQty - purchaseDetail.RejectedQty))
                        {
                            var updatePurchaseDetail = new UpdatePurchaseDetailDto();
                            updatePurchaseDetail.SysId = receiptUpdatePurchase.PurchaseDetailSysId;
                            updatePurchaseDetail.ReceivedQty = receiptUpdatePurchase.ReceivedQty;
                            updatePurchaseDetail.ReceivedGiftQty = receiptUpdatePurchase.GiftQty;
                            updatePurchaseDetail.UpdateBy = receiptOperationDto.CurrentUserId;
                            updatePurchaseDetail.UpdateUserName = receiptOperationDto.CurrentDisplayName;
                            updatePurchaseDetail.UpdateDate = DateTime.Now;

                            if (receiptUpdatePurchase.Rejected != null)
                            {
                                updatePurchaseDetail.RejectedQty = receiptUpdatePurchase.RejectedQty;
                                updatePurchaseDetail.RejectedGiftQty = receiptUpdatePurchase.RejectedGiftQty;
                                updatePurchaseDetail.Remark = receiptUpdatePurchase.Rejected.Descr;
                            }
                            else
                            {
                                updatePurchaseDetail.RejectedQty = 0;
                                updatePurchaseDetail.RejectedGiftQty = 0;
                                updatePurchaseDetail.Remark = string.Empty;
                            }
                            updatePurchaseDetailList.Add(updatePurchaseDetail);

                            //接口 实体 统计本次采购单更新的数量
                            newPurchaseDetail.Add(new PurchaseDetailDto()
                            {
                                OtherSkuId = purchaseDetail.OtherSkuId,
                                ReceivedQty = receiptUpdatePurchase.ReceivedQty,
                                RejectedQty = receiptUpdatePurchase.RejectedQty,
                                Remark = receiptUpdatePurchase.Rejected.Descr,
                                RejectedGiftQty = receiptUpdatePurchase.RejectedGiftQty,
                                GiftQty = receiptUpdatePurchase.GiftQty,
                                DamagedQuantity = receiptUpdatePurchase.DamagedQuantity

                            });
                        }
                        else
                        {
                            throw new Exception(string.Format("本次收货数量不能大于剩余收货数量，请重新加载数据", receiptUpdatePurchase.SkuUPC));
                        }
                    }
                    if (updatePurchaseDetailList.Any())
                    {
                        _wmsSqlRepository.UpdatePurchaseDetailAfterReceipt(updatePurchaseDetailList);
                    }

                    if (receiptDataRecordList.Any())
                    {
                        _wmsSqlRepository.BatchInsertReceiptDataRecordList(receiptDataRecordList);
                    }
                }
            }
            #endregion

            foreach (var info in purchaseDetailList)
            {
                var purchaseDetail = _crudRepository.Get<purchasedetail>(info.PurchaseDetailSysId.Value);
                var receiptUpdatePurchase = receiptUpdatePurchaseList.FirstOrDefault(p => p.PurchaseDetailSysId == info.PurchaseDetailSysId.Value) ?? new ReceiptOperationUpdatePurchaseDto();
                if (purchaseFinish && (purchaseDetail.Qty - purchaseDetail.ReceivedQty - purchaseDetail.RejectedQty) > receiptUpdatePurchase.ReceivedQty + receiptUpdatePurchase.RejectedQty)
                {
                    purchaseFinish = false;
                    break;
                }
            }

            #region 采购主表状态

            if (!purchaseFinish)
            {
                purchase.Status = (int)PurchaseStatus.PartReceipt;
            }
            else
            {
                purchase.Status = (int)PurchaseStatus.Finish;
            }
            purchase.UpdateDate = DateTime.Now;
            purchase.UpdateBy = receiptOperationDto.CurrentUserId;
            purchase.UpdateUserName = receiptOperationDto.CurrentDisplayName;
            purchase.LastReceiptDate = DateTime.Now;
            _crudRepository.Update(purchase);
            #endregion

            receipt.ReceiptDate = DateTime.Now;
            receipt.Status = (int)ReceiptStatus.Received;
            receipt.Descr = receiptOperationDto.Descr;
            receipt.TotalExpectedQty = totalReceivedQty;
            receipt.TotalReceivedQty = totalReceivedQty;
            receipt.TotalRejectedQty = totalRejectedQty;
            receipt.UpdateBy = receiptOperationDto.CurrentUserId;
            receipt.UpdateUserName = receiptOperationDto.CurrentDisplayName;
            receipt.UpdateDate = DateTime.Now;
            receipt.TS = Guid.NewGuid();
            _crudRepository.Update(receipt);

            if (!string.IsNullOrEmpty(purchase.ExternalOrder) && (purchase.Type == (int)PurchaseType.Purchase || purchase.Type == (int)PurchaseType.Material || purchase.Type == (int)PurchaseType.Return || purchase.Type == (int)PurchaseType.Fertilizer))
            {
                var crpe = new CommonResponse();
                try
                {
                    crpe = _thirdPartyAppService.InsertInStock(receiptOperationDto.CurrentUserId, receiptOperationDto.CurrentDisplayName, purchase.SysId, newPurchaseDetail);
                }
                catch (Exception ex)
                {
                }
            }

            if (receiptOperationDto.SNList != null && receiptOperationDto.SNList.Count > 0 && receiptOperationDto.ReceiptDetailOperationDto.Count > 0)
            {
                List<receiptsn> snlist = new List<receiptsn>();

                receiptOperationDto.SNList.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        snlist.Add(new receiptsn()
                        {
                            SysId = Guid.NewGuid(),
                            ReceiptSysId = receipt.SysId,
                            PurchaseSysId = purchase.SysId,
                            WarehouseSysId = receipt.WarehouseSysId,
                            SN = p,

                            //当前红卡SN业务，收货单只会存在同一种商品，因此取第一个即可
                            //后边当业务存在变化时，需要调整此处逻辑
                            SkuSysId = receiptOperationDto.ReceiptDetailOperationDto.First().SkuSysId,
                            Status = (int)ReceiptSNStatus.Receive,
                            CreateBy = receiptOperationDto.CurrentUserId,
                            CreateUserName = receiptOperationDto.CurrentDisplayName,
                            CreateDate = DateTime.Now,
                            UpdateBy = receiptOperationDto.CurrentUserId,
                            UpdateUserName = receiptOperationDto.CurrentDisplayName,
                            UpdateDate = DateTime.Now
                        });
                    }
                });

                _crudRepository.BatchInsertReceiptSN(snlist);
            }

            //入库完成的移仓单，发送MQ消息
            if (receipt.Status == (int)ReceiptStatus.Received && purchase.Type == (int)PurchaseType.TransferInventory)
            {
                var transferInventoryDto = new MQTransferInventoryDto
                {
                    TransferInventoryOrder = purchase.ExternalOrder,
                    Status = purchase.Status == (int)PurchaseStatus.PartReceipt ? (int)TransferInventoryStatus.PartReceipt : (int)TransferInventoryStatus.ReceiptFinish,
                    //Status = purchase.Status,
                    PurchaseSysId = purchase.SysId,
                    ReceiptSysId = receipt.SysId,
                    PurchaseDetailViewDto = receiptOperationDto.PurchaseDetailViewDto.Where(x => x.ReceivedQty > 0).ToList(),
                    CurrentUserId = receiptOperationDto.CurrentUserId,
                    CurrentDisplayName = receiptOperationDto.CurrentDisplayName,
                    FromWareHouseSysId = (Guid)purchase.FromWareHouseSysId
                };

                var processDto = new MQProcessDto<MQTransferInventoryDto>()
                {
                    BussinessSysId = purchase.SysId,
                    BussinessOrderNumber = purchase.PurchaseOrder,
                    Descr = "",
                    CurrentUserId = receiptOperationDto.CurrentUserId,
                    CurrentDisplayName = receiptOperationDto.CurrentDisplayName,
                    WarehouseSysId = receiptOperationDto.WarehouseSysId,
                    BussinessDto = transferInventoryDto
                };

                #region 接口调用
                var response = ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/MQ/OrderManagement/TransferInventory/UpdateTransferInventoryStatus", new CoreQuery(), transferInventoryDto);
                if (response == null || response.ResponseResult == null || !response.ResponseResult.IsSuccess)
                {
                    throw new Exception("移仓单状态修改失败。");
                }
                #endregion
                //_transferInventoryAppService.UpdateTransferInventoryStatus(transferInventoryDto);

            }

            #region 组织推送收货完成工单数据
            if (receipt != null)
            {
                var mqReceiptWorkDto = new MQWorkDto()
                {
                    WorkBusinessType = (int)WorkBusinessType.Update,
                    WorkType = (int)UserWorkType.Receipt,
                    WarehouseSysId = receiptOperationDto.WarehouseSysId,
                    CurrentUserId = receiptOperationDto.CurrentUserId,
                    CurrentDisplayName = receiptOperationDto.CurrentDisplayName,
                    CancelWorkDto = new CancelWorkDto()
                    {
                        DocSysIds = new List<Guid>() { receipt.SysId },
                        Status = (int)WorkStatus.Finish
                    }
                };

                var receiptWorkProcessDto = new MQProcessDto<MQWorkDto>()
                {
                    BussinessSysId = receipt.SysId,
                    BussinessOrderNumber = receipt.ReceiptOrder,
                    Descr = "",
                    CurrentUserId = receiptOperationDto.CurrentUserId,
                    CurrentDisplayName = receiptOperationDto.CurrentDisplayName,
                    WarehouseSysId = receiptOperationDto.WarehouseSysId,
                    BussinessDto = mqReceiptWorkDto
                };
                //推送工单数据
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, receiptWorkProcessDto);
            }
            #endregion

            #region 组织上架工单数据
            var workRule = _crudRepository.GetQuery<workrule>(x => x.WarehouseSysId == receiptOperationDto.WarehouseSysId).FirstOrDefault();
            if (workRule != null && workRule.Status == true && workRule.ShelvesWork == true)
            {
                var workDetailList = new List<WorkDetailDto>();
                if (receipt != null)
                {
                    var workDetail = new WorkDetailDto()
                    {
                        Status = (int)WorkStatus.Working,
                        WorkType = (int)UserWorkType.Shelve,
                        Priority = 1,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        WorkTime = DateTime.Now,
                        Source = "上架",
                        DocSysId = receipt.SysId,
                        DocOrder = receipt.ReceiptOrder,
                        WarehouseSysId = receiptOperationDto.WarehouseSysId,
                        CurrentUserId = receiptOperationDto.CurrentUserId,
                        CurrentDisplayName = receiptOperationDto.CurrentDisplayName
                    };
                    workDetailList.Add(workDetail);

                    #region 组织推送工单到MQ
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Insert,
                        WorkType = (int)UserWorkType.Shelve,
                        WorkDetailDtoList = workDetailList,
                        WarehouseSysId = receiptOperationDto.WarehouseSysId,
                        CurrentUserId = receiptOperationDto.CurrentUserId,
                        CurrentDisplayName = receiptOperationDto.CurrentDisplayName
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = receipt.SysId,
                        BussinessOrderNumber = receipt.ReceiptOrder,
                        Descr = "",
                        CurrentUserId = receiptOperationDto.CurrentUserId,
                        CurrentDisplayName = receiptOperationDto.CurrentDisplayName,
                        WarehouseSysId = receiptOperationDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                    #endregion
                }
            }
            #endregion

            return string.Empty;
        }

        /// <summary>
        /// 根据批次写入明细信息
        /// </summary>
        private void SaveReceiptDetailByLot(ReceiptOperationDto receiptOperationDto, List<PurchaseDetailSkuDto> purchaseDetailList, purchase purchase)
        {
            if (receiptOperationDto.LotTemplateValueDtos.Any())
            {
                var receiptDetails = new List<receiptdetail>();
                var skuPackList = _packageRepository.GetSkuPackageList(receiptOperationDto.LotTemplateValueDtos.Select(x => x.SkuSysId).ToList());

                foreach (var info in receiptOperationDto.LotTemplateValueDtos)
                {
                    var purchaseDetail = purchaseDetailList.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                    var receiptDetail = new receiptdetail();
                    receiptDetail.SysId = Guid.NewGuid();
                    receiptDetail.ReceiptSysId = receiptOperationDto.SysId;
                    receiptDetail.Status = (int)ReceiptDetailStatus.Received;
                    receiptDetail.SkuSysId = info.SkuSysId;

                    #region 原材料单位数量转换 
                    var requestQty = info.Qty;
                    var skuInfo = skuPackList.Where(x => x.SkuSysId == info.SkuSysId).FirstOrDefault();
                    if (skuInfo == null)
                    {
                        throw new Exception("Id为：" + info.SkuSysId + "商品信息不存在");
                    }
                    skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                    skuInfo.UnitQty = info.Qty;
                    _packageAppService.GetSkuConversionQty(ref skuInfo);
                    receiptDetail.ExpectedQty = skuInfo.BaseQty;
                    receiptDetail.ReceivedQty = skuInfo.BaseQty;
                    info.ReceivedQty = skuInfo.BaseQty;
                    #endregion

                    receiptDetail.UOMSysId = purchaseDetail.UOMSysId;
                    receiptDetail.PackSysId = purchaseDetail.PackSysId;
                    receiptDetail.ReceivedDate = DateTime.Now;
                    receiptDetail.Price = purchaseDetail.PurchasePrice;
                    receiptDetail.LotAttr01 = purchase.Channel;
                    receiptDetail.LotAttr02 = purchase.BatchNumber;
                    receiptDetail.LotAttr03 = purchase.BusinessType;     //业务类型，上下行
                    receiptDetail.LotAttr04 = info.LotValue04;
                    receiptDetail.LotAttr05 = info.LotValue05;
                    receiptDetail.LotAttr06 = info.LotValue06;
                    receiptDetail.LotAttr07 = info.LotValue07;
                    receiptDetail.LotAttr08 = purchaseDetail.PurchasePrice.ToString();
                    receiptDetail.LotAttr09 = purchase.PurchaseOrder;
                    receiptDetail.ExternalLot = info.ExternalLot;
                    receiptDetail.ProduceDate = info.ProduceDate;
                    receiptDetail.ExpiryDate = info.ExpiryDate;
                    receiptDetail.ToLoc = purchaseDetail.RecommendLoc;
                    receiptDetail.ShelvesQty = 0;
                    receiptDetail.ShelvesStatus = (int)ShelvesStatus.NotOnShelves;
                    receiptDetail.CreateBy = receiptOperationDto.CurrentUserId;
                    receiptDetail.CreateUserName = receiptOperationDto.CurrentDisplayName;
                    receiptDetail.CreateDate = DateTime.Now;
                    receiptDetail.UpdateBy = receiptOperationDto.CurrentUserId;
                    receiptDetail.UpdateDate = DateTime.Now;
                    receiptDetail.UpdateUserName = receiptOperationDto.CurrentDisplayName;
                    receiptDetail.IsDefaultLot = true;
                    receiptDetail.IsMustLot = GetIsMustLot(receiptOperationDto.PurchaseDetailViewDto, receiptDetail.SkuSysId);
                    receiptDetails.Add(receiptDetail);
                }

                //退货入库的收货，强制生成新批次 2017-09-19
                if (purchase.Type == (int)PurchaseType.Return)
                {
                    var lots = _baseAppService.GetNumber(PublicConst.GenNextNumberLot, receiptDetails.Count());
                    foreach (var receiptDetail in receiptDetails)
                    {
                        if (receiptDetail.IsMustLot != true)
                        {
                            receiptDetail.ToLot = lots[0];
                            lots.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    #region 检测是否存在相同批次  
                    //var checkLots = _crudRepository.GetToLotByReceiptDetail(receiptDetails, receiptOperationDto.WarehouseSysId);
                    var checkLots = _wmsSqlRepository.GetToLotByReceiptDetail(receiptDetails, receiptOperationDto.PurchaseSysId, receiptOperationDto.WarehouseSysId);
                    #endregion
                    foreach (var receiptDetail in receiptDetails)
                    {
                        var checkLot = checkLots.FirstOrDefault(p => p.SysId == receiptDetail.SysId && p.CheckLotSysId.HasValue);
                        if (checkLot != null && receiptDetail.IsMustLot != true)
                        {
                            receiptDetail.ToLot = checkLot.ToLot;
                        }
                    }
                    var noLotDetails = receiptDetails.Where(p => string.IsNullOrEmpty(p.ToLot) && p.IsMustLot != true);
                    if (noLotDetails.Any())
                    {
                        //var lots = _crudRepository.BatchGenNextNumber(PublicConst.GenNextNumberLot, noLotDetails.Count());
                        var lots = _baseAppService.GetNumber(PublicConst.GenNextNumberLot, noLotDetails.Count());
                        foreach (var receiptDetail in receiptDetails)
                        {
                            if (string.IsNullOrEmpty(receiptDetail.ToLot) && receiptDetail.IsMustLot != true)
                            {
                                receiptDetail.ToLot = lots[0];
                                lots.RemoveAt(0);
                            }
                        }
                    }
                }

                _wmsSqlRepository.BatchInsertReceiptDetail(receiptDetails);
            }
        }

        /// <summary>
        /// 移仓入库收货-根据批次写入明细信息
        /// </summary>
        private void SaveReceiptDetailByTransferInventoryLot(ReceiptOperationDto receiptOperationDto, List<PurchaseDetailSkuDto> purchaseDetailList, purchase purchase)
        {
            if (receiptOperationDto.LotTemplateValueDtos.Any())
            {
                var transferinventoryreceiptList = _crudRepository.GetQuery<transferinventoryreceiptextend>(p => p.PurchaseSysId == purchase.SysId && p.WarehouseSysId == purchase.WarehouseSysId);
                var receiptDetails = new List<receiptdetail>();
                var skuPackList = _packageRepository.GetSkuPackageList(receiptOperationDto.LotTemplateValueDtos.Select(x => x.SkuSysId).ToList());

                foreach (var info in receiptOperationDto.LotTemplateValueDtos)
                {

                    //TODO: 此处批次用的是移仓前的自动寻找填充，后期如果移仓收货支持扫描批次号，那么就需要用UI传来的批次号
                    var transferinventoryreceipts = transferinventoryreceiptList.Where(p => p.SkuSysId == info.SkuSysId && (p.ReceivedQty < p.Qty)).ToList();

                    if (transferinventoryreceipts == null || transferinventoryreceipts.Count == 0)
                    {
                        throw new Exception("Id为：" + info.SkuSysId + "商品批次属性不全，请联系support");
                    }

                    var purchaseDetail = purchaseDetailList.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                    
                    info.ReceivedQty = 0;

                    #region 原材料单位数量转换 

                    var skuInfo = skuPackList.Where(x => x.SkuSysId == info.SkuSysId).FirstOrDefault();
                    if (skuInfo == null)
                    {
                        throw new Exception("Id为：" + info.SkuSysId + "商品信息不存在");
                    }
                    skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                    skuInfo.UnitQty = info.Qty;
                    _packageAppService.GetSkuConversionQty(ref skuInfo);
                    var requestQty = skuInfo.BaseQty;
                    info.ReceivedQty += skuInfo.BaseQty;
                    #endregion

                    //此处循环是因为移仓前源仓库的同一个商品有可能有多个批次，因此收货的时候，需要根据之前的批次及数量对应到本次收货时候的数量
                    //保证全部收货完成之后，移仓前后两边商品的批次，属性 以及数量都一一对应
                    foreach (var item in transferinventoryreceipts)
                    {
                        if (requestQty == 0)
                        {
                            break;
                        }
                        var receiptDetail = new receiptdetail();
                        receiptDetail.SysId = Guid.NewGuid();
                        receiptDetail.ReceiptSysId = receiptOperationDto.SysId;
                        receiptDetail.Status = (int)ReceiptDetailStatus.Received;
                        receiptDetail.SkuSysId = info.SkuSysId;

                        int receivedQty = 0;
                        if ((item.Qty - item.ReceivedQty) >= requestQty)
                        {
                            receivedQty = requestQty;
                            requestQty = 0;
                            item.ReceivedQty += receivedQty;
                        }
                        else
                        {
                            receivedQty = (item.Qty - item.ReceivedQty);
                            requestQty = requestQty - (item.Qty - item.ReceivedQty);
                            item.ReceivedQty = item.Qty;
                        }

                        receiptDetail.ExpectedQty = receivedQty;
                        receiptDetail.ReceivedQty = receivedQty;

                        receiptDetail.UOMSysId = purchaseDetail.UOMSysId;
                        receiptDetail.PackSysId = purchaseDetail.PackSysId;
                        receiptDetail.ReceivedDate = DateTime.Now;
                        receiptDetail.Price = purchaseDetail.PurchasePrice;

                        receiptDetail.ToLot = item.Lot;
                        receiptDetail.LotAttr01 = item.LotAttr01;
                        receiptDetail.LotAttr02 = item.LotAttr02;
                        receiptDetail.LotAttr03 = item.LotAttr03;   
                        receiptDetail.LotAttr04 = item.LotAttr04;
                        receiptDetail.LotAttr05 = item.LotAttr05;
                        receiptDetail.LotAttr06 = item.LotAttr06;
                        receiptDetail.LotAttr07 = item.LotAttr07;
                        receiptDetail.LotAttr08 = item.LotAttr08;
                        receiptDetail.LotAttr09 = item.LotAttr09;
                        receiptDetail.ExternalLot = item.ExternalLot;
                        receiptDetail.ProduceDate = item.ProduceDate;
                        receiptDetail.ExpiryDate = item.ExpiryDate;

                        receiptDetail.ToLoc = purchaseDetail.RecommendLoc;
                        receiptDetail.ShelvesQty = 0;
                        receiptDetail.ShelvesStatus = (int)ShelvesStatus.NotOnShelves;
                        receiptDetail.CreateBy = receiptOperationDto.CurrentUserId;
                        receiptDetail.CreateUserName = receiptOperationDto.CurrentDisplayName;
                        receiptDetail.CreateDate = DateTime.Now;
                        receiptDetail.UpdateBy = receiptOperationDto.CurrentUserId;
                        receiptDetail.UpdateDate = DateTime.Now;
                        receiptDetail.UpdateUserName = receiptOperationDto.CurrentDisplayName;
                        receiptDetail.IsDefaultLot = false;
                        receiptDetail.IsMustLot = false;
                        receiptDetails.Add(receiptDetail);

                        _crudRepository.Update(item);
                    }
                }

                _wmsSqlRepository.BatchInsertReceiptDetail(receiptDetails);
            }
        }

        public bool GetIsMustLot(List<PurchaseDetailViewDto> purchaseDetailViewList, Guid skuSysId)
        {
            var isMustLot = false;
            var purchaseDetailView = purchaseDetailViewList.FirstOrDefault(x => x.SkuSysId == skuSysId);
            if (purchaseDetailView != null && purchaseDetailView.LotTemplateDto != null)
            {
                var info = purchaseDetailView.LotTemplateDto;
                if (info.LotMandatory04 == true || info.LotMandatory05 == true || info.LotMandatory06 == true || info.LotMandatory07 == true
                    || info.LotMandatory10 == true || info.LotMandatory11 == true || info.LotMandatory12 == true)
                {
                    isMustLot = true;
                }
            }
            return isMustLot;
        }

        private void ValidateReceiptData(ReceiptOperationUpdatePurchaseDto receiptUpdatePurchase, purchasedetail purchaseDetail)
        {
            if(receiptUpdatePurchase.ReceivedQty <= 0)
            {
                throw new Exception(string.Format("收货数量必须大于0", receiptUpdatePurchase.SkuUPC));
            }

            if ((receiptUpdatePurchase.ReceivedQty + (receiptUpdatePurchase.Rejected != null ? receiptUpdatePurchase.RejectedQty : 0)) > (purchaseDetail.Qty - purchaseDetail.ReceivedQty - purchaseDetail.RejectedQty))
            {
                throw new Exception(string.Format("本次收货数量不能大于剩余收货数量，请重新加载数据", receiptUpdatePurchase.SkuUPC));
            }
            if ((receiptUpdatePurchase.GiftQty + (receiptUpdatePurchase.Rejected != null ? receiptUpdatePurchase.RejectedGiftQty : 0)) > (purchaseDetail.GiftQty - purchaseDetail.ReceivedGiftQty - purchaseDetail.RejectedGiftQty))
            {
                throw new Exception(string.Format("本次赠品收货数量不能大于剩余赠品收货数量，请重新加载数据", receiptUpdatePurchase.SkuUPC));
            }

            if (((receiptUpdatePurchase.ReceivedQty + (receiptUpdatePurchase.Rejected != null ? receiptUpdatePurchase.RejectedQty : 0)) - (receiptUpdatePurchase.GiftQty + (receiptUpdatePurchase.Rejected != null ? receiptUpdatePurchase.RejectedGiftQty : 0))) > ((purchaseDetail.Qty - purchaseDetail.ReceivedQty - purchaseDetail.RejectedQty) - (purchaseDetail.GiftQty - purchaseDetail.ReceivedGiftQty - purchaseDetail.RejectedGiftQty)))
            {
                throw new Exception(string.Format("本次正品收货数量不能大于剩余正品收货数量，请重新加载数据", receiptUpdatePurchase.SkuUPC));
            }
        }


        #region 收货单管理
        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Pages<ReceiptListDto> GetReceiptList(ReceiptQuery receiptQuery)
        {
            _crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            return _crudRepository.GetReceiptListByPaging(receiptQuery);
        }

        /// <summary>
        /// 获取收货单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ReceiptViewDto GetReceiptViewById(Guid sysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            ReceiptViewDto receiptViewDto = _crudRepository.GetReceiptViewById(sysId);
            if (receiptViewDto != null)
            {
                receiptViewDto.ReceiptDetailViewDtoList = _crudRepository.GetReceiptDetailViewList(sysId);
                receiptViewDto.RelatedReceiptPurchaseDtoList = _crudRepository.GetAllList<receipt>(p =>
                   p.ExternalOrder == receiptViewDto.ExternalOrder
                    && p.SysId != sysId).JTransformTo<ReceiptPurchaseDto>();

                if (receiptViewDto.ReceiptDetailViewDtoList != null && receiptViewDto.ReceiptDetailViewDtoList.Count > 0)
                {
                    receiptViewDto.DisplayTotalExpectedQty = receiptViewDto.ReceiptDetailViewDtoList.Sum(p => p.DisplayExpectedQty);
                    receiptViewDto.DisplayTotalReceivedQty = receiptViewDto.ReceiptDetailViewDtoList.Sum(p => p.DisplayReceivedQty);
                    receiptViewDto.DisplayTotalRejectedQty = receiptViewDto.ReceiptDetailViewDtoList.Sum(p => p.DisplayRejectedQty);
                }



            }
            else
            {
                receiptViewDto = new ReceiptViewDto
                {
                    ReceiptDetailViewDtoList = new List<ReceiptDetailViewDto>(),
                    RelatedReceiptPurchaseDtoList = new List<ReceiptPurchaseDto>()
                };
            }
            return receiptViewDto;
        }

        #endregion

        /// <summary>
        /// 根据条件获取收货单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public ReceiptDto GetReceiptOrderByOrderId(ReceiptQuery receiptQuery)
        {

            _crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            var query = _crudRepository.GetQuery<receipt>(x => x.ReceiptOrder == receiptQuery.ReceiptOrderSearch && x.WarehouseSysId == receiptQuery.WarehouseSysId);

            if (receiptQuery.WaitShelvesSearch)
            {
                query = query.Where(x => x.Status == (int)ReceiptStatus.Receiving || x.Status == (int)ReceiptStatus.Received);
            }

            return query.FirstOrDefault().JTransformTo<ReceiptDto>();
        }

        #region 取消收货
        /// <summary>
        /// 取消收货（入库单整单取消）
        /// </summary>
        /// <param name="receiptCancelDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse CancelReceiptByPurchase(ReceiptCancelDto receiptCancelDto)
        {
            _crudRepository.ChangeDB(receiptCancelDto.WarehouseSysId);
            var rsp = new CommonResponse() { IsSuccess = false };
            var purchaseOrder = string.Empty;
            try
            {
                var purchase = _crudRepository.Get<purchase>(receiptCancelDto.PurchaseSysId);
                if (purchase == null)
                {
                    throw new Exception("入库单不存在");
                }
                if (purchase.Status == (int)PurchaseStatus.New || purchase.Status == (int)PurchaseStatus.Close || purchase.Status == (int)PurchaseStatus.StopReceipt || purchase.Status == (int)PurchaseStatus.Void)
                {
                    throw new Exception(string.Format("入库单状态为{0}，不允许取消收货", ((PurchaseStatus)purchase.Status).ToDescription()));
                }

                #region 判断是否已上架
                var receiptList = _crudRepository.GetQuery<receipt>(x => x.ExternalOrder == purchase.PurchaseOrder && x.Status != (int)ReceiptStatus.Cancel).ToList();
                List<Guid> receiptSysIds = receiptList.Select(x => x.SysId).ToList();
                var receiptDetailList = _crudRepository.GetQuery<receiptdetail>(x => receiptSysIds.Contains(x.ReceiptSysId) && x.Status != (int)ReceiptDetailStatus.Cancel && (x.ShelvesStatus == (int)ShelvesStatus.Shelves || x.ShelvesStatus == (int)ShelvesStatus.Finish));
                if (receiptDetailList != null && receiptDetailList.Count() > 0)
                {
                    throw new Exception("入库单中有已上架的商品，请先取消上架");
                }
                #endregion

                purchaseOrder = purchase.PurchaseOrder;
                purchase.Status = (int)PurchaseStatus.New;
                purchase.UpdateBy = receiptCancelDto.CurrentUserId;
                purchase.UpdateDate = DateTime.Now;
                purchase.UpdateUserName = receiptCancelDto.CurrentDisplayName;
                //_crudRepository.Update(purchase);

                receiptCancelDto.PurchaseOrder = purchaseOrder;

                //_wmsSqlRepository.UpdateReceiptDetailCancelReceipt(receiptCancelDto);
                var transferinventoryreceiptList = _crudRepository.GetQuery<transferinventoryreceiptextend>(p => p.PurchaseSysId == purchase.SysId && p.WarehouseSysId == purchase.WarehouseSysId).ToList();
                if (purchase.Type == (int)PurchaseType.TransferInventory && transferinventoryreceiptList.Count > 0)
                {
                    transferinventoryreceiptList.ForEach(p => {
                        p.ReceivedQty = 0;
                    });
                }
                CancelReceiptByPurchaseSaveChange(purchase, receiptCancelDto, transferinventoryreceiptList);

                #region 组织推送取消拣上架工单数据
                if (purchase != null && receiptList != null && receiptList.Count > 0)
                {
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Update,
                        WorkType = (int)UserWorkType.Receipt,
                        WarehouseSysId = receiptCancelDto.WarehouseSysId,
                        CurrentUserId = receiptCancelDto.CurrentUserId,
                        CurrentDisplayName = receiptCancelDto.CurrentDisplayName,
                        CancelWorkDto = new CancelWorkDto()
                        {
                            DocSysIds = receiptList.Select(x => x.SysId).ToList(),
                            Status = (int)WorkStatus.Cancel
                        }
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = purchase.SysId,
                        BussinessOrderNumber = purchase.PurchaseOrder,
                        Descr = "",
                        CurrentUserId = receiptCancelDto.CurrentUserId,
                        CurrentDisplayName = receiptCancelDto.CurrentDisplayName,
                        WarehouseSysId = receiptCancelDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                }
                #endregion

                rsp.IsSuccess = true;
                rsp.ErrorMessage = string.Format("取消收货成功，单号：{0}", purchaseOrder);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var message = string.Format("取消收货失败，单号：{0}，失败原因：数据重复并发提交", purchaseOrder);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                var message = string.Format("取消收货失败，失败原因：{0}", ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            return rsp;
        }

        [UnitOfWork(isTransactional: false)]
        private void CancelReceiptByPurchaseSaveChange(purchase purchase, ReceiptCancelDto receiptCancelDto,List<transferinventoryreceiptextend> transReceiptList)
        {
            try
            {
                _crudRepository.ChangeDB(receiptCancelDto.WarehouseSysId);
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    _crudRepository.Update(purchase);
                    //修改拣货明细和出库明细
                    _wmsSqlRepository.UpdateReceiptDetailCancelReceipt(receiptCancelDto);

                    _crudRepository.CancelReceiptsnByPurchase(purchase.SysId);

                    if (purchase.Type == (int)PurchaseType.TransferInventory)
                    {
                        transReceiptList.ForEach(p => {
                            _crudRepository.Update(p);
                        });   
                    }

                    _crudRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 领料分拣
        /// <summary>
        /// 领料分拣
        /// </summary>
        /// <param name="picking"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse PickingMaterial(PickingMaterialDto picking)
        {
            _crudRepository.ChangeDB(picking.WarehouseSysId);
            var receiptOrder = string.Empty;
            var rsp = new CommonResponse() { IsSuccess = false };
            try
            {
                if (string.IsNullOrEmpty(picking.PickingUserName))
                {
                    throw new Exception("请输入领料人!");
                }

                var loc = _crudRepository.GetQuery<location>(x => x.Loc == PublicConst.PickingSkuLoc && x.WarehouseSysId == picking.WarehouseSysId).FirstOrDefault();
                if (loc == null)
                {
                    throw new Exception(string.Format("领料分拣货位：{0}不存在，请维护!", PublicConst.PickingSkuLoc));
                }

                if (loc.Status == (int)LocationStatus.Frozen)
                {
                    throw new Exception($"货位{loc.Loc}已被冻结，不能领料分拣!");
                }

                if (picking != null && picking.PickingMaterialDetailListDto != null && picking.PickingMaterialDetailListDto.Count > 0)
                {
                    picking.PickingDate = DateTime.Now;
                    picking.PickingMaterialDetailListDto = picking.PickingMaterialDetailListDto.Where(x => x.InputQty > 0).ToList();
                    if (picking.PickingMaterialDetailListDto == null && picking.PickingMaterialDetailListDto.Count == 0)
                    {
                        throw new Exception("领料明细不能为空!");
                    }

                    var receipt = _crudRepository.Get<receipt>(picking.ReceiptSysId);
                    if (receipt == null)
                    {
                        throw new Exception("此收货单不存在");
                    }

                    if (receipt.Status != (int)ReceiptStatus.Received)
                    {
                        throw new Exception("收货单状态不等于收货完成,无法进行领料分拣");
                    }

                    #region  获取领料次数
                    var pickingNumber = 1;
                    var pickingRecords = _crudRepository.GetQuery<pickingrecords>(x => x.ReceiptSysId == picking.ReceiptSysId && x.PickingUserName == picking.PickingUserName).ToList();
                    if (pickingRecords != null && pickingRecords.Count > 0)
                    {
                        pickingNumber = pickingRecords.Max(x => x.PickingNumber) + 1;
                    }
                    #endregion

                    var skuSysIdList = picking.PickingMaterialDetailListDto.Select(x => x.SkuSysId).ToList();
                    //查询收货明细
                    var receiptDetailList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == picking.ReceiptSysId).ToList();

                    //查询商品包装
                    var skuPackList = _packageRepository.GetSkuPackageList(skuSysIdList);
                    //库存
                    var existedInvlotLocLpnList = _crudRepository.GetQuery<invlotloclpn>(x => skuSysIdList.Contains(x.SkuSysId) && x.Loc == loc.Loc && x.WareHouseSysId == picking.WarehouseSysId).ToList();
                    var existedInvLotList = _crudRepository.GetQuery<invlot>(x => skuSysIdList.Contains(x.SkuSysId) && x.WareHouseSysId == picking.WarehouseSysId).ToList();
                    var existedInvSkuLocList = _crudRepository.GetQuery<invskuloc>(x => skuSysIdList.Contains(x.SkuSysId) && x.Loc == loc.Loc && x.WareHouseSysId == picking.WarehouseSysId).ToList();
                    //包装
                    var packALL = _crudRepository.GetAll<pack>().ToList();
                    //商品
                    var skuALL = _crudRepository.GetQuery<sku>(x => skuSysIdList.Contains(x.SysId)).ToList();
                    //单位
                    var uomALL = _crudRepository.GetAll<uom>().ToList();

                    //收货明细
                    var receiptDetails = new List<receiptdetail>();
                    //领料分拣记录
                    var pickingRecordsList = new List<pickingrecords>();
                    //插入库存数据
                    var invLotList = new List<invlot>();
                    var invSkuLocList = new List<invskuloc>();
                    var invLotLocLpnList = new List<invlotloclpn>();
                    //修改库存数据
                    var updateInventoryDtos = new List<UpdateInventoryDto>();
                    //交易记录
                    var invTransList = new List<invtran>();

                    //商品级别
                    var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuSysIdList.Contains(p.SkuSysId.Value)
                        && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == picking.WarehouseSysId);
                    if (frozenSkuList.Count() > 0)
                    {
                        var skuSysId = frozenSkuList.First().SkuSysId;
                        var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                        throw new Exception($"商品{frozenSku.SkuName}已被冻结，无法进行领料分拣!");
                    }

                    //货位商品级别
                    var locskuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuSysIdList.Contains(p.SkuSysId.Value)
                        && p.Loc == loc.Loc && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == picking.WarehouseSysId).ToList();

                    if (locskuList.Count > 0)
                    {
                        var firstFrozenLocsku = locskuList.First();
                        var skuSysId = firstFrozenLocsku.SkuSysId;
                        var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                        throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，无法进行领料分拣!");
                    }


                    #region 领料分拣单
                    var pickingModel = new picking()
                    {
                        SysId = Guid.NewGuid(),
                        PickingOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPicking),
                        WareHouseSysId = picking.WarehouseSysId,
                        ReceiptSysId = picking.ReceiptSysId,
                        ReceiptOrder = picking.ReceiptOrder,
                        PickingNumber = pickingNumber,
                        PickingUserId = picking.PickingUserId,
                        PickingUserName = picking.PickingUserName.Trim(),
                        PickingDate = picking.PickingDate,
                        Remark = "",
                        CreateBy = picking.CurrentUserId,
                        CreateDate = DateTime.Now,
                        CreateUserName = picking.CurrentDisplayName,
                        UpdateBy = picking.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = picking.CurrentDisplayName
                    };
                    #endregion

                    foreach (var info in picking.PickingMaterialDetailListDto)
                    {
                        #region 原材料单位数量转换
                        var skuInfo = skuPackList.FirstOrDefault(x => x.SkuSysId == info.SkuSysId);
                        if (skuInfo == null)
                        {
                            throw new Exception("Id为：" + info.SkuSysId + "商品信息不存在");
                        }
                        skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                        skuInfo.UnitQty = info.InputQty;
                        _packageAppService.GetSkuConversionQty(ref skuInfo);
                        if (skuInfo.result)
                        {
                            info.Qty = skuInfo.BaseQty;
                        }
                        else
                        {
                            info.Qty = (int)info.InputQty;
                            if (info.Qty <= 0)
                            {
                                throw new Exception("成品数量必须为大于0的整数");
                            }
                        }
                        #endregion

                        var pickingQty = info.Qty;
                        var rds = receiptDetailList.FindAll(x => x.SkuSysId == info.SkuSysId);
                        if (rds == null)
                        {
                            throw new Exception("收货明细不存在");
                        }
                        if (rds.Any(x => x.ToLot == null || x.ToLot == ""))
                        {
                            throw new Exception("有未采集批次的商品，请先采集批次");
                        }

                        foreach (var rd in rds)
                        {
                            #region 收货明细组织库存数量并修改收货明细上架数量和上架状态
                            if (pickingQty <= 0)
                            {
                                break;
                            }

                            //待领料分拣数量
                            var waitQty = (int)rd.ReceivedQty - rd.ShelvesQty;
                            //入库存数量
                            var invQty = 0;

                            if (waitQty > pickingQty)
                            {
                                rd.ShelvesQty = rd.ShelvesQty + pickingQty;
                                rd.ShelvesStatus = (int)ShelvesStatus.Shelves;
                                invQty = pickingQty;
                                pickingQty = 0;
                            }
                            else
                            {
                                rd.ShelvesQty = rd.ShelvesQty + waitQty;
                                pickingQty = pickingQty - waitQty;
                                rd.ShelvesStatus = (int)ShelvesStatus.Finish;
                                invQty = waitQty;
                            }
                            rd.TS = Guid.NewGuid();
                            receiptDetails.Add(rd);
                            #endregion

                            #region InvLotLocLpn
                            var invLotLocLpn = existedInvlotLocLpnList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == loc.Loc && x.Lpn == "" && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                            if (invLotLocLpn != null)
                            {
                                updateInventoryDtos.Add(new UpdateInventoryDto()
                                {
                                    InvLotLocLpnSysId = invLotLocLpn.SysId,
                                    InvLotSysId = new Guid(),
                                    InvSkuLocSysId = new Guid(),
                                    Qty = invQty,
                                    CurrentUserId = picking.CurrentUserId,
                                    CurrentDisplayName = picking.CurrentDisplayName,
                                    WarehouseSysId = picking.WarehouseSysId,
                                });
                            }
                            else
                            {
                                if (!invLotLocLpnList.Any(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == loc.Loc && x.Lpn == "" && x.WareHouseSysId == picking.WarehouseSysId))
                                {
                                    var newInvLotLocLpn = new invlotloclpn()
                                    {
                                        SysId = Guid.NewGuid(),
                                        WareHouseSysId = picking.WarehouseSysId,
                                        SkuSysId = rd.SkuSysId,
                                        Loc = loc.Loc,
                                        Lot = rd.ToLot,
                                        Lpn = "",
                                        Qty = invQty,
                                        AllocatedQty = 0,
                                        PickedQty = 0,
                                        Status = 1,
                                        CreateBy = picking.CurrentUserId,
                                        CreateDate = DateTime.Now,
                                        CreateUserName = picking.CurrentDisplayName,
                                        UpdateBy = picking.CurrentUserId,
                                        UpdateDate = DateTime.Now,
                                        UpdateUserName = picking.CurrentDisplayName
                                    };
                                    invLotLocLpnList.Add(newInvLotLocLpn);
                                }
                                else
                                {
                                    var oldInvLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.Loc == loc.Loc && x.Lpn == "" && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                                    oldInvLotLocLpn.Qty += invQty;
                                }
                            }
                            #endregion

                            #region InvLot
                            var invLot = existedInvLotList.Where(x => x.Lot == rd.ToLot && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                            if (invLot != null)
                            {
                                updateInventoryDtos.Add(new UpdateInventoryDto()
                                {
                                    InvLotLocLpnSysId = new Guid(),
                                    InvLotSysId = invLot.SysId,
                                    InvSkuLocSysId = new Guid(),
                                    Qty = invQty,
                                    CurrentUserId = picking.CurrentUserId,
                                    CurrentDisplayName = picking.CurrentDisplayName,
                                    WarehouseSysId = picking.WarehouseSysId,
                                });
                            }
                            else
                            {
                                if (!invLotList.Any(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.WareHouseSysId == picking.WarehouseSysId))
                                {
                                    var newInvLot = new invlot()
                                    {
                                        SysId = Guid.NewGuid(),
                                        WareHouseSysId = picking.WarehouseSysId,
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
                                        CreateBy = picking.CurrentUserId,
                                        CreateDate = DateTime.Now,
                                        CreateUserName = picking.CurrentDisplayName,
                                        UpdateBy = picking.CurrentUserId,
                                        UpdateDate = DateTime.Now,
                                        UpdateUserName = picking.CurrentDisplayName,
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
                                    var oldInvLot = invLotList.Where(x => x.SkuSysId == rd.SkuSysId && x.Lot == rd.ToLot && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                                    oldInvLot.Qty += invQty;
                                }
                            }
                            #endregion

                            #region InvSkuLoc
                            var invSkuLoc = _crudRepository.GetQuery<invskuloc>(x => x.Loc == loc.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                            if (invSkuLoc != null)
                            {
                                updateInventoryDtos.Add(new UpdateInventoryDto()
                                {
                                    InvLotLocLpnSysId = new Guid(),
                                    InvLotSysId = new Guid(),
                                    InvSkuLocSysId = invSkuLoc.SysId,
                                    Qty = invQty,
                                    CurrentUserId = picking.CurrentUserId,
                                    CurrentDisplayName = picking.CurrentDisplayName,
                                    WarehouseSysId = picking.WarehouseSysId,
                                });
                            }
                            else
                            {
                                if (!invSkuLocList.Any(x => x.Loc == loc.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == picking.WarehouseSysId))
                                {
                                    var newInvSkuLoc = new invskuloc()
                                    {
                                        SysId = Guid.NewGuid(),
                                        WareHouseSysId = picking.WarehouseSysId,
                                        SkuSysId = rd.SkuSysId,
                                        Loc = loc.Loc,
                                        Qty = invQty,
                                        AllocatedQty = 0,
                                        PickedQty = 0,
                                        CreateBy = picking.CurrentUserId,
                                        CreateDate = DateTime.Now,
                                        CreateUserName = picking.CurrentDisplayName,
                                        UpdateBy = picking.CurrentUserId,
                                        UpdateDate = DateTime.Now,
                                        UpdateUserName = picking.CurrentDisplayName
                                    };
                                    invSkuLocList.Add(newInvSkuLoc);
                                }
                                else
                                {
                                    var oldInvSkuLoc = invSkuLocList.Where(x => x.Loc == loc.Loc && x.SkuSysId == rd.SkuSysId && x.WareHouseSysId == picking.WarehouseSysId).FirstOrDefault();
                                    oldInvSkuLoc.Qty += invQty;
                                }
                            }
                            #endregion

                            #region InvTrans
                            var sku = skuALL.FirstOrDefault(x => x.SysId == rd.SkuSysId);
                            var pack = packALL.FirstOrDefault(x => x.SysId == sku.PackSysId);
                            var uom = uomALL.FirstOrDefault(x => x.SysId == rd.UOMSysId);
                            var invTrans = new invtran()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = picking.WarehouseSysId,
                                DocOrder = receipt.ReceiptOrder,
                                DocSysId = receipt.SysId,
                                DocDetailSysId = rd.SysId,
                                SkuSysId = rd.SkuSysId,
                                SkuCode = sku.SkuCode,
                                TransType = InvTransType.Inbound,
                                SourceTransType = InvSourceTransType.Shelve,
                                Qty = invQty,
                                Loc = loc.Loc,
                                Lot = rd.ToLot,
                                Lpn = "",
                                ToLoc = loc.Loc,
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
                                CreateBy = picking.CurrentUserId,
                                CreateDate = DateTime.Now,
                                CreateUserName = picking.CurrentDisplayName,
                                UpdateBy = picking.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                UpdateUserName = picking.CurrentDisplayName
                            };
                            invTransList.Add(invTrans);

                            #endregion
                        }

                        if (pickingQty > 0)
                        {
                            throw new Exception("领料分拣数量不能大于待上架数量");
                        }

                        #region 领料分拣记录
                        var pickingrecords = new pickingrecords()
                        {
                            SysId = Guid.NewGuid(),
                            PickingSysId = pickingModel.SysId,
                            WareHouseSysId = picking.WarehouseSysId,
                            ReceiptSysId = receipt.SysId,
                            ReceiptOrder = receipt.ReceiptOrder,
                            SkuSysId = info.SkuSysId,
                            PickingNumber = pickingNumber,
                            Qty = info.Qty,
                            PickingUserId = picking.PickingUserId,
                            PickingUserName = picking.PickingUserName.Trim(),
                            PickingDate = picking.PickingDate,
                            Remark = "",
                            CreateBy = picking.CurrentUserId,
                            CreateDate = DateTime.Now,
                            CreateUserName = picking.CurrentDisplayName,
                            UpdateBy = picking.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            UpdateUserName = picking.CurrentDisplayName
                        };
                        pickingRecordsList.Add(pickingrecords);

                        #endregion
                    }

                    PickingMaterialSaveChange(receiptDetails, invLotLocLpnList, invLotList, invSkuLocList, invTransList, updateInventoryDtos, pickingRecordsList, pickingModel);
                }
                else
                {
                    throw new Exception("领料明细不能为空!");
                }

                rsp.IsSuccess = true;
                rsp.ErrorMessage = string.Format("领料分拣处理完成，单号：{0}", receiptOrder);
            }
            catch (Exception ex)
            {
                var message = string.Format("领料分拣处理失败，失败原因：{0}", ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }

            return rsp;
        }

        [UnitOfWork(isTransactional: false)]
        private void PickingMaterialSaveChange(List<receiptdetail> receiptDetails, List<invlotloclpn> invLotLocLpnList, List<invlot> invLotList, List<invskuloc> invSkuLocList, List<invtran> invTranList, List<UpdateInventoryDto> updateInventoryDtos, List<pickingrecords> pickingrecordsList, picking pickingModel)
        {
            try
            {

                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    foreach (var info in receiptDetails)
                    {
                        _crudRepository.Update(info);
                    }
                    _wmsSqlRepository.BatchInsertInvLotLocLpn(invLotLocLpnList);
                    _wmsSqlRepository.BatchInsertInvLot(invLotList);
                    _wmsSqlRepository.BatchInsertInvSkuLoc(invSkuLocList);
                    //领料分拣执行上架方法
                    _wmsSqlRepository.UpdateInventoryQtyByShelves(updateInventoryDtos);
                    _wmsSqlRepository.BatchInsertInvTrans(invTranList);

                    //领料分拣记录
                    _crudRepository.Insert(pickingModel);
                    _wmsSqlRepository.BatchInsertPickingRecords(pickingrecordsList);

                    _crudRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("数据重复并发提交，请刷新后重试");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取领料记录
        /// </summary>
        /// <param name="pickingQuery"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public Pages<PickingMaterialListDto> GetPickingMaterialList(PickingMaterialQuery pickingQuery)
        {
            _crudRepository.ChangeDB(pickingQuery.WarehouseSysId);
            return _crudRepository.GetPickingMaterialList(pickingQuery);
        }
        #endregion

        public CheckDuplicateSNDto CheckDuplicateSN(List<string> snList, string type, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            CheckDuplicateSNDto response = new CheckDuplicateSNDto();
            if (type.Equals("Receive", StringComparison.OrdinalIgnoreCase))
            {
                var query = _crudRepository.GetQuery<receiptsn>(x => snList.Contains(x.SN) && x.WarehouseSysId == warehouseSysId);
                if (query.Count() > 0)
                {
                    response.DuplicateList = query.Select(p => p.SN).ToList();
                }
            }
            else if (type.Equals("Outbound", StringComparison.OrdinalIgnoreCase))
            {
                var query = _crudRepository.GetQuery<receiptsn>(x => snList.Contains(x.SN) && x.WarehouseSysId == warehouseSysId);
                if (query.Count() > 0)
                {
                    var outboundQuery = query.Where(p => p.Status == (int)ReceiptSNStatus.Outbound);
                    response.OutboundList = outboundQuery.Select(p => p.SN).ToList();

                    var existsList = query.Select(p => p.SN).ToList();
                    response.NotExistsList = snList.Where(p => !existsList.Contains(p)).ToList();

                    response.NormalList = existsList.Where(p => !response.OutboundList.Contains(p)).ToList();
                }
                else
                {
                    response.NotExistsList = snList;
                }
            }

            return response;
        }

        #region 批次采集
        /// <summary>
        /// 根据商品获取批次采集相关信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public ReceiptCollectionLotViewDto GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var receiptCollectionLotViewDto = new ReceiptCollectionLotViewDto();
            try
            {
                var receiptDetailViewList = _crudRepository.GetReceiptDetailCollectionLotViewList(request);
                if (receiptDetailViewList != null && receiptDetailViewList.Any())
                {
                    receiptCollectionLotViewDto = receiptDetailViewList.FirstOrDefault().JTransformTo<ReceiptCollectionLotViewDto>();
                    receiptCollectionLotViewDto.TotalReceivedQty = receiptDetailViewList.Sum(x => x.DisplayReceivedQty);
                    var fromReceiptDetail = receiptDetailViewList.Find(x => x.IsDefaultLot == true);
                    receiptCollectionLotViewDto.NoCollectionQty = fromReceiptDetail != null ? fromReceiptDetail.DisplayReceivedQty : 0;
                    receiptCollectionLotViewDto.ReceiptDetailLotViewDtoList = receiptDetailViewList.Where(x => x.IsDefaultLot == false).ToList();
                    return receiptCollectionLotViewDto;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 保存批次采集
        /// </summary>
        /// <param name="receiptCollectionLotDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public ReceiptDetailResponseDto SaveReceiptDetailLot(ReceiptCollectionLotDto receiptCollectionLotDto)
        {
            _crudRepository.ChangeDB(receiptCollectionLotDto.WarehouseSysId);
            var rsp = new ReceiptDetailResponseDto() { IsSuccess = false };
            try
            {
                if (receiptCollectionLotDto != null && receiptCollectionLotDto.LotTemplateValueDtos != null && receiptCollectionLotDto.LotTemplateValueDtos.Any())
                {
                    sku sku = null;    //商品信息
                    List<receiptdetail> receiptDetailList = null;     //收货明细
                    var receiptDetails = new List<receiptdetail>();     //组织收货批次明细
                    var addReceiptDetails = new List<receiptdetail>();     //需要新增的收货批次明细
                    var updateReceiptDetails = new List<receiptdetail>();     //需要修改的收货批次明细

                    #region 判断单据
                    var receipt = _crudRepository.Get<receipt>(receiptCollectionLotDto.ReceiptSysId);
                    if (receipt == null)
                    {
                        throw new Exception("收货单不存在");
                    }
                    if (receipt.Status != (int)ReceiptStatus.Received)
                    {
                        throw new Exception("收货单状态必须为收货完成才能采集批次");
                    }

                    var purchase = _crudRepository.GetQuery<purchase>(x => x.WarehouseSysId == receiptCollectionLotDto.WarehouseSysId && x.PurchaseOrder == receipt.ExternalOrder).FirstOrDefault();
                    if (purchase == null)
                    {
                        throw new Exception("采购单不存在!");
                    }
                    #endregion

                    #region 根据UPC获取对应商品信息
                    receiptDetailList = _crudRepository.GetQuery<receiptdetail>(x => x.ReceiptSysId == receiptCollectionLotDto.ReceiptSysId).ToList();
                    if (receiptDetailList == null || receiptDetailList.Count == 0)
                    {
                        throw new Exception("收货明细不存在，请检查");
                    }

                    if (receiptCollectionLotDto.SkuSysId.HasValue)
                    {
                        sku = _crudRepository.GetQuery<sku>(x => x.SysId == receiptCollectionLotDto.SkuSysId).FirstOrDefault();
                    }
                    else
                    {
                        var skuList = _crudRepository.GetQuery<sku>(x => x.UPC == receiptCollectionLotDto.UPC).ToList();

                        var query = from a in receiptDetailList
                                    join b in skuList on a.SkuSysId equals b.SysId
                                    select b;

                        sku = query.FirstOrDefault();
                    }
                    #endregion

                    #region 判断商品，未采集批次收货明细是否存在
                    if (sku == null)
                    {
                        throw new Exception("商品不存在");
                    }
                    var fromReceiptDetail = receiptDetailList.Find(x => x.SkuSysId == sku.SysId && x.IsDefaultLot == true);
                    if (fromReceiptDetail == null)
                    {
                        throw new Exception("未采集批次的收货明细不存在");
                    }
                    if (fromReceiptDetail.ShelvesQty > 0)
                    {
                        throw new Exception("已存在上架数量，无法采集批次");
                    }
                    #endregion

                    var skuPackList = _packageRepository.GetSkuPackageList(new List<Guid> { sku.SysId });
                    foreach (var info in receiptCollectionLotDto.LotTemplateValueDtos)
                    {
                        if (fromReceiptDetail.ReceivedQty <= 0)
                        {
                            throw new Exception("批次数量不能大于剩余未采集数量，请刷新后重试");
                        }

                        #region 新收货批次明细
                        var receiptDetail = new receiptdetail();
                        receiptDetail.SysId = Guid.NewGuid();
                        receiptDetail.ReceiptSysId = fromReceiptDetail.ReceiptSysId;
                        receiptDetail.SkuSysId = fromReceiptDetail.SkuSysId;
                        receiptDetail.Status = fromReceiptDetail.Status;
                        receiptDetail.RejectedQty = fromReceiptDetail.RejectedQty;
                        receiptDetail.Remark = fromReceiptDetail.Remark;
                        receiptDetail.UOMSysId = fromReceiptDetail.UOMSysId;
                        receiptDetail.PackSysId = fromReceiptDetail.PackSysId;
                        receiptDetail.ToLoc = fromReceiptDetail.ToLoc;
                        receiptDetail.ToLpn = fromReceiptDetail.ToLpn;
                        receiptDetail.LotAttr01 = fromReceiptDetail.LotAttr01;
                        receiptDetail.LotAttr02 = fromReceiptDetail.LotAttr02;
                        receiptDetail.LotAttr03 = fromReceiptDetail.LotAttr03;
                        receiptDetail.LotAttr08 = fromReceiptDetail.LotAttr08;
                        receiptDetail.LotAttr09 = fromReceiptDetail.LotAttr09;
                        receiptDetail.ReceivedDate = fromReceiptDetail.ReceivedDate;
                        receiptDetail.Price = fromReceiptDetail.Price;
                        receiptDetail.IsMustLot = fromReceiptDetail.IsMustLot;

                        #region 原材料单位数量转换 
                        var requestQty = info.Qty;
                        var skuInfo = skuPackList.Where(x => x.SkuSysId == sku.SysId).FirstOrDefault();
                        if (skuInfo == null)
                        {
                            throw new Exception("商品编号：" + sku.SkuCode + "，商品信息不存在");
                        }
                        skuInfo.Flag = (int)ReceiptConvert.ToMaterial;
                        skuInfo.UnitQty = info.Qty;
                        _packageAppService.GetSkuConversionQty(ref skuInfo);
                        receiptDetail.ExpectedQty = skuInfo.BaseQty;
                        receiptDetail.ReceivedQty = skuInfo.BaseQty;
                        info.ReceivedQty = skuInfo.BaseQty;
                        #endregion

                        receiptDetail.IsDefaultLot = false;
                        receiptDetail.LotAttr04 = info.LotValue04;
                        receiptDetail.LotAttr05 = info.LotValue05;
                        receiptDetail.LotAttr06 = info.LotValue06;
                        receiptDetail.LotAttr07 = info.LotValue07;
                        receiptDetail.ExternalLot = info.ExternalLot;
                        receiptDetail.ProduceDate = info.ProduceDate;
                        if (info.ExpiryDate.HasValue)
                        {
                            receiptDetail.ExpiryDate = info.ExpiryDate;
                        }
                        else
                        {
                            if (receiptDetail.ProduceDate.HasValue && sku.ShelfLife.HasValue)
                            {
                                receiptDetail.ExpiryDate = receiptDetail.ProduceDate.Value.AddDays(sku.DaysToExpire.Value);
                            }
                        }

                        receiptDetail.ShelvesStatus = (int)ShelvesStatus.NotOnShelves;
                        receiptDetail.ShelvesQty = 0;
                        receiptDetail.CreateBy = receiptCollectionLotDto.CurrentUserId;
                        receiptDetail.CreateUserName = receiptCollectionLotDto.CurrentDisplayName;
                        receiptDetail.CreateDate = DateTime.Now;
                        receiptDetail.UpdateBy = receiptCollectionLotDto.CurrentUserId;
                        receiptDetail.UpdateDate = DateTime.Now;
                        receiptDetail.UpdateUserName = receiptCollectionLotDto.CurrentDisplayName;
                        receiptDetail.TS = Guid.NewGuid();
                        receiptDetails.Add(receiptDetail);
                        #endregion

                        #region 收货时的原始明细
                        fromReceiptDetail.ExpectedQty -= receiptDetail.ExpectedQty;
                        fromReceiptDetail.ReceivedQty -= receiptDetail.ReceivedQty;
                        fromReceiptDetail.UpdateBy = receiptCollectionLotDto.CurrentUserId;
                        fromReceiptDetail.UpdateUserName = receiptCollectionLotDto.CurrentDisplayName;
                        fromReceiptDetail.UpdateDate = DateTime.Now;
                        fromReceiptDetail.TS = Guid.NewGuid();
                        #endregion
                    }

                    if (fromReceiptDetail.ReceivedQty < 0)
                    {
                        throw new Exception("批次数量不能大于剩余未采集数量，请刷新后重试");
                    }

                    #region 检测是否存在相同批次  
                    var checkLots = _wmsSqlRepository.GetToLotByReceiptDetail(receiptDetails, purchase.SysId, receiptCollectionLotDto.WarehouseSysId);
                    #endregion

                    #region 赋值批次号
                    foreach (var receiptDetail in receiptDetails)
                    {
                        var checkLot = checkLots.FirstOrDefault(p => p.SysId == receiptDetail.SysId && p.ReceiptSysId == receiptDetail.ReceiptSysId && p.CheckLotSysId.HasValue);
                        if (checkLot != null)
                        {
                            var updateReceiptDetail = receiptDetailList.Find(x => x.SysId == checkLot.CheckLotSysId);
                            if (updateReceiptDetail != null)
                            {
                                updateReceiptDetail.ExpectedQty += receiptDetail.ExpectedQty;
                                updateReceiptDetail.ReceivedQty += receiptDetail.ReceivedQty;
                                updateReceiptDetail.UpdateBy = receiptCollectionLotDto.CurrentUserId;
                                updateReceiptDetail.UpdateDate = DateTime.Now;
                                updateReceiptDetail.UpdateUserName = receiptCollectionLotDto.CurrentDisplayName;
                                updateReceiptDetail.TS = Guid.NewGuid();
                                rsp.ToLot = updateReceiptDetail.ToLot;
                                rsp.LotAttr01 = updateReceiptDetail.LotAttr01;
                                updateReceiptDetails.Add(updateReceiptDetail);
                            }
                            else
                            {
                                receiptDetail.ToLot = checkLot.ToLot;
                                rsp.ToLot = receiptDetail.ToLot;
                                rsp.LotAttr01 = receiptDetail.LotAttr01;
                                addReceiptDetails.Add(receiptDetail);
                            }
                        }
                        else
                        {
                            addReceiptDetails.Add(receiptDetail);
                        }
                    }

                    var noLotDetails = addReceiptDetails.Where(p => string.IsNullOrEmpty(p.ToLot));
                    if (noLotDetails.Any())
                    {
                        var lots = _baseAppService.GetNumber(PublicConst.GenNextNumberLot, noLotDetails.Count());
                        foreach (var receiptDetail in addReceiptDetails)
                        {
                            if (string.IsNullOrEmpty(receiptDetail.ToLot))
                            {
                                receiptDetail.ToLot = lots[0];
                                rsp.ToLot = receiptDetail.ToLot;
                                rsp.LotAttr01 = receiptDetail.LotAttr01;
                                lots.RemoveAt(0);
                            }
                        }
                    }
                    #endregion

                    //执行批次采集sql
                    SaveReceiptDetailLotSaveChange(fromReceiptDetail, addReceiptDetails, updateReceiptDetails);
                }
                else
                {
                    throw new Exception("批次采集传入数据为空");
                }

                rsp.IsSuccess = true;
            }
            catch (Exception ex)
            {
                var message = string.Format("批次采集保存失败，失败原因：{0}", ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }

            return rsp;
        }

        [UnitOfWork(isTransactional: false)]
        private void SaveReceiptDetailLotSaveChange(receiptdetail fromReceiptDetail, List<receiptdetail> toReceiptDetailList, List<receiptdetail> updateReceiptDetailList)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    if (fromReceiptDetail != null)
                    {
                        if (fromReceiptDetail.ReceivedQty == 0)
                        {
                            _crudRepository.Delete(fromReceiptDetail);
                        }
                        else
                        {
                            _crudRepository.Update(fromReceiptDetail);
                        }
                    }

                    if (toReceiptDetailList != null && toReceiptDetailList.Count > 0)
                    {
                        _wmsSqlRepository.BatchInsertReceiptDetail(toReceiptDetailList);
                    }

                    if (updateReceiptDetailList != null && updateReceiptDetailList.Count > 0)
                    {
                        foreach (var item in updateReceiptDetailList)
                        {
                            _crudRepository.Update(item);
                        }
                    }

                    _crudRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion
    }
}
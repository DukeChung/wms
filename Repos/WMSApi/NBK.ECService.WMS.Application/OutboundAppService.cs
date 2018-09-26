using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO.MQ.Log;
using NBK.ECService.WMS.Utility.Enum.Log;
using NBK.ECService.WMS.Utility.RabbitMQ;
using NBK.ECService.WMS.Utility.Message;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using Abp.Domain.Uow;
using NBK.ECService.WMS.DTO.MQ;
using NBK.ECService.WMS.Utility.Redis;
using NBK.ECService.WMS.DTO.ThirdParty;
using NBK.ECService.WMS.Application.Check;
using NBK.ECService.WMS.DTO.MQ.OrderRule;
using System.Transactions;
using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.DTO.ThirdParty.OutboundReturn;

namespace NBK.ECService.WMS.Application
{
    public class OutboundAppService : WMSApplicationService, IOutboundAppService
    {
        private ICrudRepository _crudRepository = null;
        private IOutboundRepository _outboundRepository = null;
        //private IReceiptRepository _receiptRepository = null;
        private IVanningRepository _vanningRepository = null;
        private IInventoryRepository _inventoryRepository = null;
        private IThirdPartyAppService _thirdPartyAppService = null;
        private IWMSSqlRepository _wmsSqlRepository = null;
        private IPrePackCrudRepository _prePackCrudRepository = null;
        private ITransferInventoryAppService _transferInventoryAppService = null;
        private IBaseAppService _baseAppService = null;
        private IPickDetailAppService _pickDetailAppService = null;
        private IOutboundTransferOrderRepository _outboundTransferOrderRepository = null;
        private IPreBulkPackRepository _preBulkPackRepository;
        private IRedisAppService _redisAppService = null;

        public OutboundAppService(IOutboundRepository outboundRepository, IVanningRepository vanningRepository,
            IThirdPartyAppService thirdPartyAppService, IInventoryRepository inventoryRepository,
            IWMSSqlRepository wmsSqlRepository, IPrePackCrudRepository prePackCrudRepository, ITransferInventoryAppService transferInventoryAppService, ICrudRepository crudRepository, IBaseAppService baseAppService, IPreBulkPackRepository preBulkPackRepository,
            IPickDetailAppService pickDetailAppService, IOutboundTransferOrderRepository outboundTransferOrderRepository, IRedisAppService redisAppService)
        {

            _inventoryRepository = inventoryRepository;
            _outboundRepository = outboundRepository;
            _vanningRepository = vanningRepository;
            this._thirdPartyAppService = thirdPartyAppService;
            this._wmsSqlRepository = wmsSqlRepository;
            _prePackCrudRepository = prePackCrudRepository;
            _crudRepository = crudRepository;
            _transferInventoryAppService = transferInventoryAppService;
            this._baseAppService = baseAppService;
            this._preBulkPackRepository = preBulkPackRepository;
            this._pickDetailAppService = pickDetailAppService;
            this._outboundTransferOrderRepository = outboundTransferOrderRepository;
            this._redisAppService = redisAppService;
        }


        /// <summary>
        /// 获取发货箱信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<ScanDeliveryDto> GetDeliveryBoxByOrderNumber(string type, string orderNumber, Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var list = _outboundRepository.GetDeliveryBoxByByOrderNumber(type, orderNumber, wareHouseSysId);
            var checkOutboundStatus = list.Where(x => x.OutboundStatus == (int)OutboundStatus.Delivery);
            if (checkOutboundStatus.Any())
            {
                var msg = string.Empty;
                foreach (var info in checkOutboundStatus)
                {
                    msg += "装箱单：" + info.VanningOrder + ";";
                }
                msg += "已经发货,无法再次发货!";
                throw new Exception(msg);
            }
            return list;
        }


        [UnitOfWork(isTransactional: false)]
        public void SaveDeliveryByVanningSysIdSaveChange(List<vanningdetail> vanningDetailList, List<outbound> outboundList, List<List<UpdateInventoryDto>> updateInventoryDtosList)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    foreach (var info in vanningDetailList)
                    {
                        _outboundRepository.Update(info);
                    }
                    foreach (var info in outboundList)
                    {
                        _outboundRepository.Update(info);
                    }
                    foreach (var info in updateInventoryDtosList)
                    {
                        _wmsSqlRepository.UpdateInventoryQtyByPickedQty(info);
                    }
                    _outboundRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                //scope.
            }
        }


        /// <summary>
        /// 保存发货信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public void SaveDeliveryByVanningSysId(List<Guid> vanningSysIds, string currentUserName, int currentUserId,
            Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            if (vanningSysIds.Any())
            {
                var outboundList = new List<outbound>();
                var deliveryHandoverGroupDto = new List<DeliveryHandoverGroupDto>();
                var vanningDetailList = new List<vanningdetail>();
                var updateInventoryDtosList = new List<List<UpdateInventoryDto>>();
                foreach (var sysId in vanningSysIds)
                {
                    var businessLogDto = new BusinessLogDto();
                    var outboundOrder = string.Empty;
                    try
                    {
                        businessLogDto.doc_sysId = sysId;
                        businessLogDto.doc_order = "装箱明细主键（vanningdetail表）";
                        businessLogDto.access_log_sysId = Guid.NewGuid();
                        businessLogDto.request_json = JsonConvert.SerializeObject(vanningSysIds);
                        businessLogDto.user_id = currentUserId.ToString();
                        businessLogDto.user_name = currentUserName;
                        businessLogDto.business_name = BusinessName.Deliver.ToDescription();
                        businessLogDto.business_type = BusinessType.Outbound.ToDescription();
                        businessLogDto.business_operation = PublicConst.LogSaveDelivery;
                        businessLogDto.descr = "[old_json记录 扣减库存记录 , new_json记录 交接数据 记录]";
                        businessLogDto.flag = true;

                        #region 检查拣货记录是否都已经全部装箱
                        var vanning = _outboundRepository.Get<vanning>(sysId);

                        if (vanning == null)
                        {
                            outboundList = new List<outbound>();
                            throw new Exception("装箱单不存在");
                        }
                        if (vanning.Status != (int)VanningStatus.Finish)
                        {
                            outboundList = new List<outbound>();
                            throw new Exception("装箱单状态不为装箱完成，不允许发货");
                        }

                        var pickDetailList =
                            _outboundRepository.GetQuery<pickdetail>(
                                x =>
                                    x.OutboundSysId == vanning.OutboundSysId &&
                                    x.Status != (int)PickDetailStatus.Finish && x.Status != (int)PickDetailStatus.Cancel).ToList();
                        if (pickDetailList.Any())
                        {
                            outboundList = new List<outbound>();
                            throw new Exception("装箱单:" + vanning.VanningOrder + " 没有将拣货货品全部装箱,拣货单号：" + pickDetailList.FirstOrDefault().PickDetailOrder + ". 请核对");
                        }
                        #endregion
                        var detailList = _outboundRepository.GetQuery<vanningdetail>(x => x.VanningSysId == sysId && x.Status != (int)VanningStatus.Cancel).ToList();

                        #region  交接单数据创建 

                        foreach (var detail in detailList)
                        {
                            var handoverGroup =
                                deliveryHandoverGroupDto.Where(x => x.CarrierSysId == detail.CarrierSysId);
                            if (!handoverGroup.Any())
                            {

                                var orderNumber = _baseAppService.GetNumber(PublicConst.GenNextNumberHandoverGroup);
                                deliveryHandoverGroupDto.Add(new DeliveryHandoverGroupDto()
                                {
                                    VanningDetailSysId = detail.SysId,
                                    CarrierSysId = detail.CarrierSysId,
                                    HandoverGroupOrder = orderNumber
                                });
                                detail.HandoverGroupOrder = orderNumber;
                                detail.HandoverCreateBy = currentUserId;
                                detail.HandoverCreateDate = DateTime.Now;
                                detail.UpdateBy = currentUserId;
                                detail.UpdateUserName = currentUserName;
                                detail.UpdateDate = DateTime.Now;
                            }
                            else
                            {
                                detail.HandoverGroupOrder = handoverGroup.FirstOrDefault().HandoverGroupOrder;
                                detail.HandoverCreateBy = currentUserId;
                                detail.UpdateUserName = currentUserName;
                                detail.HandoverCreateDate = DateTime.Now;
                                detail.UpdateBy = currentUserId;
                                detail.UpdateDate = DateTime.Now;
                            }
                            vanningDetailList.Add(detail);

                        }


                        #endregion

                        var vanningDeliveryList = _vanningRepository.GetVanningPickDetailDtoByVanningSysId(sysId,
                            wareHouseSysId);
                        var updateInventoryDtos = new List<UpdateInventoryDto>();
                        if (vanningDeliveryList.Any())
                        {
                            var totalShippedQty = 0;
                            //发货是基于一个订单进行操作的，装箱也是基于一个订单操作的
                            var outBoundSysId = vanningDeliveryList.FirstOrDefault().OutboundSysId;
                            var outBound = _outboundRepository.Get<outbound>(outBoundSysId.Value);
                            if (outBound.Status == (int)OutboundStatus.Delivery)
                            {
                                outboundList = new List<outbound>();
                                throw new Exception("出库单:" + outBound.OutboundOrder + ",无法重复发货");
                            }
                            if (outBound.IsReturn == (int)OutboundReturnStatus.B2CReturn)
                            {
                                outboundList = new List<outbound>();
                                throw new Exception("出库单:" + outBound.OutboundOrder + "已经被B2C退货，无法发货！");
                            }

                            var outBoundDetailList =
                                _outboundRepository.GetQuery<outbounddetail>(x => x.OutboundSysId == outBoundSysId.Value)
                                    .ToList();

                            #region 生成库存扣减记录
                            foreach (var info in vanningDeliveryList)
                            {
                                totalShippedQty += info.Qty.Value;
                                var invlotloclpn = _outboundRepository.Get<invlotloclpn>(info.InvLotLocLpnSysId.Value);
                                if (invlotloclpn.PickedQty < info.Qty.Value)
                                {
                                    outboundList = new List<outbound>();
                                    throw new Exception("invLotLocLpn异常");
                                }
                                updateInventoryDtos.Add(new UpdateInventoryDto()
                                {
                                    InvLotLocLpnSysId = info.InvLotLocLpnSysId.Value,
                                    InvLotSysId = info.InvLotSysId.Value,
                                    InvSkuLocSysId = info.InvSkuLocSysId.Value,
                                    Qty = info.Qty.Value,
                                    CurrentDisplayName = currentUserName,
                                    CurrentUserId = currentUserId
                                });


                                foreach (var item in outBoundDetailList)
                                {
                                    if (item.SysId == info.OutboundDetailSysId)
                                    {
                                        if (!item.ShippedQty.HasValue)
                                        {
                                            item.ShippedQty = 0;
                                        }
                                        item.ShippedQty += info.Qty;
                                    }
                                    if (item.ShippedQty == item.Qty)
                                    {
                                        item.Status = (int)OutboundDetailStatus.Delivery;
                                    }
                                }
                            }
                            #endregion

                            var outboundDetailStatusCount =
                                outBoundDetailList.Where(x => x.Status == (int)OutboundDetailStatus.Delivery).ToList();
                            outBound.TotalShippedQty = totalShippedQty;
                            outBound.ActualShipDate = DateTime.Now;
                            outBound.TS = Guid.NewGuid();
                            outBound.UpdateBy = currentUserId;
                            outBound.UpdateDate = DateTime.Now;
                            outBound.UpdateUserName = currentUserName;
                            outBound.OutboundMethod = InvSourceTransType.Shipment;
                            outboundOrder = outBound.OutboundOrder;
                            if (outboundDetailStatusCount.Count() == outBoundDetailList.Count())
                            {
                                outBound.Status = (int)OutboundStatus.Delivery;
                            }
                            outboundList.Add(outBound);
                            updateInventoryDtosList.Add(updateInventoryDtos);

                            businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);
                            businessLogDto.new_json = JsonConvert.SerializeObject(detailList);

                            #region 移仓类型发货完成生成入库单
                            if (outBound.OutboundType == (int)OutboundType.TransferInventory)
                            {
                                var transferInv = _crudRepository.GetQuery<transferinventory>(p => p.TransferInventoryOrder == outBound.ExternOrderId).FirstOrDefault();
                                var transferInventoryDto = new MQTransferInventoryDto()
                                {
                                    TransferInventoryOrder = outBound.ExternOrderId,
                                    Status = (int)TransferInventoryStatus.Delivery,
                                    CurrentUserId = currentUserId,
                                    CurrentDisplayName = currentUserName,
                                    FromWareHouseSysId = transferInv.FromWareHouseSysId,
                                    TransferOutboundDate = transferInv.TransferOutboundDate,
                                    AuditingDate = transferInv.AuditingDate,
                                    AuditingBy = transferInv.AuditingBy,
                                    AuditingName = transferInv.AuditingName,
                                    WarehouseSysId = wareHouseSysId,
                                    ToWareHouseSysId = transferInv.ToWareHouseSysId,
                                    //增加渠道信息
                                    Channel = transferInv.Channel
                                };
                                transferInventoryDto.transferinventorydetails = transferInv.transferinventorydetails.JTransformTo<TransferInventoryDetailDto>();
                                transferInventoryDto.PurchaseSysId = Guid.NewGuid();
                                if (outBound.OutboundOrder.IndexOf("OB") > -1)
                                {
                                    transferInventoryDto.PurchaseOrder = outBound.OutboundOrder.Replace("OB", "PO0");
                                }
                                else
                                {
                                    transferInventoryDto.PurchaseOrder = "PO0" + outBound.OutboundOrder;
                                }
                                var processDto = new MQProcessDto<MQTransferInventoryDto>()
                                {
                                    BussinessSysId = outBound.SysId,
                                    BussinessOrderNumber = outBound.OutboundOrder,
                                    Descr = "",
                                    CurrentUserId = currentUserId,
                                    CurrentDisplayName = currentUserName,
                                    WarehouseSysId = wareHouseSysId,
                                    BussinessDto = transferInventoryDto
                                };

                                _transferInventoryAppService.UpdateTransferInventoryStatus(transferInventoryDto);

                                var response = ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/MQ/OrderManagement/TransferInventory/CreateTransferInventoryReceipt", new CoreQuery(), transferInventoryDto);
                                if (response != null && response.ResponseResult != null && response.ResponseResult.IsSuccess)
                                {
                                    // 生成完入库单更新一仓单数据
                                    transferInv.TransferPurchaseSysId = transferInventoryDto.PurchaseSysId;
                                    transferInv.TransferPurchaseOrder = transferInventoryDto.PurchaseOrder;
                                    transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                                    transferInv.UpdateDate = DateTime.Now;
                                    transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                                    _crudRepository.Update(transferInv);
                                }
                                else
                                {
                                    throw new Exception("创建入库单失败");
                                }

                                //_transferInventoryAppService.CreateTransferInventoryReceipt(transferInventoryDto);
                                //RabbitWMS.SetRabbitMQAsync(RabbitMQType.TransferInventoryInbound, processDto);
                            }
                            #endregion
                        }
                        else
                        {
                            outboundList = new List<outbound>();
                            throw new Exception("系统库存异常,无法进行正常发货!");
                        }

                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        outboundList = new List<outbound>();
                        var message = string.Format("扫描出库处理失败，订单号：{0}，失败原因：数据重复并发提交", outboundOrder);
                        throw new Exception(message);
                    }
                    catch (Exception ex)
                    {
                        outboundList = new List<outbound>();
                        businessLogDto.descr += ex.Message;
                        businessLogDto.flag = false;
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        //发送MQ
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
                    }
                }
                SaveDeliveryByVanningSysIdSaveChange(vanningDetailList, outboundList, updateInventoryDtosList);
                if (outboundList.Any())
                {
                    foreach (var info in outboundList)
                    {

                        #region 出库回写
                        if (!string.IsNullOrEmpty(info.ExternOrderId) && info.OutboundType != (int)OutboundType.TransferInventory)
                        {
                            var crpe = new CommonResponse();
                            try
                            {
                                crpe = _thirdPartyAppService.InsertOutStock(info.SysId, currentUserName,
                                    currentUserId);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        #endregion
                    }
                }
            }
            else
            {
                throw new Exception("箱号为空");
            }
        }

        /// <summary>
        /// 出库快进快出业务
        /// </summary>
        /// <param name="outboundBatchDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public string BatchOutboundByFIFO(OutboundBatchDto outboundBatchDto)
        {
            _crudRepository.ChangeDB(outboundBatchDto.WarehouseSysId);
            //Task task = new Task(() =>
            //{
            return OutboundByFIFO(outboundBatchDto);
            //});
            //task.Start();
        }

        /// <summary>
        /// 出库快进快出
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        private string OutboundByFIFO(OutboundBatchDto outboundBatchDto)
        {
            _crudRepository.ChangeDB(outboundBatchDto.WarehouseSysId);
            outboundBatchDto.SysId = Guid.NewGuid();
            outboundBatchDto.TotalQty = 0;
            outboundBatchDto.TotalPrice = 0;
            var outboundDetail = CreateOutboundDetailByFIFO(outboundBatchDto);
            var ountbound = CreateOutboundByFIFO(outboundBatchDto);
            _outboundRepository.Insert(ountbound);
            _outboundRepository.BatchInsert<outbounddetail>(outboundDetail);
            MatchingReceipt(outboundDetail, ountbound, outboundBatchDto.CurrentUserId,
                outboundBatchDto.CurrentDisplayName, outboundBatchDto.WarehouseSysId);
            //写入消息或者发送邮件

            return ountbound.OutboundOrder;
        }

        /// <summary>
        /// 出库匹配入库
        /// </summary>   
        [UnitOfWork(isTransactional: false)]
        public void MatchingReceipt(List<outbounddetail> outboundDetails, outbound outBound, int currentUserId,
            string currentDisplayName, Guid wareHouseSysId)
        {
            _crudRepository.ChangeDB(wareHouseSysId);
            var purchaseList =
                _outboundRepository.GetQuery<purchase>(
                    x => x.Source == PublicConst.ReceiptSource && x.WarehouseSysId == wareHouseSysId).ToList();
            if (!purchaseList.Any())
            {
                throw new Exception("未找到铺货采购单");
            }
            var purchaseOrderList = purchaseList.Select(x => x.PurchaseOrder);
            var receiptList =
                _outboundRepository.GetQuery<receipt>(
                    x => purchaseOrderList.Contains(x.ExternalOrder) && x.WarehouseSysId == wareHouseSysId).ToList();
            if (!receiptList.Any())
            {
                throw new Exception("未找到快进快出入库单");
            }
            var receiptSysIdList = receiptList.Select(x => x.SysId);
            var receiptDetailList =
                _outboundRepository.GetQuery<receiptdetail>(
                    x => receiptSysIdList.Contains(x.ReceiptSysId) && x.ShelvesStatus != (int)ShelvesStatus.Finish)
                    .ToList();
            if (!receiptDetailList.Any())
            {
                throw new Exception("未找到快进快出入库单");
            }
            foreach (var info in outboundDetails)
            {
                var rdList = receiptDetailList.Where(x => x.SkuSysId == info.SkuSysId);
                if (!rdList.Any())
                {
                    throw new Exception("未找到快进快出入库单对应的skuSysId");
                }
                var residualQty = info.Qty.Value;
                var currentQty = 0;
                foreach (var rd in rdList)
                {
                    var receipt = receiptList.FirstOrDefault(x => x.SysId == rd.ReceiptSysId);
                    //屏蔽脏数据
                    //var receiptDetail = _outboundRepository.Get<receiptdetail>(rd.SysId);
                    var receiptDetail = rd;
                    if (receiptDetail.ShelvesStatus == (int)ShelvesStatus.Finish)
                    {
                        continue;
                    }
                    var receiptDetailQty = receiptDetail.ReceivedQty.Value - receiptDetail.ShelvesQty;

                    if (residualQty >= receiptDetailQty)
                    {
                        currentQty = receiptDetailQty;
                        receiptDetail.ShelvesQty = receiptDetail.ShelvesQty + receiptDetailQty;
                        ;
                        residualQty = residualQty - receiptDetailQty;
                    }
                    else if (residualQty < receiptDetail.ReceivedQty)
                    {
                        currentQty = residualQty;
                        receiptDetail.ShelvesQty = receiptDetail.ShelvesQty + residualQty;
                        residualQty = 0;
                    }
                    if (receiptDetail.ReceivedQty == receiptDetail.ShelvesQty)
                    {
                        receiptDetail.ShelvesStatus = (int)ShelvesStatus.Finish;
                    }
                    else
                    {
                        receiptDetail.ShelvesStatus = (int)ShelvesStatus.Shelves;
                    }

                    #region 入库上架交易

                    var rdInvtrans = new invtran()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = receipt.WarehouseSysId,
                        DocOrder = receipt.ReceiptOrder,
                        DocSysId = rd.ReceiptSysId,
                        DocDetailSysId = rd.SysId,
                        SkuSysId = rd.SkuSysId,
                        SkuCode = "",
                        TransType = InvTransType.Inbound,
                        SourceTransType = InvSourceTransType.Shelve,
                        Qty = currentQty,
                        Loc = rd.ToLoc,
                        Lot = rd.ToLot,
                        Lpn = rd.ToLpn,
                        ToLoc = rd.ToLoc,
                        ToLot = rd.ToLot,
                        ToLpn = rd.ToLpn,
                        Status = InvTransStatus.Ok,
                        //LotAttr01 = string.Empty,
                        //LotAttr02 = string.Empty,
                        //LotAttr03 = string.Empty,
                        //LotAttr04 = string.Empty,
                        //LotAttr05 = string.Empty,
                        //LotAttr06 = string.Empty,
                        //LotAttr07 = string.Empty,
                        //LotAttr08 = string.Empty,
                        //LotAttr09 = string.Empty,
                        //ExternalLot = string.Empty,
                        //ProduceDate = new DateTime(),
                        //ExpiryDate = new DateTime(),
                        ReceivedDate = receiptDetail.ReceivedDate,
                        PackSysId = rd.PackSysId.Value,
                        PackCode = "",
                        UOMSysId = rd.UOMSysId.Value,
                        UOMCode = "",
                        CreateBy = currentUserId,
                        CreateUserName = currentDisplayName,
                        CreateDate = DateTime.Now,
                        UpdateBy = currentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = currentDisplayName,
                    };

                    #endregion

                    #region 出库拣货交易

                    var pdInvtrans = new invtran()
                    {
                        SysId = Guid.NewGuid(),
                        WareHouseSysId = outBound.WareHouseSysId,
                        DocOrder = outBound.OutboundOrder,
                        DocSysId = info.OutboundSysId.Value,
                        DocDetailSysId = info.SysId,
                        SkuSysId = info.SkuSysId,
                        SkuCode = "",
                        TransType = InvTransType.Outbound,
                        SourceTransType = InvSourceTransType.Picking,
                        Qty = currentQty * -1,
                        Loc = rd.ToLoc,
                        Lot = rd.ToLot,
                        Lpn = string.Empty,
                        ToLoc = rd.ToLoc,
                        ToLot = rd.ToLot,
                        ToLpn = string.Empty,
                        Status = InvTransStatus.Ok,
                        //LotAttr01 = string.Empty,
                        //LotAttr02 = string.Empty,
                        //LotAttr03 = string.Empty,
                        //LotAttr04 = string.Empty,
                        //LotAttr05 = string.Empty,
                        //LotAttr06 = string.Empty,
                        //LotAttr07 = string.Empty,
                        //LotAttr08 = string.Empty,
                        //LotAttr09 = string.Empty,
                        //ExternalLot = string.Empty,
                        //ProduceDate = new DateTime(),
                        //ExpiryDate = new DateTime(),
                        ReceivedDate = DateTime.Now,
                        PackSysId = info.PackSysId.Value,
                        PackCode = "",
                        UOMSysId = info.UOMSysId.Value,
                        UOMCode = "",
                        CreateBy = currentUserId,
                        CreateUserName = currentDisplayName,
                        CreateDate = DateTime.Now.AddMinutes(1),
                        UpdateBy = currentUserId,
                        UpdateDate = DateTime.Now.AddMinutes(1),
                        UpdateUserName = currentDisplayName,
                    };

                    #endregion

                    _outboundRepository.Insert(rdInvtrans);
                    _outboundRepository.Insert(pdInvtrans);
                    rd.UpdateBy = currentUserId;
                    rd.UpdateDate = DateTime.Now;
                    rd.UpdateUserName = currentDisplayName;
                    _outboundRepository.Update(rd);
                    if (residualQty == 0)
                    {
                        break;
                    }
                }
                if (residualQty != 0)
                {
                    throw new Exception("商品入库数量少于出库数量,无法发货");
                }
            }
        }

        /// <summary>
        /// 先进先出 创建出库单
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        private List<outbounddetail> CreateOutboundDetailByFIFO(OutboundBatchDto outboundBatchDto)
        {
            _crudRepository.ChangeDB(outboundBatchDto.WarehouseSysId);
            List<outbounddetail> outboundDetails = null;
            if (outboundBatchDto.BatchOutboundDetailDtos.Any())
            {
                outboundDetails = new List<outbounddetail>();

                #region 组织创建明细基本信息

                List<Guid> skuSysIdList = outboundBatchDto.BatchOutboundDetailDtos.Select(p => p.SkuSysId).ToList();
                List<sku> skuList = _outboundRepository.GetAllList<sku>(p => skuSysIdList.Contains(p.SysId));
                List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                List<pack> packList = _outboundRepository.GetAllList<pack>(p => packSysIdList.Contains(p.SysId));
                List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                List<uom> uomList = _outboundRepository.GetAllList<uom>(p => fieldUom01List.Contains(p.SysId));

                #endregion

                foreach (var info in outboundBatchDto.BatchOutboundDetailDtos)
                {
                    var sku = skuList.FirstOrDefault(p => p.SysId == info.SkuSysId);
                    var pack = new pack();
                    var uom = new uom();
                    if (sku != null)
                    {
                        pack = packList.FirstOrDefault(p => p.SysId == sku.PackSysId);
                        if (pack != null)
                        {
                            uom = uomList.FirstOrDefault(p => p.SysId == pack.FieldUom01);
                        }
                        else
                        {
                            throw new Exception("未找到对应的商品包装信息");
                        }
                    }
                    else
                    {
                        throw new Exception("未找到对应的SKU信息");
                    }
                    outboundBatchDto.TotalQty += info.Qty;
                    outboundBatchDto.TotalPrice += (info.Qty * sku.SalePrice);

                    outbounddetail outboundDetail = new outbounddetail
                    {
                        SysId = Guid.NewGuid(),
                        OutboundSysId = outboundBatchDto.SysId,
                        SkuSysId = sku.SysId,
                        UOMSysId = uom.SysId,
                        PackSysId = pack.SysId,
                        Qty = info.Qty,
                        ShippedQty = info.Qty,
                        Status = (int)OutboundDetailStatus.Delivery,
                        Price = sku.SalePrice,
                        CreateBy = outboundBatchDto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        CreateUserName = outboundBatchDto.CurrentDisplayName,
                        UpdateBy = outboundBatchDto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = outboundBatchDto.CurrentDisplayName,
                    };
                    outboundDetails.Add(outboundDetail);
                }
            }
            else
            {
                throw new Exception("未找到可以出库的明细信息!");
            }
            return outboundDetails;
        }

        /// <summary>
        /// 创建出库单
        /// </summary>
        /// <param name="outboundBatchDto"></param>
        [UnitOfWork(isTransactional: false)]
        private outbound CreateOutboundByFIFO(OutboundBatchDto outboundBatchDto)
        {
            _crudRepository.ChangeDB(outboundBatchDto.WarehouseSysId);
            var outbound = new outbound
            {
                SysId = outboundBatchDto.SysId,
                //OutboundOrder = _outboundRepository.GenNextNumber(PublicConst.GenNextNumberOutbound),
                OutboundOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberOutbound),
                WareHouseSysId = outboundBatchDto.WarehouseSysId,
                RequestedShipDate = DateTime.Now,
                ActualShipDate = DateTime.Now,
                OutboundType = (int)OutboundType.FIFO,
                Status = (int)OutboundStatus.Delivery,
                OutboundDate = DateTime.Now,
                ConsigneeArea = "",
                ConsigneeCity = "",
                ConsigneeProvince = "",
                ConsigneeTown = "",
                ConsigneeVillage = "",
                ConsigneeName = outboundBatchDto.ConsigneeName,
                ConsigneeAddress = outboundBatchDto.ConsigneeAddress,
                ConsigneePhone = outboundBatchDto.ConsigneePhone,
                OutboundGroup = outboundBatchDto.OutboundGroup,
                PostalCode = PublicConst.PostCode,
                TotalQty = outboundBatchDto.TotalQty,
                TotalShippedQty = outboundBatchDto.TotalQty,
                TotalPrice = outboundBatchDto.TotalPrice,
                CreateBy = outboundBatchDto.CurrentUserId,
                CreateDate = DateTime.Now,
                CreateUserName = outboundBatchDto.CurrentDisplayName,
                UpdateBy = outboundBatchDto.CurrentUserId,
                UpdateDate = DateTime.Now,
                UpdateUserName = outboundBatchDto.CurrentDisplayName,
                AuditingDate = DateTime.Now
            };

            return outbound;
        }

        /// <summary>
        /// 根据条件获取出库单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public OutboundViewDto GetOutboundOrderByOrderId(OutboundQuery outboundQuery)
        {
            _crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            var query =
                _outboundRepository.GetQuery<outbound>(
                    x =>
                        x.OutboundOrder == outboundQuery.OutboundOrder &&
                        x.WareHouseSysId == outboundQuery.WarehouseSysId);

            if (outboundQuery.WaitPickSearch)
            {
                query =
                    query.Where(
                        x => x.Status == (int)OutboundStatus.New || x.Status == (int)OutboundStatus.PartAllocation);
            }

            return query.FirstOrDefault().JTransformTo<OutboundViewDto>();
        }

        /// <summary>
        /// 快速发货(接收到数据发送到消息队列)
        /// </summary>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse OutboundQuickDeliverySendMQ(OutboundQuickDeliveryDto outboundQuickDeliveryDto)
        {
            _crudRepository.ChangeDB(outboundQuickDeliveryDto.WarehouseSysId);
            var rsp = new CommonResponse();

            try
            {
                var outboundRule = _crudRepository.GetQuery<outboundrule>(x => x.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId).FirstOrDefault();

                if (outboundRule != null)
                {
                    if (outboundRule.DeliveryIsAsyn == true)
                    {
                        var outbound = _crudRepository.Get<outbound>(outboundQuickDeliveryDto.SysId.Value);
                        var bussinessProcessLogDto = new AsynBussinessProcessLogDto<OutboundQuickDeliveryDto>()
                        {
                            BussinessSysId = (Guid)outboundQuickDeliveryDto.SysId,
                            BussinessOrderNumber = outbound.OutboundOrder,
                            BussinessType = "OutboundQuickDelivery",
                            BussinessTypeName = "快速出库",
                            Descr = string.Format("用户:{0},触发了快速发货,单号:{1}", outboundQuickDeliveryDto.CurrentDisplayName, outboundQuickDeliveryDto.OutboundOrder),
                            CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                            CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName,
                            WarehouseSysId = outboundQuickDeliveryDto.WarehouseSysId,
                            BussinessDto = outboundQuickDeliveryDto
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.OutboundQuickDelivery, bussinessProcessLogDto);
                        rsp.IsAsyn = true;
                        rsp.Message = "快速发货命令已提交，请稍后等待返回处理消息";
                    }
                    else
                    {
                        rsp = OutboundQuickDelivery(outboundQuickDeliveryDto);
                        rsp.IsAsyn = false;
                        rsp.Message = rsp.ErrorMessage;
                    }
                }
                else
                {
                    throw new Exception("出库设置规则不存在，请检查");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rsp;
        }

        [UnitOfWork(isTransactional: false)]
        private void OutboundQuickDeliverySaveChange(OutboundQuickDeliveryDto outboundQuickDeliveryDto, List<UpdateInventoryDto> updateInventoryDtos, List<UpdateOutboundDto> updateOutboundDtos, BusinessLogDto businessLogDto, List<pickdetail> pickdetailList, List<invtran> invtranList, outbound outbound, prepack prepack)
        {
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    _outboundRepository.Update(outbound);
                    _wmsSqlRepository.UpdateInventoryQuickDelivery(updateInventoryDtos);
                    _wmsSqlRepository.UpdateOutboundDetailQuickDelivery(updateOutboundDtos);
                    PickDetailQuickDelivery(businessLogDto, pickdetailList, invtranList);

                    #region 更新对应的散货封箱单，更新交接单为完成

                    _preBulkPackRepository.UpdaPreBulkPack(outbound.SysId, outboundQuickDeliveryDto.CurrentUserId, outboundQuickDeliveryDto.CurrentDisplayName);

                    //还原拣货容器状态
                    _wmsSqlRepository.ClearContainer(new ClearContainerDto
                    {
                        ContainerSysIds = containerSysIds,
                        WarehouseSysId = outboundQuickDeliveryDto.WarehouseSysId,
                        CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName
                    });
                    //清除复核缓存
                    _redisAppService.CleanReviewRecords(outbound.OutboundOrder, outboundQuickDeliveryDto.WarehouseSysId);

                    //更新交接单到-->完成
                    _outboundTransferOrderRepository.UpdateOutboundTransferOrderFinish(new OutboundTransferOrderQueryDto()
                    {
                        WarehouseSysId = outbound.WareHouseSysId,
                        OutboundSysId = outbound.SysId,
                        CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName
                    });

                    #endregion

                    #region 更新预包装
                    if (prepack != null && prepack.WareHouseSysId != new Guid())
                    {
                        _outboundRepository.Update(prepack);
                    }
                    #endregion

                    #region 更新SN表记录信息

                    if (outboundQuickDeliveryDto.SNList != null && outboundQuickDeliveryDto.SNList.Count > 0)
                    {
                        _outboundRepository.BatchUpdateSNListForOutbound(outboundQuickDeliveryDto.SNList, outbound.WareHouseSysId, outbound.SysId, outbound.UpdateBy, outbound.UpdateUserName);
                    }

                    #endregion

                    _outboundRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                //scope.
            }

        }

        /// <summary>
        /// B2B 业务一键发货
        /// </summary>
        /// <param name="outboundQuickDeliveryDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse OutboundQuickDelivery(OutboundQuickDeliveryDto outboundQuickDeliveryDto)
        {
            _crudRepository.ChangeDB(outboundQuickDeliveryDto.WarehouseSysId);
            var outboundOrder = string.Empty;
            var rsp = new CommonResponse() { IsSuccess = false };
            try
            {
                var outbound = _outboundRepository.Get<outbound>(outboundQuickDeliveryDto.SysId.Value);
                outboundOrder = outbound.OutboundOrder;
                if (outbound.Status != (int)OutboundStatus.New)
                {
                    throw new Exception("出库单状态不等于新建,无法进行快速发货");
                }
                var outboundDetail = outbound.outbounddetails.ToList();
                var totalAllocatedQty = 0;

                var outboundStatusFlg = true;
                var skuIds = outboundDetail.Select(x => x.SkuSysId).ToList();
                var pickdetailList = new List<pickdetail>();
                var invtranList = new List<invtran>();
                var skuALL = _outboundRepository.GetQuery<sku>(x => skuIds.Contains(x.SysId)).ToList();
                var packALL = _outboundRepository.GetAll<pack>().ToList();
                var uomALL = _outboundRepository.GetAll<uom>().ToList();

                var updateInventoryDtos = new List<UpdateInventoryDto>();
                var updateOutboundDtos = new List<UpdateOutboundDto>();
                var outboundRule = _crudRepository.GetQuery<outboundrule>(x => x.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId).FirstOrDefault();
                var inv = _inventoryRepository.GetlotloclpnBySkuSysIdOrderByLotDetail(skuIds, outboundQuickDeliveryDto.WarehouseSysId, outbound, outboundRule);
                var invLocList = inv.Select(x => x.Loc).Distinct();
                var locationList = _outboundRepository.GetQuery<location>(p => invLocList.Contains(p.Loc) && p.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId).ToList();
                var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuIds.Contains(p.SkuSysId.Value)
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId);
                var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuIds.Contains(p.SkuSysId.Value)
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId).ToList();

                outboundDetail.ForEach(item =>
                {
                    var invLotLocLpn = inv.Where(x => x.SkuSysId == item.SkuSysId).ToList();
                    var sku = skuALL.FirstOrDefault(x => x.SysId == item.SkuSysId);
                    ///默认使用 先进先出原则
                    //var invLotLocLpn = _inventoryRepository.GetlotloclpnBySkuSysIdOrderByLotDetail(item.SkuSysId,
                    //    PublicConst.PickRuleFO, outboundQuickDeliveryDto.WarehouseSysId);
                    if (invLotLocLpn.Any())
                    {
                        //剩余数量
                        var residualQty = item.Qty.Value;
                        foreach (var info in invLotLocLpn)
                        {
                            //校验商品冻结
                            var frozenSku = frozenSkuList.FirstOrDefault(p => p.SkuSysId == item.SkuSysId);
                            if (frozenSku != null)
                            {
                                throw new Exception($"商品({sku.SkuName})已经被冻结，不能发货!");
                            }

                            //校验库存冻结
                            var location = locationList.FirstOrDefault(p => p.Loc.ToUpper() == info.Loc.ToUpper() && p.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId);

                            if (location == null)
                            {
                                throw new Exception($"货位{info.Loc}已经不存在，请重新创建!");
                            }

                            if (location.Status == (int)LocationStatus.Frozen)
                            {
                                //冻结货位或者储区，不能分配
                                continue;
                            }

                            //校验冻结: 货位商品 
                            if (frozenLocSkuList.Count > 0)
                            {
                                var locskuFrozen = frozenLocSkuList.FirstOrDefault(p => p.SkuSysId == info.SkuSysId && p.Loc == info.Loc);//info

                                if (locskuFrozen != null)
                                {
                                    //货位商品冻结，不能分配
                                    continue;
                                }
                            }

                            //扣减数量
                            var deductionQty = 0;
                            //当前可用数量
                            var currentQty = CommonBussinessMethod.GetAvailableQty(info.Qty, info.AllocatedQty, info.PickedQty, info.FrozenQty);
                            if (currentQty == 0)
                            {
                                //如果可用数量 =0 那么匹配下一条库存信息
                                continue;
                            }
                            deductionQty = residualQty <= currentQty ? residualQty : currentQty;
                            //计算剩余数量
                            residualQty = residualQty - deductionQty;
                            totalAllocatedQty = totalAllocatedQty + deductionQty;

                            updateInventoryDtos.Add(new UpdateInventoryDto()
                            {
                                InvLotLocLpnSysId = info.InvLotLocLpnSysId,
                                InvLotSysId = info.InvLotSysId,
                                InvSkuLocSysId = info.InvSkuLocSysId,
                                Qty = deductionQty,
                                CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                                CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName,

                            });

                            #region 写入拣货

                            var pickDetail = new pickdetail();
                            pickDetail.SysId = Guid.NewGuid();
                            pickDetail.OutboundSysId = item.OutboundSysId;
                            pickDetail.OutboundDetailSysId = item.SysId;
                            pickDetail.SkuSysId = item.SkuSysId;
                            pickDetail.PickDetailOrder = "";
                            pickDetail.PackSysId = item.PackSysId;
                            pickDetail.UOMSysId = item.UOMSysId;
                            pickDetail.Qty = deductionQty;
                            pickDetail.CreateBy = outboundQuickDeliveryDto.CurrentUserId;
                            pickDetail.CreateUserName = outboundQuickDeliveryDto.CurrentDisplayName;
                            pickDetail.CreateDate = DateTime.Now;
                            pickDetail.UpdateUserName = outboundQuickDeliveryDto.CurrentDisplayName;
                            pickDetail.UpdateBy = outboundQuickDeliveryDto.CurrentUserId;
                            pickDetail.UpdateDate = DateTime.Now;
                            pickDetail.Status = (int)PickDetailStatus.Finish;
                            pickDetail.Loc = info.Loc;
                            pickDetail.Lot = info.Lot;
                            pickDetail.Lpn = info.Lpn;
                            pickDetail.WareHouseSysId = outbound.WareHouseSysId;
                            pickdetailList.Add(pickDetail);

                            #endregion

                            #region InvTrans

                            var pack = packALL.FirstOrDefault(x => x.SysId == item.PackSysId.Value);
                            var uom = uomALL.FirstOrDefault(x => x.SysId == item.UOMSysId.Value);
                            var invTrans = new invtran()
                            {
                                SysId = Guid.NewGuid(),
                                WareHouseSysId = outbound.WareHouseSysId,
                                DocOrder = outbound.OutboundOrder, //装箱单
                                DocSysId = outbound.SysId, // 箱子SysId
                                DocDetailSysId = item.SysId, //箱内拣货SysId,
                                SkuSysId = item.SkuSysId,
                                SkuCode = sku.SkuCode,
                                TransType = InvTransType.Outbound,
                                SourceTransType = InvSourceTransType.QuickDelivery,
                                Qty = deductionQty * -1,
                                Loc = info.Loc,
                                Lot = info.Lot,
                                Lpn = info.Lpn,
                                ToLoc = info.Loc,
                                ToLot = info.Lot,
                                ToLpn = info.Lpn,
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
                                ReceivedDate = info.ReceiptDate ?? new DateTime(),
                                PackSysId = item.PackSysId.Value,
                                PackCode = pack != null ? pack.PackCode : "",
                                UOMSysId = item.UOMSysId.Value,
                                UOMCode = uom != null ? uom.UOMCode : "",
                                CreateBy = outboundQuickDeliveryDto.CurrentUserId,
                                CreateDate = DateTime.Now,
                                UpdateBy = outboundQuickDeliveryDto.CurrentUserId,
                                UpdateDate = DateTime.Now,
                                CreateUserName = outboundQuickDeliveryDto.CurrentDisplayName,
                                UpdateUserName = outboundQuickDeliveryDto.CurrentDisplayName
                            };
                            invtranList.Add(invTrans);

                            #endregion

                            if (residualQty == 0)
                            {
                                break;
                            }
                        }
                        if (residualQty > 0)
                        {
                            throw new Exception("订单号" + outbound.OutboundOrder + "库存不足,无法进行分配！");
                        }

                        updateOutboundDtos.Add(new UpdateOutboundDto()
                        {
                            SysId = item.SysId,
                            Qty = item.Qty.Value,
                            Status = (int)OutboundDetailStatus.Delivery,
                            CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                            CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName,

                        });
                    }
                    else
                    {
                        throw new Exception("订单号" + outbound.OutboundOrder + "库存不足,无法进行分配！");
                    }
                });
                //如果 状态标示等于 true 那么状态更新为 分配完成
                if (outboundStatusFlg)
                {
                    outbound.Status = (int)OutboundStatus.Delivery;
                }
                else
                {
                    throw new Exception("订单号" + outbound.OutboundOrder + "库存不足,无法进行分配！");
                }

                outbound.OutboundMethod = InvSourceTransType.QuickDelivery;
                outbound.ActualShipDate = DateTime.Now;
                outbound.TotalAllocatedQty = totalAllocatedQty;
                outbound.TotalPickedQty = totalAllocatedQty;
                outbound.TotalShippedQty = totalAllocatedQty;
                outbound.UpdateBy = outboundQuickDeliveryDto.CurrentUserId;
                outbound.UpdateUserName = outboundQuickDeliveryDto.CurrentDisplayName;
                outbound.UpdateDate = DateTime.Now;
                outbound.TS = Guid.NewGuid();

                #region 异步写入 拣货与交易记录

                var businessLogDto = new BusinessLogDto();
                businessLogDto.access_log_sysId = Guid.NewGuid();
                businessLogDto.doc_order = outbound.OutboundOrder;
                businessLogDto.doc_sysId = outbound.SysId;
                businessLogDto.user_id = outboundQuickDeliveryDto.CurrentUserId.ToString();
                businessLogDto.user_name = outboundQuickDeliveryDto.CurrentDisplayName;
                businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);

                #endregion

                #region 更细预包装信息
                var prepack = new prepack();
                var preOrderrule =
                    _outboundRepository.FirstOrDefault<preorderrule>(
                        x => x.WarehouseSysId == outboundQuickDeliveryDto.WarehouseSysId);
                if (preOrderrule != null && preOrderrule.Status.HasValue && preOrderrule.Status.Value)
                {
                    prepack = _outboundRepository.FirstOrDefault<prepack>(x => x.OutboundSysId == outbound.SysId);
                    if (prepack != null)
                    {
                        prepack.Status = (int)PrePackStatus.Finish;
                        prepack.UpdateBy = outboundQuickDeliveryDto.CurrentUserId;
                        prepack.UpdateUserName = outboundQuickDeliveryDto.CurrentDisplayName;
                        prepack.UpdateDate = DateTime.Now;
                    }
                }
                #endregion

                OutboundQuickDeliverySaveChange(outboundQuickDeliveryDto, updateInventoryDtos, updateOutboundDtos,
                    businessLogDto, pickdetailList, invtranList, outbound, prepack);

                #region 出库回写

                if (!string.IsNullOrEmpty(outbound.ExternOrderId) &&
                    outbound.OutboundType != (int)OutboundType.TransferInventory)
                {
                    var crpe = new CommonResponse();
                    try
                    {
                        crpe = _thirdPartyAppService.InsertOutStock(outbound.SysId,
                            outboundQuickDeliveryDto.CurrentDisplayName, outboundQuickDeliveryDto.CurrentUserId);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                #endregion

                #region 移仓类型发货完成生成入库单

                if (outbound.OutboundType == (int)OutboundType.TransferInventory)
                {
                    var transferInv = _crudRepository.GetQuery<transferinventory>(p => p.TransferInventoryOrder == outbound.ExternOrderId).FirstOrDefault();
                    var transferInventoryDto = new MQTransferInventoryDto()
                    {
                        TransferInventoryOrder = outbound.ExternOrderId,
                        Status = (int)TransferInventoryStatus.Delivery,
                        CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName,
                        TransferOutboundDate = transferInv.TransferOutboundDate,
                        AuditingDate = transferInv.AuditingDate,
                        AuditingBy = transferInv.AuditingBy,
                        AuditingName = transferInv.AuditingName,
                        WarehouseSysId = outboundQuickDeliveryDto.WarehouseSysId,
                        ToWareHouseSysId = transferInv.ToWareHouseSysId,
                        FromWareHouseSysId = transferInv.FromWareHouseSysId,
                        //移仓入库增加渠道
                        Channel = transferInv.Channel
                    };
                    transferInventoryDto.transferinventorydetails = transferInv.transferinventorydetails.JTransformTo<TransferInventoryDetailDto>();
                    transferInventoryDto.PurchaseSysId = Guid.NewGuid();
                    if (outbound.OutboundOrder.IndexOf("OB") > -1)
                    {
                        transferInventoryDto.PurchaseOrder = outbound.OutboundOrder.Replace("OB", "PO0");
                    }
                    else
                    {
                        transferInventoryDto.PurchaseOrder = "PO0" + outbound.OutboundOrder;
                    }
                    var processDto = new MQProcessDto<MQTransferInventoryDto>()
                    {
                        BussinessSysId = outbound.SysId,
                        BussinessOrderNumber = outbound.OutboundOrder,
                        Descr = "",
                        CurrentUserId = outboundQuickDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundQuickDeliveryDto.CurrentDisplayName,
                        WarehouseSysId = outboundQuickDeliveryDto.WarehouseSysId,
                        BussinessDto = transferInventoryDto
                    };

                    _transferInventoryAppService.UpdateTransferInventoryStatus(transferInventoryDto);


                    var response = ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/MQ/OrderManagement/TransferInventory/CreateTransferInventoryReceipt", new CoreQuery(), transferInventoryDto);

                    if (response != null && response.ResponseResult != null && response.ResponseResult.IsSuccess)
                    {
                        // 生成完入库单更新一仓单数据
                        transferInv.TransferPurchaseSysId = transferInventoryDto.PurchaseSysId;
                        transferInv.TransferPurchaseOrder = transferInventoryDto.PurchaseOrder;
                        transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                        transferInv.UpdateDate = DateTime.Now;
                        transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                        _crudRepository.Update(transferInv);
                    }
                    else
                    {
                        throw new Exception("创建入库单失败");
                    }

                    //_transferInventoryAppService.CreateTransferInventoryReceipt(transferInventoryDto);

                    //RabbitWMS.SetRabbitMQAsync(RabbitMQType.TransferInventoryInbound, processDto);
                }

                #endregion

                rsp.IsSuccess = true;
                rsp.ErrorMessage = string.Format("快速出库处理完成，订单号：{0}", outboundOrder);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var message = string.Format("快速出库处理失败，订单号：{0}，失败原因：数据重复并发提交", outboundOrder);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                var message = string.Format("快速出库处理失败，订单号：{0}，失败原因：{1}", outboundOrder, ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }

            return rsp;
        }

        /// <summary>
        /// 快速发货 写入 交易和拣货明细
        /// </summary>
        [UnitOfWork(isTransactional: false)]
        private void PickDetailQuickDelivery(BusinessLogDto businessLogDto, List<pickdetail> pickdetailList,
            List<invtran> invtran)
        {
            //new Task(() =>
            //{
            try
            {
                //var pickDetailOrder = _outboundRepository.GenNextNumber(PublicConst.GenNextNumberPickDetail);
                var pickDetailOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPickDetail);

                #region 组织业务日志 
                businessLogDto.business_name = BusinessName.QuicklyDeliver.ToDescription();
                businessLogDto.business_type = BusinessType.Outbound.ToDescription();
                businessLogDto.business_operation = PublicConst.LogQuicklyDeliver;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json记录 生成扣减库存数据, new_json记录 生成的InvTrans 记录]";
                businessLogDto.new_json = JsonConvert.SerializeObject(invtran);
                businessLogDto.create_date = DateTime.Now;
                #endregion

                _wmsSqlRepository.QuickDeliveryInsertPickDetail(pickdetailList, pickDetailOrder);
                _wmsSqlRepository.QuickDeliveryInsertInvTrans(invtran);

            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {

                //发送MQ
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }
            // }).Start();

        }

        [UnitOfWork(isTransactional: false)]
        private void OutboundAllocationDeliverySaveChange(outbound outbound, List<UpdateInventoryDto> updateInventoryDtos, UpdateOutboundDto updateOutboundDto, BusinessLogDto businessLogDto, List<invtran> invtranList, prepack prepack, List<string> SNList)
        {

            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

                var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    _outboundRepository.Update(outbound);

                    _wmsSqlRepository.UpdateInventoryAllocationDelivery(updateInventoryDtos);
                    //修改拣货明细和出库明细
                    //var updateOutboundDetailDtoList = new List<UpdateOutboundDetailDto>();
                    var updateOutboundDetailDtoList = invtranList.GroupBy(p => p.DocDetailSysId).Select(p => new UpdateOutboundDetailDto { SysId = p.Key, Qty = Math.Abs(p.Sum(x => x.Qty)) }).ToList();
                    _wmsSqlRepository.UpdateOdAndPdByAllocationDelivery(updateOutboundDto, updateOutboundDetailDtoList);
                    PickDetailInvTransAllocationDelivery(businessLogDto, invtranList);


                    #region 更新对应的散货封箱单，更新交接单为完成

                    _preBulkPackRepository.UpdaPreBulkPack(outbound.SysId, updateOutboundDto.CurrentUserId, updateOutboundDto.CurrentDisplayName);

                    //还原拣货容器状态
                    _wmsSqlRepository.ClearContainer(new ClearContainerDto
                    {
                        ContainerSysIds = containerSysIds,
                        WarehouseSysId = updateOutboundDto.WarehouseSysId,
                        CurrentUserId = updateOutboundDto.CurrentUserId,
                        CurrentDisplayName = updateOutboundDto.CurrentDisplayName
                    });
                    //清除复核缓存
                    _redisAppService.CleanReviewRecords(outbound.OutboundOrder, updateOutboundDto.WarehouseSysId);
                    RedisWMS.CleanRedis<List<RFOutboundReviewDetailDto>>(string.Format(RedisSourceKey.RedisOutboundReviewDiff, outbound.OutboundOrder, updateOutboundDto.WarehouseSysId));

                    //更新交接单到-->完成
                    _outboundTransferOrderRepository.UpdateOutboundTransferOrderFinish(new OutboundTransferOrderQueryDto()
                    {
                        WarehouseSysId = outbound.WareHouseSysId,
                        OutboundSysId = outbound.SysId,
                        CurrentUserId = updateOutboundDto.CurrentUserId,
                        CurrentDisplayName = updateOutboundDto.CurrentDisplayName
                    });

                    #endregion

                    #region 更新预包装
                    if (prepack != null && prepack.WareHouseSysId != new Guid())
                    {
                        _outboundRepository.Update(prepack);
                    }
                    #endregion

                    #region 更新SN表记录信息

                    if (SNList != null && SNList.Count > 0)
                    {
                        _outboundRepository.BatchUpdateSNListForOutbound(SNList, outbound.WareHouseSysId, outbound.SysId, outbound.UpdateBy, outbound.UpdateUserName);
                    }

                    #endregion

                    _outboundRepository.SaveChange();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                //scope.
            }



        }
        #region 分配发货
        /// <summary>
        /// 分配发货
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse OutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            _crudRepository.ChangeDB(outboundAllocationDeliveryDto.WarehouseSysId);
            var outboundOrder = string.Empty;
            var rsp = new CommonResponse() { IsSuccess = false };
            try
            {
                var outbound = _outboundRepository.Get<outbound>(outboundAllocationDeliveryDto.SysId.Value);
                outboundOrder = outbound.OutboundOrder;
                //B2C退货入库校验
                if (outbound.IsReturn == (int)OutboundReturnStatus.B2CReturn)
                {
                    throw new Exception("出库单已经被B2C退货，无法进行分配发货！");
                }
                if (outbound.Status != (int)OutboundStatus.Allocation)
                {
                    throw new Exception("出库单状态不等于分配完成，无法进行分配发货");
                }
                var outboundDetail = outbound.outbounddetails.ToList();
                var totalAllocatedQty = 0;

                var pickdetailList = new List<pickdetail>();
                var invtranList = new List<invtran>();
                var skuIds = outboundDetail.Select(x => x.SkuSysId).ToList();
                var packALL = _outboundRepository.GetAll<pack>().ToList();
                var skuALL = _outboundRepository.GetQuery<sku>(x => skuIds.Contains(x.SysId)).ToList();
                var uomALL = _outboundRepository.GetAll<uom>().ToList();

                var updateInventoryDtos = new List<UpdateInventoryDto>();
                var transferinventoryreceiptextendRequest = new List<MQTransferinventoryReceiptExtendDto>();

                var inv = _inventoryRepository.GetlotloclpnBySkuSysIdOrderByLotDetail(skuIds, outboundAllocationDeliveryDto.WarehouseSysId, outbound, null);
                var pickDetails = _crudRepository.GetQuery<pickdetail>(x => x.OutboundSysId == outbound.SysId && x.Status == (int)PickDetailStatus.New).ToList();
                if (pickDetails == null || pickDetails.Count == 0)
                {
                    throw new Exception("分配明细不存在，无法进行分配发货");
                }

                bool partialShipmentFlag = pickDetails.Sum(p => p.PickedQty) > 0;
                foreach (var pd in pickDetails)
                {
                    var info = inv.FirstOrDefault(x => x.SkuSysId == pd.SkuSysId && x.Lot == pd.Lot && x.Loc == pd.Loc && x.Lpn == pd.Lpn);
                    if (info == null)
                    {
                        throw new Exception("库存数据不存在，请检查");
                    }

                    //拣货单没有拣货数量，认为是全部发货
                    if (!partialShipmentFlag)
                    {
                        pd.PickedQty = pd.Qty.GetValueOrDefault();
                    }

                    totalAllocatedQty = totalAllocatedQty + pd.PickedQty;
                    updateInventoryDtos.Add(new UpdateInventoryDto()
                    {
                        InvLotLocLpnSysId = info.InvLotLocLpnSysId,
                        InvLotSysId = info.InvLotSysId,
                        InvSkuLocSysId = info.InvSkuLocSysId,
                        Qty = pd.PickedQty,
                        CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                    });

                    #region InvTrans
                    if (pd.PickedQty != 0)
                    {
                        var pack = packALL.FirstOrDefault(x => x.SysId == pd.PackSysId.Value);
                        var sku = skuALL.FirstOrDefault(x => x.SysId == pd.SkuSysId);
                        var uom = uomALL.FirstOrDefault(x => x.SysId == pd.UOMSysId.Value);
                        var invTrans = new invtran()
                        {
                            SysId = Guid.NewGuid(),
                            WareHouseSysId = outbound.WareHouseSysId,
                            DocOrder = pd.PickDetailOrder,   //拣货单号
                            DocSysId = pd.SysId,    //拣货单Id
                                                    //DocDetailSysId = item.SysId,
                                                    //SkuSysId = item.SkuSysId,
                            DocDetailSysId = (Guid)pd.OutboundDetailSysId,
                            SkuSysId = pd.SkuSysId,
                            SkuCode = sku.SkuCode,
                            TransType = InvTransType.Outbound,
                            SourceTransType = InvSourceTransType.AllocationDelivery,
                            Qty = pd.PickedQty * -1,
                            Loc = info.Loc,
                            Lot = info.Lot,
                            Lpn = info.Lpn,
                            ToLoc = info.Loc,
                            ToLot = info.Lot,
                            ToLpn = info.Lpn,
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
                            ReceivedDate = info.ReceiptDate ?? new DateTime(),
                            //PackSysId = item.PackSysId.Value,
                            PackSysId = pd.PackSysId.Value,
                            PackCode = pack != null ? pack.PackCode : "",
                            //UOMSysId = item.UOMSysId.Value,
                            UOMSysId = pd.UOMSysId.Value,
                            UOMCode = uom != null ? uom.UOMCode : "",
                            CreateBy = outboundAllocationDeliveryDto.CurrentUserId,
                            CreateDate = DateTime.Now,
                            UpdateBy = outboundAllocationDeliveryDto.CurrentUserId,
                            UpdateDate = DateTime.Now,
                            CreateUserName = outboundAllocationDeliveryDto.CurrentDisplayName,
                            UpdateUserName = outboundAllocationDeliveryDto.CurrentDisplayName
                        };
                        invtranList.Add(invTrans);
                    }
                    #endregion

                    if (transferinventoryreceiptextendRequest.Exists(p => p.Lot.Equals(pd.Lot, StringComparison.OrdinalIgnoreCase)))
                    {
                        transferinventoryreceiptextendRequest.First(p => p.Lot.Equals(pd.Lot, StringComparison.OrdinalIgnoreCase)).Qty += pd.PickedQty;
                    }
                    else
                    {
                        transferinventoryreceiptextendRequest.Add(new MQTransferinventoryReceiptExtendDto()
                        {
                            SkuSysId = pd.SkuSysId,
                            Qty = pd.PickedQty,
                            ReceivedQty = 0,
                            Lot = pd.Lot,
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
                            ReceivedDate = info.ReceiptDate ?? new DateTime()
                        });
                    }

                }

                outbound.OutboundMethod = InvSourceTransType.AllocationDelivery;
                outbound.Status = (int)OutboundStatus.Delivery;
                outbound.ActualShipDate = DateTime.Now;
                //outbound.TotalAllocatedQty = totalAllocatedQty;
                outbound.TotalPickedQty = totalAllocatedQty;
                outbound.TotalShippedQty = totalAllocatedQty;
                outbound.UpdateBy = outboundAllocationDeliveryDto.CurrentUserId;
                outbound.UpdateUserName = outboundAllocationDeliveryDto.CurrentDisplayName;
                outbound.UpdateDate = DateTime.Now;
                outbound.TS = Guid.NewGuid();



                #region 写入交易记录

                var businessLogDto = new BusinessLogDto();
                businessLogDto.access_log_sysId = Guid.NewGuid();
                businessLogDto.doc_order = outbound.OutboundOrder;
                businessLogDto.doc_sysId = outbound.SysId;
                businessLogDto.user_id = outboundAllocationDeliveryDto.CurrentUserId.ToString();
                businessLogDto.user_name = outboundAllocationDeliveryDto.CurrentDisplayName;
                businessLogDto.old_json = JsonConvert.SerializeObject(updateInventoryDtos);

                #endregion

                #region 更细预包装信息
                var prepack = new prepack();
                var preOrderrule = _outboundRepository.FirstOrDefault<preorderrule>(x => x.WarehouseSysId == outboundAllocationDeliveryDto.WarehouseSysId);
                if (preOrderrule != null && preOrderrule.Status.HasValue && preOrderrule.Status.Value)
                {
                    prepack = _outboundRepository.FirstOrDefault<prepack>(x => x.OutboundSysId == outbound.SysId);
                    if (prepack != null)
                    {
                        prepack.Status = (int)PrePackStatus.Finish;
                        prepack.UpdateBy = outboundAllocationDeliveryDto.CurrentUserId;
                        prepack.UpdateUserName = outboundAllocationDeliveryDto.CurrentDisplayName;
                        prepack.UpdateDate = DateTime.Now;
                    }
                }

                #endregion

                OutboundAllocationDeliverySaveChange(outbound, updateInventoryDtos, new UpdateOutboundDto()
                {
                    SysId = outbound.SysId,
                    WarehouseSysId = outboundAllocationDeliveryDto.WarehouseSysId,
                    CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                    CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                    PartialShipmentFlag = partialShipmentFlag
                }, businessLogDto, invtranList, prepack, outboundAllocationDeliveryDto.SNList);

                //部分发货，扣减剩余占用库存
                UpdateInventoryByRemainingAllocatedQty(outboundAllocationDeliveryDto, inv, pickDetails);

                #region 出库回写
                //通知ECC
                if (!string.IsNullOrEmpty(outbound.ExternOrderId) && outbound.OutboundType != (int)OutboundType.TransferInventory)
                {
                    var crpe = new CommonResponse();
                    try
                    {
                        crpe = _thirdPartyAppService.InsertOutStock(outbound.SysId,
                            outboundAllocationDeliveryDto.CurrentDisplayName, outboundAllocationDeliveryDto.CurrentUserId);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                //通知TMS
                if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer || outbound.OutboundType == (int)OutboundType.TransferInventory)
                {
                    TMSOrderType TMS_OrderType = TMSOrderType.B2BOrder;
                    string OrderId = outbound.ExternOrderId;

                    if (outbound.OutboundType == (int)OutboundType.TransferInventory)
                    {
                        TMS_OrderType = TMSOrderType.TransferOrder;
                        var tod = _outboundRepository.GetQuery<transferinventory>(x => x.TransferInventoryOrder == outbound.ExternOrderId).FirstOrDefault();
                        OrderId = tod.ExternOrderId;
                    }

                    var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                    {
                        OutboundSysId = outbound.SysId,
                        OutboundOrder = outbound.OutboundOrder,
                        OrderId = OrderId,
                        Status = (int)TMSStatus.Outbound,
                        UpdateDate = DateTime.Now,
                        EditUserName = outboundAllocationDeliveryDto.CurrentDisplayName,
                        UserId = outboundAllocationDeliveryDto.CurrentUserId,
                        OrderType = (int)TMS_OrderType
                    };
                    _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
                }
                #endregion

                #region 移仓类型发货完成生成入库单
                if (outbound.OutboundType == (int)OutboundType.TransferInventory)
                {
                    var transferInv = _crudRepository.GetQuery<transferinventory>(p => p.TransferInventoryOrder == outbound.ExternOrderId).FirstOrDefault();

                    var transferInventoryDto = new MQTransferInventoryDto()
                    {
                        TransferInventoryOrder = outbound.ExternOrderId,
                        Status = (int)TransferInventoryStatus.Delivery,
                        CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                        FromWareHouseSysId = transferInv.FromWareHouseSysId,
                        TransferOutboundDate = transferInv.TransferOutboundDate,
                        AuditingDate = transferInv.AuditingDate,
                        AuditingBy = transferInv.AuditingBy,
                        AuditingName = transferInv.AuditingName,
                        WarehouseSysId = outboundAllocationDeliveryDto.WarehouseSysId,
                        ToWareHouseSysId = transferInv.ToWareHouseSysId,
                        //增加渠道信息
                        Channel = transferInv.Channel
                    };
                    transferInventoryDto.transferinventorydetails = transferInv.transferinventorydetails.JTransformTo<TransferInventoryDetailDto>();
                    transferInventoryDto.PurchaseSysId = Guid.NewGuid();
                    if (outbound.OutboundOrder.IndexOf("OB") > -1)
                    {
                        transferInventoryDto.PurchaseOrder = outbound.OutboundOrder.Replace("OB", "PO0");
                    }
                    else
                    {
                        transferInventoryDto.PurchaseOrder = "PO0" + outbound.OutboundOrder;
                    }
                    var processDto = new MQProcessDto<MQTransferInventoryDto>()
                    {
                        BussinessSysId = outbound.SysId,
                        BussinessOrderNumber = outbound.OutboundOrder,
                        Descr = "",
                        CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                        WarehouseSysId = outboundAllocationDeliveryDto.WarehouseSysId,
                        BussinessDto = transferInventoryDto
                    };

                    _transferInventoryAppService.UpdateTransferInventoryStatus(transferInventoryDto);

                    transferInventoryDto.Transferinventoryreceiptextends = transferinventoryreceiptextendRequest;

                    var response = ApiClient.Post<CommonResponse>(PublicConst.WmsApiUrl, "/MQ/OrderManagement/TransferInventory/CreateTransferInventoryReceipt", new CoreQuery(), transferInventoryDto);

                    if (response != null && response.ResponseResult != null && response.ResponseResult.IsSuccess)
                    {
                        // 生成完入库单更新一仓单数据
                        transferInv.TransferPurchaseSysId = transferInventoryDto.PurchaseSysId;
                        transferInv.TransferPurchaseOrder = transferInventoryDto.PurchaseOrder;
                        transferInv.UpdateBy = transferInventoryDto.CurrentUserId;
                        transferInv.UpdateDate = DateTime.Now;
                        transferInv.UpdateUserName = transferInventoryDto.CurrentDisplayName;
                        _crudRepository.Update(transferInv);
                    }
                    else
                    {
                        throw new Exception("创建入库单失败");
                    }
                    //_transferInventoryAppService.CreateTransferInventoryReceipt(transferInventoryDto);
                    //RabbitWMS.SetRabbitMQAsync(RabbitMQType.TransferInventoryInbound, processDto);
                }
                #endregion

                #region 组织推送拣货完成工单数据
                if (outbound != null)
                {
                    var mqWorkDto = new MQWorkDto()
                    {
                        WorkBusinessType = (int)WorkBusinessType.Update,
                        WorkType = (int)UserWorkType.Picking,
                        WarehouseSysId = outboundAllocationDeliveryDto.WarehouseSysId,
                        CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                        CancelWorkDto = new CancelWorkDto()
                        {
                            DocSysIds = new List<Guid>() { outbound.SysId },
                            Status = (int)WorkStatus.Finish
                        }
                    };

                    var workProcessDto = new MQProcessDto<MQWorkDto>()
                    {
                        BussinessSysId = outbound.SysId,
                        BussinessOrderNumber = outbound.OutboundOrder,
                        Descr = "",
                        CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                        CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                        WarehouseSysId = outboundAllocationDeliveryDto.WarehouseSysId,
                        BussinessDto = mqWorkDto
                    };
                    //推送工单数据
                    RabbitWMS.SetRabbitMQAsync(RabbitMQType.Work_Insert_Update, workProcessDto);
                }
                #endregion

                rsp.IsSuccess = true;
                rsp.ErrorMessage = string.Format("分配发货处理完成，订单号：{0}", outboundOrder);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                var message = string.Format("分配发货处理失败，订单号：{0}，失败原因：数据重复并发提交", outboundOrder);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                var message = string.Format("分配发货处理失败，订单号：{0}，失败原因：{1}", outboundOrder, ex.Message);
                rsp.IsSuccess = false;
                rsp.ErrorMessage = message;
                throw new Exception(message);
            }
            return rsp;
        }

        /// <summary>
        /// 分配发货检查差异
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        public CommonResponse CheckOutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            CommonResponse rsp = new CommonResponse();
            _crudRepository.ChangeDB(outboundAllocationDeliveryDto.WarehouseSysId);
            var pickDetails = _crudRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outboundAllocationDeliveryDto.SysId).ToList();
            var outboundDetails = _crudRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outboundAllocationDeliveryDto.SysId);
            if (pickDetails.Sum(p => p.PickedQty) > 0 && outboundDetails.Sum(p => p.Qty) != pickDetails.Sum(p => p.PickedQty))
            {
                var outbound = _crudRepository.GetQuery<outbound>(p => p.SysId == outboundAllocationDeliveryDto.SysId).FirstOrDefault();
                if (outbound != null && outbound.OutboundType != (int)OutboundType.B2B && outbound.OutboundType != (int)OutboundType.Fertilizer)
                {
                    rsp.IsSuccess = false;
                    rsp.ErrorMessage = "只有B2B和农资类型订单才能部分发货";
                }
                else
                {
                    rsp.IsSuccess = false;
                    rsp.Message = "拣货数量与订单数量不一致，是否继续发货？";
                }
            }
            return rsp;
        }

        /// <summary>
        /// 获取部分发货商品明细
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        public List<PartShipmentDetailDto> GetPartShipmentSkuList(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            _crudRepository.ChangeDB(outboundAllocationDeliveryDto.WarehouseSysId);
            var rsp = _outboundRepository.GetPartShipmentSkuList(outboundAllocationDeliveryDto);
            return rsp.Where(p => p.Qty != p.PickedQty).ToList();
        }

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        public CommonResponse SavePartShipmentMemo(PartShipmentMemoDto partShipmentMemoDto)
        {
            _crudRepository.ChangeDB(partShipmentMemoDto.WarehouseSysId);
            return _wmsSqlRepository.UpdateOutboundDetailMemo(partShipmentMemoDto);
        }

        /// <summary>
        /// 部分发货，扣减剩余占用库存
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <param name="inv"></param>
        /// <param name="pickDetails"></param>
        private void UpdateInventoryByRemainingAllocatedQty(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto, List<InvLotLocLpnDto> inv, List<pickdetail> pickDetails)
        {
            var updateInventoryList = new List<UpdateInventoryDto>();
            foreach (var pd in pickDetails)
            {
                if (pd.Qty == pd.PickedQty) continue;

                var info = inv.FirstOrDefault(x => x.SkuSysId == pd.SkuSysId && x.Lot == pd.Lot && x.Loc == pd.Loc && x.Lpn == pd.Lpn);
                if (info == null)
                {
                    throw new Exception("库存数据不存在，请检查");
                }

                updateInventoryList.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = info.InvLotLocLpnSysId,
                    InvLotSysId = info.InvLotSysId,
                    InvSkuLocSysId = info.InvSkuLocSysId,
                    Qty = pd.Qty.GetValueOrDefault() - pd.PickedQty,
                    CurrentUserId = outboundAllocationDeliveryDto.CurrentUserId,
                    CurrentDisplayName = outboundAllocationDeliveryDto.CurrentDisplayName,
                });
            }

            if (updateInventoryList.Any())
            {
                _wmsSqlRepository.UpdateInventoryByRemainingAllocatedQty(updateInventoryList);
            }
        }

        /// <summary>
        /// 分配发货 写入 交易
        /// </summary>
        private void PickDetailInvTransAllocationDelivery(BusinessLogDto businessLogDto, List<invtran> invtran)
        {
            try
            {
                #region 组织业务日志 
                businessLogDto.business_name = BusinessName.AllocationDeliver.ToDescription();
                businessLogDto.business_type = BusinessType.Outbound.ToDescription();
                businessLogDto.business_operation = PublicConst.LogAllocationDeliver;
                businessLogDto.flag = true;
                businessLogDto.descr = "[old_json记录 生成扣减库存数据, new_json记录 生成的InvTrans 记录]";
                businessLogDto.new_json = JsonConvert.SerializeObject(invtran);
                businessLogDto.create_date = DateTime.Now;
                #endregion

                _wmsSqlRepository.AllocationDeliveryInsertInvTrans(invtran);
            }
            catch (Exception ex)
            {
                businessLogDto.descr += ex.Message;
                businessLogDto.flag = false;
                throw new Exception(ex.Message);
            }
            finally
            {
                //发送MQ
                RabbitWMS.SetRabbitMQAsync(RabbitMQType.BusinessLog, businessLogDto);
            }
        }
        #endregion

        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        public bool ObsoleteOutbound(OutboundOperateDto outboundDto)
        {
            _crudRepository.ChangeDB(outboundDto.WarehouseSysId);
            var result = false;
            try
            {
                var outbound = _outboundRepository.Get<outbound>(outboundDto.SysId);

                if (outbound == null)
                {
                    throw new Exception("出库单不存在");
                }

                if (outbound.Status != (int)OutboundStatus.New)
                {
                    throw new Exception("只能作废状态为新建的出库单");
                }

                outbound.Status = (int)OutboundStatus.Cancel;
                outbound.UpdateBy = outboundDto.CurrentUserId;
                outbound.UpdateUserName = outboundDto.CurrentDisplayName;
                outbound.UpdateDate = DateTime.Now;
                _outboundRepository.Update(outbound);


                #region 作废出库单，将出库单绑定的散货封箱作废,绑定的交界单作废

                _preBulkPackRepository.UpdatePreBulkPackStatus(outbound.SysId, outboundDto.CurrentUserId, outboundDto.CurrentDisplayName, (int)PreBulkPackStatus.Cancel);

                ////还原拣货容器状态
                ////var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
                ////_wmsSqlRepository.ClearContainer(new ClearContainerDto
                ////{
                ////    ContainerSysIds = containerSysIds,
                ////    WarehouseSysId = outboundDto.WarehouseSysId,
                ////    CurrentUserId = outboundDto.CurrentUserId,
                ////    CurrentDisplayName = outboundDto.CurrentDisplayName
                ////});
                //////清除复核缓存
                ////_redisAppService.CleanReviewRecords(outbound.OutboundOrder, outboundDto.WarehouseSysId);

                //更新交接单到-->作废
                _outboundTransferOrderRepository.UpdateOutboundTransferOrder(new OutboundTransferOrderQueryDto()
                {
                    Status = (int)OutboundTransferOrderStatus.Cancel,
                    WarehouseSysId = outboundDto.WarehouseSysId,
                    OutboundSysId = outbound.SysId,
                    CurrentUserId = outboundDto.CurrentUserId,
                    CurrentDisplayName = outboundDto.CurrentDisplayName
                });

                #endregion

                #region 如果B2B作废，通知TMS
                if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
                {   //5作废
                    var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                    {
                        OutboundSysId = outbound.SysId,
                        OutboundOrder = outbound.OutboundOrder,
                        OrderId = outbound.ExternOrderId,
                        Status = (int)TMSStatus.Close,
                        UpdateDate = DateTime.Now,
                        EditUserName = outboundDto.CurrentDisplayName,
                        UserId = outboundDto.CurrentUserId,
                        OrderType = (int)TMSOrderType.B2BOrder
                    };
                    _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
                }
                #endregion

                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 退货
        /// </summary>
        /// <param name="outboundDto"></param>
        public void OutboundReturn(OutboundOperateDto outboundDto)
        {
            _crudRepository.ChangeDB(outboundDto.WarehouseSysId);
            var outbound = _outboundRepository.Get<outbound>(outboundDto.SysId);

            if (outbound == null)
            {
                throw new Exception("出库单不存在");
            }

            if (outbound.Status != (int)OutboundStatus.Delivery)
            {
                throw new Exception("只有出库完成的出库单才能退货入库");
            }

            if (outbound.IsReturn != null)
            {
                var purchaseReturn = _crudRepository.FirstOrDefault<purchase>(o => o.OutboundSysId == outbound.SysId);
                if (purchaseReturn != null)
                {
                    throw new Exception($"该出库单已经做过退货入库!对应入库单号号为:{purchaseReturn.PurchaseOrder}");
                }
                throw new Exception($"该出库单已经做过退货入库!");
            }

            var purchase = new PurchaseForReturnDto();
            purchase.SysId = Guid.NewGuid();


            //加入b2c退货入库 即物流拒收
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.B2C || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {
                ECCReturnOrder trdAPIRequest = new ECCReturnOrder()
                {
                    OriginalOutStockId = int.Parse(outbound.ExternOrderId),
                    WarhouseSysId = 0, //若为0默认为出库仓收货
                    SourcePlatform = "WMS",
                    RequestUser = outboundDto.CurrentDisplayName,
                    Type = (int)ECCReturnOrderType.All
                };

                var crpe = _thirdPartyAppService.CreatePurchaseOrderNumber(trdAPIRequest, outboundDto.CurrentUserId, outboundDto.CurrentDisplayName, purchase.SysId);
                if (crpe.IsSuccess)
                {
                    purchase.PurchaseOrder = crpe.ResultData.PurchaseOrder;
                    purchase.ExternalOrder = crpe.ResultData.ExternalOrder;

                    //判断调用ECC接口生成的入库单单号和外部单据号是否唯一
                    var purchaseExternalExist = _outboundRepository.FirstOrDefault<purchase>(p => p.ExternalOrder == purchase.ExternalOrder);
                    if (purchaseExternalExist != null)
                    {
                        throw new Exception($"调用ECC系统生成入库单号已存在!");
                    }
                }
                else
                {
                    throw new Exception($"调用ECC系统生成入库单号异常:{crpe.ErrorMessage}");
                }
            }
            else
            {
                purchase.PurchaseOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase);
                purchase.ExternalOrder = outbound.OutboundOrder;
            }

            //将出库单对应的标记修改成已生成退货入库
            outbound.IsReturn = (int)OutboundReturnStatus.AllReturn;
            outbound.PurchaseOrder = purchase.PurchaseOrder;
            outbound.UpdateBy = outboundDto.CurrentUserId;
            outbound.UpdateUserName = outboundDto.CurrentDisplayName;
            outbound.UpdateDate = DateTime.Now;
            _outboundRepository.Update(outbound);

            var outbounddetails = _outboundRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outboundDto.SysId);
            foreach (var outboundDetail in outbounddetails)
            {
                outboundDetail.ReturnQty = outboundDetail.ShippedQty.HasValue ? outboundDetail.ShippedQty.Value : 0;
                outboundDetail.UpdateBy = outboundDto.CurrentUserId;
                outboundDetail.UpdateDate = DateTime.Now;
                outboundDetail.UpdateUserName = outboundDto.CurrentDisplayName;
                _outboundRepository.Update(outboundDetail);
            }

            purchase.DeliveryDate = DateTime.Now;
            purchase.VendorSysId = new Guid(PublicConst.ReturnVendorSysId);  //WMS退货专用虚拟供应商
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {
                purchase.Descr = $"{outbound.ConsigneeAddress}:{outbound.ConsigneeName}--{outbound.ConsigneePhone}";
            }
            else
            {
                purchase.Descr = $"{outbound.ConsigneeProvince}{outbound.ConsigneeCity}{outbound.ConsigneeArea}{outbound.ConsigneeAddress}:{outbound.ConsigneeName}--{outbound.ConsigneePhone}";
            }
            purchase.PurchaseDate = DateTime.Now;
            purchase.AuditingBy = outboundDto.CurrentUserId.ToString();
            purchase.AuditingDate = DateTime.Now;
            purchase.AuditingName = outboundDto.CurrentDisplayName;
            purchase.CreateBy = outboundDto.CurrentUserId;
            purchase.CreateDate = DateTime.Now;
            purchase.CreateUserName = outboundDto.CurrentDisplayName;
            purchase.UpdateBy = outboundDto.CurrentUserId;
            purchase.UpdateDate = DateTime.Now;
            purchase.UpdateUserName = outboundDto.CurrentDisplayName;
            purchase.Status = (int)PurchaseStatus.New;
            purchase.Type = (int)PurchaseType.Return;
            purchase.Source = outbound.Source;
            //purchase.LastReceiptDate = DateTime.Now;
            //purchase.WarehouseSysId = outboundDto.SelectWarehouseSysId;
            purchase.WarehouseSysId = outbound.WareHouseSysId;
            purchase.FromWareHouseSysId = outbound.WareHouseSysId;
            purchase.Channel = outbound.Channel;
            purchase.BatchNumber = outbound.BatchNumber;
            purchase.OutboundSysId = outbound.SysId;
            purchase.OutboundOrder = outbound.OutboundOrder;

            var purchaseDetailList = _outboundRepository.GetPurchasedetailForOutboundReturn(purchase.OutboundSysId.Value, purchase.SysId, outboundDto.CurrentUserId, outboundDto.CurrentDisplayName);
            purchaseDetailList = purchaseDetailList.Where(p => p.Qty > 0).ToList();
            if (purchaseDetailList == null || purchaseDetailList.Count() == 0)
            {
                throw new Exception("出库明细获取失败，无法创建入库明细");
            }

            var purchaseDetails = purchaseDetailList.JTransformTo<PurchaseDetailForReturnDto>();
            purchase.purchasedetails = purchaseDetails;
            purchase.PurchaseExtend = new PurchaseExtendForReturnDto()
            {
                ServiceStationCode = outbound.ServiceStationCode,
                ServiceStationName = outbound.ServiceStationName
            };

            var response = ApiClient.Post(PublicConst.WmsApiUrl, "/Inbound/Purchase/InsertPurchaseAndDetailsByReturnOutbound", new CoreQuery(), purchase);

            if (!response.Success)
            {
                throw new Exception("创建退货入库单失败");
            }

            //_crudRepository.Insert(purchase);
            //_crudRepository.BatchInsert(purchaseDetails);
            //_wmsSqlRepository.BatchInsertPurchaseAndDetails(purchase, purchaseDetails);

            #region 如果B2B退货入库，通知TMS
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {   //"2":退货入库
                var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                {
                    OutboundSysId = outbound.SysId,
                    OutboundOrder = outbound.OutboundOrder,
                    OrderId = outbound.ExternOrderId,
                    Status = (int)TMSStatus.Return,
                    UpdateDate = DateTime.Now,
                    EditUserName = outboundDto.CurrentDisplayName,
                    UserId = outboundDto.CurrentUserId,
                    OrderType = (int)TMSOrderType.B2BOrder
                };
                _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
            }
            #endregion
        }


        /// <summary>
        /// 部分退货入库
        /// </summary>
        /// <param name="outboundDto"></param>

        public void OutboundPartReturn(OutboundPartReturnDto outboundDto)
        {
            _crudRepository.ChangeDB(outboundDto.WarehouseSysId);
            var outbound = _outboundRepository.Get<outbound>(outboundDto.SysId);

            if (outbound == null)
            {
                throw new Exception("出库单不存在");
            }

            if (outbound.Status != (int)OutboundStatus.Delivery)
            {
                throw new Exception("只有出库完成的出库单才能退货入库");
            }

            //if (outbound.OutboundType != (int)OutboundType.B2B)
            //{
            //    throw new Exception("只有(B2B)类型的出库单才能退货入库");
            //}
            var outbounddetails = _outboundRepository.GetQuery<outbounddetail>(p => p.OutboundSysId == outboundDto.SysId).ToList();

            if (!outbounddetails.ToList().Exists(p => p.ReturnQty < p.Qty))
            {
                throw new Exception($"该出库单已经全部退货!");
            }

            var purchase = new PurchaseForReturnDto();
            purchase.SysId = Guid.NewGuid();

            //将出库单对应的标记修改成已生成退货入库
            outboundDto.OutboundPartDetailList.ForEach(p =>
            {
                var outboundDetail = outbounddetails.First(q => q.SkuSysId == p.SkuSysId);
                if (outboundDetail.ReturnQty + p.ReturnQty > outboundDetail.Qty)
                {
                    throw new Exception("可退货数量不足，请刷新页面");
                }
                outboundDetail.ReturnQty += Convert.ToInt32(p.ReturnQty);
                outboundDetail.UpdateBy = outboundDto.CurrentUserId;
                outboundDetail.UpdateUserName = outboundDto.CurrentDisplayName;
                outboundDetail.UpdateDate = DateTime.Now;
                _outboundRepository.Update(outboundDetail);
            });
            outbound.IsReturn = (int)OutboundReturnStatus.PartReturn;
            outbound.PurchaseOrder = purchase.PurchaseOrder;
            outbound.UpdateBy = outboundDto.CurrentUserId;
            outbound.UpdateUserName = outboundDto.CurrentDisplayName;
            outbound.UpdateDate = DateTime.Now;
            _outboundRepository.Update(outbound);

            purchase.DeliveryDate = DateTime.Now;
            purchase.VendorSysId = new Guid(PublicConst.ReturnVendorSysId);  //WMS退货专用虚拟供应商
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {
                purchase.Descr = $"{outbound.ConsigneeAddress}:{outbound.ConsigneeName}--{outbound.ConsigneePhone}";
            }
            else
            {
                purchase.Descr = $"{outbound.ConsigneeProvince}{outbound.ConsigneeCity}{outbound.ConsigneeArea}{outbound.ConsigneeAddress}:{outbound.ConsigneeName}--{outbound.ConsigneePhone}";
            }
            purchase.PurchaseDate = DateTime.Now;
            purchase.AuditingBy = outboundDto.CurrentUserId.ToString();
            purchase.AuditingDate = DateTime.Now;
            purchase.AuditingName = outboundDto.CurrentDisplayName;
            purchase.CreateBy = outboundDto.CurrentUserId;
            purchase.CreateDate = DateTime.Now;
            purchase.CreateUserName = outboundDto.CurrentDisplayName;
            purchase.UpdateBy = outboundDto.CurrentUserId;
            purchase.UpdateDate = DateTime.Now;
            purchase.UpdateUserName = outboundDto.CurrentDisplayName;
            purchase.Status = (int)PurchaseStatus.New;
            purchase.Type = (int)PurchaseType.Return;
            purchase.Source = outbound.Source;
            //purchase.LastReceiptDate = DateTime.Now;
            purchase.WarehouseSysId = outboundDto.SelectWarehouseSysId;
            purchase.FromWareHouseSysId = outbound.WareHouseSysId;
            purchase.Channel = outbound.Channel;
            purchase.BatchNumber = outbound.BatchNumber;
            purchase.OutboundSysId = outbound.SysId;
            purchase.OutboundOrder = outbound.OutboundOrder;

            var purchaseDetailList = _outboundRepository.GetPurchasedetailForOutboundReturn(purchase.OutboundSysId.Value, purchase.SysId, outboundDto.CurrentUserId, outboundDto.CurrentDisplayName);

            //遍历入库单明细并通过商品移除不存在在部分退货入库单据中的入库单信息
            var outboundskuList = outboundDto.OutboundPartDetailList.Select(x => x.SkuSysId).ToList();
            //var purchaseDetailListFinal = purchaseDetailList.DeepClone();
            if (purchaseDetailList != null)
            {
                for (int i = purchaseDetailList.Count - 1; i >= 0; i--)
                {
                    if (!outboundskuList.Contains(purchaseDetailList[i].SkuSysId))
                    {
                        purchaseDetailList.RemoveAt(i);
                    }
                }
            }

            if (purchaseDetailList == null || purchaseDetailList.Count() == 0)
            {
                throw new Exception("出库明细获取失败，无法创建入库明细");
            }

            foreach (var item in purchaseDetailList)
            {
                var pdetail = from n in outboundDto.OutboundPartDetailList where n.SkuSysId == item.SkuSysId select new { n.ReturnQty };
                decimal qty = 0;
                if (pdetail != null && pdetail.Any())
                {
                    qty = pdetail.FirstOrDefault().ReturnQty;
                }
                item.Qty = Convert.ToInt32(qty);
            }

            //加入b2c退货入库 即物流拒收
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.B2C || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {
                var selectWarehouse = _outboundRepository.GetQuery<warehouse>(p => p.SysId == outboundDto.SelectWarehouseSysId).First();
                ECCReturnOrder trdAPIRequest = new ECCReturnOrder()
                {
                    OriginalOutStockId = int.Parse(outbound.ExternOrderId),
                    WarhouseSysId = int.Parse(selectWarehouse.OtherId),
                    SourcePlatform = "WMS",
                    RequestUser = outboundDto.CurrentDisplayName,
                    Type = (int)ECCReturnOrderType.Part,
                    ReturnDetailList = (from source in purchaseDetailList
                                        where source.Qty > 0
                                        select new ECCReturnOrderDetail()
                                        {
                                            ProductCode = source.OtherSkuId,
                                            Qty = source.Qty
                                        }).ToList()
                };


                var crpe = _thirdPartyAppService.CreatePurchaseOrderNumber(trdAPIRequest, outboundDto.CurrentUserId, outboundDto.CurrentDisplayName, purchase.SysId);
                if (crpe.IsSuccess)
                {
                    purchase.PurchaseOrder = crpe.ResultData.PurchaseOrder;
                    purchase.ExternalOrder = crpe.ResultData.ExternalOrder;

                    //判断调用ECC接口生成的入库单单号和外部单据号是否唯一
                    var purchaseExternalExist = _outboundRepository.FirstOrDefault<purchase>(p => p.ExternalOrder == purchase.ExternalOrder);
                    if (purchaseExternalExist != null)
                    {
                        throw new Exception($"调用ECC系统生成入库单号已存在!");
                    }
                }
                else
                {
                    throw new Exception($"调用ECC系统生成入库单号异常:{crpe.ErrorMessage}");
                }
            }
            else
            {
                purchase.PurchaseOrder = _baseAppService.GetNumber(PublicConst.GenNextNumberPurchase);
                purchase.ExternalOrder = outbound.OutboundOrder;
            }

            var purchaseDetails = purchaseDetailList.JTransformTo<PurchaseDetailForReturnDto>();
            purchase.purchasedetails = purchaseDetails;
            purchase.PurchaseExtend = new PurchaseExtendForReturnDto()
            {
                ServiceStationCode = outbound.ServiceStationCode,
                ServiceStationName = outbound.ServiceStationName
            };

            var response = ApiClient.Post(PublicConst.WmsApiUrl, "/Inbound/Purchase/InsertPurchaseAndDetailsByReturnOutbound", new CoreQuery(), purchase);

            if (!response.Success)
            {
                throw new Exception("创建退货入库单失败");
            }

            #region 如果B2B部分退货入库，通知TMS
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {   //"3":部分退货入库
                var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                {
                    OutboundSysId = outbound.SysId,
                    OutboundOrder = outbound.OutboundOrder,
                    OrderId = outbound.ExternOrderId,
                    Status = (int)TMSStatus.PartReturn,
                    UpdateDate = DateTime.Now,
                    EditUserName = outboundDto.CurrentDisplayName,
                    UserId = outboundDto.CurrentUserId,
                    OrderType = (int)TMSOrderType.B2BOrder
                };
                _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
            }
            #endregion
        }

        /// <summary>
        /// 出库单取消发货
        /// </summary>
        /// <param name="request"></param>
        public void OutboundCancel(OutboundOperateDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var outbound = _outboundRepository.Get<outbound>(request.SysId);

            if (outbound == null)
            {
                throw new Exception("出库单不存在");
            }

            if (outbound.OutboundType == (int)OutboundType.TransferInventory)
            {
                throw new Exception("移仓出库单不支持取消发货");
            }

            if (outbound.Status != (int)OutboundStatus.Delivery)
            {
                throw new Exception("只有出库完成的出库单才能取消发货");
            }

            if (string.IsNullOrEmpty(outbound.OutboundMethod))
            {
                throw new Exception("该出库单的出库方式缺失，请联系support补全之后再做取消操作");
            }

            var purchase = _outboundRepository.FirstOrDefault<purchase>(p => p.OutboundSysId.Value == outbound.SysId);
            if (purchase != null)
            {
                throw new Exception($"该出库单已经做过退货入库,入库单号({purchase.PurchaseOrder}),不能再做取消发货");
            }

            //快速发货
            if (outbound.OutboundMethod.Equals(InvSourceTransType.QuickDelivery, StringComparison.OrdinalIgnoreCase))
            {
                OutboundCancel_QuickDelivery(outbound, request);
            }

            //分配发货
            else if (outbound.OutboundMethod.Equals(InvSourceTransType.AllocationDelivery, StringComparison.OrdinalIgnoreCase))
            {
                OutboundCancel_AllocationDelivery(outbound, request);
            }

            //正常发货
            else if (outbound.OutboundMethod.Equals(InvSourceTransType.Shipment, StringComparison.OrdinalIgnoreCase))
            {
                OutboundCancel_Shipment(outbound, request);
            }

            else
            {
                throw new Exception("该出库单的出库方式不支持取消发货");
            }

            //如果出库单包含散货封箱单增修改状态为进行中
            _preBulkPackRepository.CancelPreBulkPack(outbound.SysId, request.CurrentUserId, request.CurrentDisplayName);

            //还原拣货容器状态
            var containerSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PreBulkPackStatus.RFPicking).Select(p => p.SysId).ToList();
            _wmsSqlRepository.ClearContainer(new ClearContainerDto
            {
                ContainerSysIds = containerSysIds,
                WarehouseSysId = request.WarehouseSysId,
                CurrentUserId = request.CurrentUserId,
                CurrentDisplayName = request.CurrentDisplayName
            });
            //清除复核缓存
            _redisAppService.CleanReviewRecords(outbound.OutboundOrder, request.WarehouseSysId);

            #region 如果B2B取消发货，通知TMS
            if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer)
            {   //"4":取消发货
                var tmsDto = new ThirdPartyUpdateOutboundTypeDto()
                {
                    OutboundSysId = outbound.SysId,
                    OutboundOrder = outbound.OutboundOrder,
                    OrderId = outbound.ExternOrderId,
                    Status = (int)TMSStatus.Cancel,
                    UpdateDate = DateTime.Now,
                    EditUserName = request.CurrentDisplayName,
                    UserId = request.CurrentUserId,
                    OrderType = (int)TMSOrderType.B2BOrder
                };
                _thirdPartyAppService.UpdateOutboundTypeToTMS(tmsDto);
            }
            #endregion
        }

        /// <summary>
        /// 快速发货 取消出库
        /// </summary>
        /// <param name="outbound"></param>
        public void OutboundCancel_QuickDelivery(outbound outbound, OutboundOperateDto request)
        {
            outbound.Status = (int)OutboundStatus.New;
            outbound.ActualShipDate = null;
            outbound.TotalAllocatedQty = 0;
            outbound.TotalPickedQty = 0;
            outbound.TotalShippedQty = 0;

            _wmsSqlRepository.UpdateOutboundDetailForOutboundCancel_QuickDelivery(outbound.SysId, OutboundDetailStatus.New, request.CurrentUserId, request.CurrentDisplayName);

            var pickdetails = _outboundRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PickDetailStatus.Finish).ToList();
            if (pickdetails == null || pickdetails.Count == 0)
            {
                throw new Exception("拣货明细信息缺失，请检查");
            }

            //校验库存冻结
            var locs = pickdetails.Select(q => q.Loc).ToList();
            var frozenLoc = _crudRepository.GetQuery<location>(p => locs.Contains(p.Loc) && p.WarehouseSysId == request.WarehouseSysId && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();

            if (frozenLoc != null)
            {
                throw new Exception($"货位{frozenLoc.Loc}已被冻结，不能取消发货!");
            }
            var skuIds = pickdetails.Select(q => q.SkuSysId).ToList();
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && skuIds.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId);
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能取消发货!");
            }

            var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && skuIds.Contains(p.SkuSysId.Value)
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId).ToList();
            //校验冻结: 货位商品 
            if (frozenLocSkuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in pickdetails
                                        join T2 in frozenLocSkuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能取消发货!");
                }
            }


            var invLotList = _outboundRepository.GetInvlotForOutboundCancel(outbound.SysId);
            var invSkuLocList = _outboundRepository.GetInvskulocForOutboundCancel(outbound.SysId);
            var invLotLocLpnList = _outboundRepository.GetInvlotloclpnForOutboundCancel(outbound.SysId);
            var updateInventoryDtos = new List<UpdateInventoryDto>();

            foreach (var pickdetail in pickdetails)
            {
                //入库存数量
                var invQty = pickdetail.Qty.Value;

                var invLot = invLotList.Where(x => x.Lot == pickdetail.Lot && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invSkuLoc = invSkuLocList.Where(x => x.Loc == pickdetail.Loc && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == pickdetail.SkuSysId && x.Lot == pickdetail.Lot && x.Loc == pickdetail.Loc && x.Lpn == pickdetail.Lpn && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();

                updateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = invLotLocLpn.SysId,
                    InvLotSysId = invLot.SysId,
                    InvSkuLocSysId = invSkuLoc.SysId,
                    Qty = invQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = outbound.WareHouseSysId,
                });

            }
            _wmsSqlRepository.UpdatePickdetailForOutboundCancel_QuickDelivery(outbound.SysId, PickDetailStatus.Cancel, request.CurrentUserId, request.CurrentDisplayName);

            //更新库存
            _wmsSqlRepository.UpdateInventoryForCancelOutbound(updateInventoryDtos);

            //更新交易
            _wmsSqlRepository.UpdateInvtranForOutboundCancel_QuickDelivery(outbound.SysId, InvTransStatus.Cancel, request.CurrentUserId, request.CurrentDisplayName);

            _outboundRepository.CancelReceiptsnByOutbound(outbound.SysId, request.CurrentUserId, request.CurrentDisplayName);

            //var invtrans = _outboundRepository.GetQuery<invtran>(p => p.DocSysId == outbound.SysId && p.Status == InvTransStatus.Ok);
            //if (invtrans != null && invtrans.Count() > 0)
            //{
            //    foreach (var invtran in invtrans)
            //    {
            //        invtran.Status = InvTransStatus.Cancel;
            //        invtran.UpdateBy = request.CurrentUserId;
            //        invtran.UpdateDate = DateTime.Now;
            //        invtran.UpdateUserName = request.CurrentDisplayName;
            //    }
            //}

        }

        /// <summary>
        /// 分配发货 取消出库
        /// </summary>
        /// <param name="outbound"></param>
        public void OutboundCancel_AllocationDelivery(outbound outbound, OutboundOperateDto request)
        {
            outbound.Status = (int)OutboundStatus.Allocation;
            outbound.ActualShipDate = null;
            outbound.TotalShippedQty = 0;
            outbound.TotalAllocatedQty = outbound.TotalQty;

            _wmsSqlRepository.UpdateOutboundDetailForOutboundCancel_AllocationDelivery(outbound.SysId, OutboundDetailStatus.Allocation, request.CurrentUserId, request.CurrentDisplayName);
            var pickdetails = _outboundRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PickDetailStatus.Finish).ToList();
            if (pickdetails == null || pickdetails.Count == 0)
            {
                throw new Exception("拣货明细信息缺失，请检查");
            }

            var pickLoc = pickdetails.Select(q => q.Loc).ToList();
            //校验库存冻结
            var frozenLoc = _crudRepository.GetQuery<location>(p => pickLoc.Contains(p.Loc) && p.WarehouseSysId == request.WarehouseSysId && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();

            if (frozenLoc != null)
            {
                throw new Exception($"货位{frozenLoc.Loc}已被冻结，不能取消发货!");
            }

            var pickSku = pickdetails.Select(q => q.SkuSysId).ToList();
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && pickSku.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId);
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能取消发货!");
            }

            var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && pickSku.Contains(p.SkuSysId.Value)
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId).ToList();
            //校验冻结: 货位商品 
            if (frozenLocSkuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in pickdetails
                                        join T2 in frozenLocSkuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能取消发货!");
                }
            }

            var invLotList = _outboundRepository.GetInvlotForOutboundCancel(outbound.SysId);
            var invSkuLocList = _outboundRepository.GetInvskulocForOutboundCancel(outbound.SysId);
            var invLotLocLpnList = _outboundRepository.GetInvlotloclpnForOutboundCancel(outbound.SysId);
            var updateInventoryDtos = new List<UpdateInventoryDto>();

            foreach (var pickdetail in pickdetails)
            {
                //入库存数量
                var invQty = pickdetail.PickedQty;

                var invLot = invLotList.Where(x => x.Lot == pickdetail.Lot && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invSkuLoc = invSkuLocList.Where(x => x.Loc == pickdetail.Loc && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == pickdetail.SkuSysId && x.Lot == pickdetail.Lot && x.Loc == pickdetail.Loc && x.Lpn == pickdetail.Lpn && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                updateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = invLotLocLpn.SysId,
                    InvLotSysId = invLot.SysId,
                    InvSkuLocSysId = invSkuLoc.SysId,
                    Qty = invQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = outbound.WareHouseSysId,
                });

            }
            _wmsSqlRepository.UpdatePickdetailForOutboundCancel_AllocationDelivery(outbound.SysId, PickDetailStatus.New, request.CurrentUserId, request.CurrentDisplayName);
            //更新库存
            _wmsSqlRepository.UpdateInventoryForCancelOutboundAllocationDelivery(updateInventoryDtos);

            var pickdetailsSysIds = pickdetails.Select(q => q.SysId).ToList();


            _wmsSqlRepository.UpdateInvtranForOutboundCancel_AllocationDelivery(pickdetailsSysIds, InvTransStatus.Cancel, request.CurrentUserId, request.CurrentDisplayName);

            _outboundRepository.CancelReceiptsnByOutbound(outbound.SysId, request.CurrentUserId, request.CurrentDisplayName);

            //var invtrans = _outboundRepository.GetQuery<invtran>(p => pickdetailsSysIds.Contains(p.DocSysId) && p.Status == InvTransStatus.Ok);
            //if (invtrans != null && invtrans.Count() > 0)
            //{
            //    foreach (var invtran in invtrans)
            //    {
            //        invtran.Status = InvTransStatus.Cancel;
            //        invtran.UpdateBy = request.CurrentUserId;
            //        invtran.UpdateDate = DateTime.Now;
            //        invtran.UpdateUserName = request.CurrentDisplayName;
            //    }
            //}
        }

        /// <summary>
        /// 正常发货 取消出库
        /// </summary>
        /// <param name="outbound"></param>
        public void OutboundCancel_Shipment(outbound outbound, OutboundOperateDto request)
        {
            outbound.Status = (int)OutboundStatus.Picking;
            outbound.ActualShipDate = null;
            outbound.TotalShippedQty = 0;
            outbound.TotalPickedQty = outbound.TotalQty;

            _wmsSqlRepository.UpdateOutboundDetailForOutboundCancel_Shipment(outbound.SysId, OutboundDetailStatus.Picking, request.CurrentUserId, request.CurrentDisplayName);
            var pickdetails = _outboundRepository.GetQuery<pickdetail>(p => p.OutboundSysId == outbound.SysId && p.Status == (int)PickDetailStatus.Finish).ToList();
            if (pickdetails == null || pickdetails.Count == 0)
            {
                throw new Exception("拣货明细信息缺失，请检查");
            }

            var pickLoc = pickdetails.Select(q => q.Loc).ToList();

            //校验冻结
            var frozenLoc = _crudRepository.GetQuery<location>(p => pickLoc.Contains(p.Loc) && p.WarehouseSysId == request.WarehouseSysId && p.Status == (int)LocationStatus.Frozen).FirstOrDefault();
            //货位级别
            if (frozenLoc != null)
            {
                throw new Exception($"货位{frozenLoc.Loc}已被冻结，不能取消发货!");
            }
            var pickSku = pickdetails.Select(q => q.SkuSysId).ToList();
            var frozenSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.Sku && pickSku.Contains(p.SkuSysId.Value)
                             && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId);
            //商品级别
            if (frozenSkuList.Count() > 0)
            {
                var skuSysId = frozenSkuList.First().SkuSysId;
                var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).First();
                throw new Exception($"商品{frozenSku.SkuName}已被冻结，不能取消发货!");
            }

            var frozenLocSkuList = _crudRepository.GetQuery<stockfrozen>(p => p.Type == (int)FrozenType.LocSku && pickSku.Contains(p.SkuSysId.Value)
                                && p.Status == (int)FrozenStatus.Frozen && p.WarehouseSysId == request.WarehouseSysId).ToList();
            //校验冻结: 货位商品 
            if (frozenLocSkuList.Count > 0)
            {
                var locskuFrozenQuery = from T1 in pickdetails
                                        join T2 in frozenLocSkuList on new { T1.SkuSysId, T1.Loc } equals new { SkuSysId = T2.SkuSysId.Value, T2.Loc }
                                        select T2;

                if (locskuFrozenQuery.Count() > 0)
                {
                    var firstFrozenLocsku = locskuFrozenQuery.First();
                    var skuSysId = firstFrozenLocsku.SkuSysId;
                    var frozenSku = _crudRepository.GetQuery<sku>(p => p.SysId == skuSysId).FirstOrDefault();
                    throw new Exception($"商品'{frozenSku.SkuName}'在货位'{firstFrozenLocsku.Loc}'已被冻结，不能取消发货!");
                }
            }

            var invLotList = _outboundRepository.GetInvlotForOutboundCancel(outbound.SysId);
            var invSkuLocList = _outboundRepository.GetInvskulocForOutboundCancel(outbound.SysId);
            var invLotLocLpnList = _outboundRepository.GetInvlotloclpnForOutboundCancel(outbound.SysId);
            var updateInventoryDtos = new List<UpdateInventoryDto>();

            foreach (var pickdetail in pickdetails)
            {
                //入库存数量
                var invQty = pickdetail.Qty.Value;

                var invLot = invLotList.Where(x => x.Lot == pickdetail.Lot && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invSkuLoc = invSkuLocList.Where(x => x.Loc == pickdetail.Loc && x.SkuSysId == pickdetail.SkuSysId && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                var invLotLocLpn = invLotLocLpnList.Where(x => x.SkuSysId == pickdetail.SkuSysId && x.Lot == pickdetail.Lot && x.Loc == pickdetail.Loc && x.Lpn == pickdetail.Lpn && x.WareHouseSysId == outbound.WareHouseSysId).FirstOrDefault();
                updateInventoryDtos.Add(new UpdateInventoryDto()
                {
                    InvLotLocLpnSysId = invLotLocLpn.SysId,
                    InvLotSysId = invLot.SysId,
                    InvSkuLocSysId = invSkuLoc.SysId,
                    Qty = invQty,
                    CurrentUserId = request.CurrentUserId,
                    CurrentDisplayName = request.CurrentDisplayName,
                    WarehouseSysId = outbound.WareHouseSysId,
                });
            }

            //更新库存
            _wmsSqlRepository.UpdateInventoryForCancelOutboundShipment(updateInventoryDtos);
        }


        public OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            OutboundPrePackDiffDto rsp = new OutboundPrePackDiffDto();
            var prePack = _prePackCrudRepository.GetQuery<prepack>(p => p.OutboundSysId == outboundSysId).FirstOrDefault();
            var outboundDetails = _outboundRepository.GetOutboundDetails(outboundSysId);
            var prepackDetails = _prePackCrudRepository.GetPrePackDetailByOutboundSysId(outboundSysId);
            if (prePack != null)
            {
                rsp.StorageLoc = prePack.StorageLoc;
            }
            if (!prepackDetails.Any())
            {
                return rsp;
            }
            DiffDto diffDto = null;
            const string more = "预包装数量比出库单多{0}个 请移除";
            const string less = "预包装数量比出库单少{0}个 请补充";
            foreach (var outboundDetail in outboundDetails)
            {
                diffDto = null;
                var prepackDetail = prepackDetails.FirstOrDefault(p => p.UPC == outboundDetail.UPC);
                if (prepackDetail != null)
                {
                    if (prepackDetail.Qty != outboundDetail.DisplayQty)
                    {
                        diffDto = new DiffDto
                        {
                            UPC = outboundDetail.UPC,
                            SkuName = outboundDetail.SkuName,
                            SkuDescr = outboundDetail.SkuDescr,
                            UOMCode = outboundDetail.UOMCode,
                            OutboundDisplayQty = outboundDetail.DisplayQty,
                            PrePackDisplayQty = Convert.ToDecimal(prepackDetail.Qty),
                            MoreOrLess = prepackDetail.Qty > outboundDetail.DisplayQty,
                            Memo = string.Format(prepackDetail.Qty > outboundDetail.DisplayQty ? more : less, Math.Abs(outboundDetail.DisplayQty - prepackDetail.Qty.GetValueOrDefault()))
                        };
                    }
                    prepackDetails.Remove(prepackDetail);
                }
                else
                {
                    diffDto = new DiffDto
                    {
                        UPC = outboundDetail.UPC,
                        SkuName = outboundDetail.SkuName,
                        SkuDescr = outboundDetail.SkuDescr,
                        UOMCode = outboundDetail.UOMCode,
                        OutboundDisplayQty = outboundDetail.DisplayQty,
                        PrePackDisplayQty = decimal.Zero,
                        MoreOrLess = false,
                        Memo = string.Format(less, outboundDetail.DisplayQty)
                    };
                }
                if (diffDto != null)
                {
                    rsp.DetailDiffList.Add(diffDto);
                }

            }
            if (prepackDetails.Where(p => p.Qty.GetValueOrDefault() != 0).Any())
            {
                foreach (var prepackDetail in prepackDetails.Where(p => p.Qty.GetValueOrDefault() != 0))
                {
                    diffDto = new DiffDto
                    {
                        UPC = prepackDetail.UPC,
                        SkuName = prepackDetail.SkuName,
                        SkuDescr = prepackDetail.SkuDescr,
                        UOMCode = prepackDetail.UomCode,
                        OutboundDisplayQty = 0,
                        PrePackDisplayQty = Convert.ToDecimal(prepackDetail.Qty),
                        MoreOrLess = true,
                        Memo = string.Format(more, prepackDetail.Qty)
                    };
                    rsp.DetailDiffList.Add(diffDto);
                }
            }
            return rsp;
        }

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public OutboundPrePackDiffDto GetOutboundPreBulkPackDiff(Guid outboundSysId)
        {
            OutboundPrePackDiffDto rsp = new OutboundPrePackDiffDto();
            var preBulkPackSysIds = _crudRepository.GetQuery<prebulkpack>(p => p.OutboundSysId == outboundSysId).Select(p => p.SysId).ToList();
            if (!preBulkPackSysIds.Any())
            {
                return rsp;
            }
            var outboundDetails = _outboundRepository.GetOutboundDetails(outboundSysId);
            var preBulkPackDetails = _preBulkPackRepository.GetPreBulkPackDetailByPreBulkPackSysIds(preBulkPackSysIds);
            DiffDto diffDto = null;
            const string more = "箱数量比出库单多{0}个 请移除";
            const string less = "箱数量比出库单少{0}个 请补充";
            foreach (var outboundDetail in outboundDetails)
            {
                diffDto = null;
                var preBulkPackDetail = preBulkPackDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
                if (preBulkPackDetail != null)
                {
                    if (preBulkPackDetail.Qty != outboundDetail.DisplayQty)
                    {
                        diffDto = new DiffDto
                        {
                            UPC = outboundDetail.UPC,
                            SkuName = outboundDetail.SkuName,
                            SkuDescr = outboundDetail.SkuDescr,
                            UOMCode = outboundDetail.UOMCode,
                            OutboundDisplayQty = outboundDetail.DisplayQty,
                            PrePackDisplayQty = Convert.ToDecimal(preBulkPackDetail.Qty),
                            MoreOrLess = preBulkPackDetail.Qty > outboundDetail.DisplayQty,
                            Memo = string.Format(preBulkPackDetail.Qty > outboundDetail.DisplayQty ? more : less, Math.Abs(outboundDetail.DisplayQty - preBulkPackDetail.Qty))
                        };
                    }
                    preBulkPackDetails.Remove(preBulkPackDetail);
                }
                else
                {
                    diffDto = new DiffDto
                    {
                        UPC = outboundDetail.UPC,
                        SkuName = outboundDetail.SkuName,
                        SkuDescr = outboundDetail.SkuDescr,
                        UOMCode = outboundDetail.UOMCode,
                        OutboundDisplayQty = outboundDetail.DisplayQty,
                        PrePackDisplayQty = decimal.Zero,
                        MoreOrLess = false,
                        Memo = string.Format(less, outboundDetail.DisplayQty)
                    };
                }
                if (diffDto != null)
                {
                    rsp.DetailDiffList.Add(diffDto);
                }

            }
            if (preBulkPackDetails.Where(p => p.Qty != 0).Any())
            {
                foreach (var preBulkPackDetail in preBulkPackDetails.Where(p => p.Qty != 0))
                {
                    diffDto = new DiffDto
                    {
                        UPC = preBulkPackDetail.UPC,
                        SkuName = preBulkPackDetail.SkuName,
                        SkuDescr = preBulkPackDetail.SkuDescr,
                        UOMCode = preBulkPackDetail.UomCode,
                        OutboundDisplayQty = 0,
                        PrePackDisplayQty = Convert.ToDecimal(preBulkPackDetail.Qty),
                        MoreOrLess = true,
                        Memo = string.Format(more, preBulkPackDetail.Qty)
                    };
                    rsp.DetailDiffList.Add(diffDto);
                }
            }
            return rsp;
        }

        /// <summary>
        ///绑定预包装单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool BindPrePackOrder(OutboundBindQuery dto)
        {
            try
            {
                _crudRepository.ChangeDB(dto.WarehouseSysId);
                if (dto == null)
                {
                    throw new Exception("绑定信息不能为空!");
                }
                var info = _crudRepository.GetQuery<prepack>(x => x.PrePackOrder == dto.PrePackOrder && x.WareHouseSysId == dto.WarehouseSysId).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("预包装单：" + dto.PrePackOrder + " 不存在，请重新输入");
                }
                if (!string.IsNullOrEmpty(info.OutboundOrder) || info.OutboundSysId != null)
                {
                    throw new Exception("预包装单：" + dto.PrePackOrder + " 已经被绑定，不能重复绑定");
                }
                info.OutboundOrder = dto.OutboundOrder;
                info.OutboundSysId = dto.OutboundSysId;
                info.UpdateBy = dto.CurrentUserId;
                info.UpdateDate = DateTime.Now;
                info.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Update(info);

                //赋值预包装单的批号给出库单
                var model = _crudRepository.GetQuery<outbound>(x => x.SysId == dto.OutboundSysId).FirstOrDefault();
                if (model == null)
                {
                    throw new Exception("出库单查询出错!");
                }
                model.BatchNumber = info.BatchNumber;
                _crudRepository.Update(model);

                //修改散货装箱出库单号
                _wmsSqlRepository.UpdatePreBulkPackOutboundByBind(dto.OutboundSysId, dto.OutboundOrder, info.SysId);
            }
            catch (Exception ex)
            {
                throw new Exception("绑定失败：" + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 解绑预包装单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool UnBindPrePackOrder(OutboundBindQuery dto)
        {
            try
            {
                _crudRepository.ChangeDB(dto.WarehouseSysId);
                if (dto == null)
                {
                    throw new Exception("解绑信息不能为空！");
                }

                //清空出库单对应的预包装批号
                var outboundInfo = _crudRepository.GetQuery<outbound>(x => x.SysId == dto.OutboundSysId).FirstOrDefault();
                if (outboundInfo == null)
                {
                    throw new Exception("要解绑出库单不存在！");
                }
                outboundInfo.BatchNumber = null;
                _crudRepository.Update(outboundInfo);



                var info = _crudRepository.GetQuery<prepack>(x => x.PrePackOrder == dto.PrePackOrder && x.OutboundSysId == dto.OutboundSysId).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("预包装单：" + dto.PrePackOrder + " 不存在，请重新输入");
                }
                info.OutboundOrder = null;
                info.OutboundSysId = null;
                info.UpdateBy = dto.CurrentUserId;
                info.UpdateDate = DateTime.Now;
                info.UpdateUserName = dto.CurrentDisplayName;
                _crudRepository.Update(info);

                //修改散货装箱出库单号
                _wmsSqlRepository.UpdatePreBulkPackOutboundByUnBind(info.SysId);
            }
            catch (Exception ex)
            {
                throw new Exception("解绑失败：" + ex.Message);
            }
            return true;
        }

        [UnitOfWork(isTransactional: false)]
        public CommonResponse CreateOutboundByMQ(MQProcessDto<ThirdPartyOutboundDto> request)
        {
            CommonResponse rsp = new CommonResponse { IsSuccess = true };
            try
            {
                _crudRepository.ChangeDB(Guid.Parse(request.BussinessDto.WareHouseSysId));
                var outboundSysId = request.BussinessSysId;
                string outboundOrder = request.BussinessOrderNumber;
                ThirdPartyOutboundDto outboundDto = request.BussinessDto;
                warehouse warehouse = _crudRepository.GetQuery<warehouse>(p => p.OtherId == outboundDto.WareHouseSysId).FirstOrDefault();
                if (outboundDto.OutboundDetailDtoList != null && outboundDto.OutboundDetailDtoList.Any())
                {
                    List<string> otherSkuIdList = outboundDto.OutboundDetailDtoList.Select(p => p.OtherSkuId).ToList();
                    List<sku> skuList = _crudRepository.GetQuery<sku>(p => otherSkuIdList.Contains(p.OtherId)).ToList();
                    List<Guid> skuClassSysIdList = skuList.Select(p => p.SkuClassSysId).ToList();
                    List<Guid> packSysIdList = skuList.Select(p => p.PackSysId).ToList();
                    List<skuclass> skuClassList = _crudRepository.GetAllList<skuclass>(p => skuClassSysIdList.Contains(p.SysId));
                    List<pack> packList = _crudRepository.GetQuery<pack>(p => packSysIdList.Contains(p.SysId)).ToList();
                    List<Guid?> fieldUom01List = packList.Select(p => p.FieldUom01).ToList();
                    List<uom> uomList = _crudRepository.GetQuery<uom>(p => fieldUom01List.Contains(p.SysId)).ToList();
                    if (new InsertOutboundDetailCheck(rsp, outboundDto.OutboundDetailDtoList, skuList, skuClassList, packList, uomList).Execute().IsSuccess)
                    {
                        #region 出库单明细
                        List<outbounddetail> outboundDetails = new List<outbounddetail>();

                        #region 组织B2C赠品数据
                        if (outboundDto.OutboundType == (int)OutboundType.B2C)
                        {
                            var skuOtherIds = outboundDto.OutboundDetailDtoList.GroupBy(p => new { p.OtherSkuId }).Select(p => new ThirdPartyOutboundDetailDto()
                            {
                                OtherSkuId = p.Key.OtherSkuId
                            }).ToList();

                            var newOutboundDetailList = new List<ThirdPartyOutboundDetailDto>();
                            foreach (var otherId in skuOtherIds)
                            {
                                var newlits = outboundDto.OutboundDetailDtoList.Where(x => x.OtherSkuId == otherId.OtherSkuId).ToList();
                                var detailDto = new ThirdPartyOutboundDetailDto();
                                if (newlits.Count() > 1)
                                {
                                    detailDto.OtherSkuId = otherId.OtherSkuId;
                                    detailDto.PackFactor = newlits[0].PackFactor;
                                    detailDto.Price = newlits[0].Price;
                                    detailDto.IsGift = false;
                                    detailDto.Qty = newlits.Sum(x => x.Qty);

                                    var giftQty = newlits.Where(x => x.IsGift == true).Sum(x => x.Qty);
                                    detailDto.GiftQty = giftQty.HasValue ? giftQty.Value : 0;
                                }
                                else
                                {
                                    detailDto = newlits[0];
                                    if (detailDto.IsGift)
                                    {
                                        detailDto.GiftQty = detailDto.Qty.Value;
                                    }
                                }
                                newOutboundDetailList.Add(detailDto);
                            }

                            outboundDto.OutboundDetailDtoList = newOutboundDetailList;
                        }
                        #endregion


                        foreach (var outboundDetailDto in outboundDto.OutboundDetailDtoList)
                        {
                            sku sku = skuList.FirstOrDefault(p => p.OtherId == outboundDetailDto.OtherSkuId);
                            skuclass skuClass = null;
                            pack pack = null;
                            uom uom = null;
                            if (sku != null)
                            {
                                skuClass = skuClassList.FirstOrDefault(p => p.SysId == sku.SkuClassSysId);
                                pack = packList.FirstOrDefault(p => p.SysId == sku.PackSysId);
                                if (pack != null)
                                {
                                    uom = uomList.FirstOrDefault(p => p.SysId == pack.FieldUom01);
                                }
                            }
                            outbounddetail outboundDetail = new outbounddetail
                            {
                                SysId = Guid.NewGuid(),
                                OutboundSysId = outboundSysId,
                                SkuSysId = sku.SysId,
                                UOMSysId = uom.SysId,
                                PackSysId = pack.SysId,
                                Qty = outboundDetailDto.Qty,
                                Price = outboundDetailDto.Price,
                                Status = (int)OutboundDetailStatus.New,
                                CreateBy = 99999,
                                CreateDate = DateTime.Now,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                                PackFactor = outboundDetailDto.PackFactor,
                                //是否赠品
                                IsGift = outboundDetailDto.IsGift,
                                GiftQty = 0
                            };
                            outboundDetails.Add(outboundDetail);
                        }
                        #endregion

                        #region 出库单
                        outbound outbound = null;

                        if (new InsertOutboundCheck(rsp, warehouse).Execute().IsSuccess)
                        {
                            //获取之前业务的单号
                            Guid? receiptSysId = null;
                            string purchaseOrder = string.Empty;
                            if (!string.IsNullOrEmpty(outboundDto.ExternPurchaseOrder))
                            {
                                var purchase =
                                    _crudRepository.GetQuery<purchase>(
                                        x => x.ExternalOrder == outboundDto.ExternPurchaseOrder).FirstOrDefault();
                                if (purchase != null)
                                {
                                    receiptSysId = purchase.SysId;
                                    purchaseOrder = purchase.PurchaseOrder;
                                }
                            }

                            outbound = new outbound
                            {
                                SysId = outboundSysId,
                                OutboundOrder = outboundOrder,
                                WareHouseSysId = warehouse.SysId,
                                RequestedShipDate = outboundDto.RequestedShipDate,
                                ActualShipDate = outboundDto.ActualShipDate,
                                DeliveryDate = outboundDto.DeliveryDate,
                                OutboundType = outboundDto.OutboundType,
                                OutboundChildType = outboundDto.OutboundChildType,
                                Status = (int)OutboundStatus.New,
                                AuditingDate = outboundDto.AuditingDate,
                                AuditingBy = outboundDto.AuditingBy,
                                AuditingName = outboundDto.AuditingName,
                                OutboundDate = outboundDto.ExternOrderDate,
                                ExternOrderDate = outboundDto.ExternOrderDate,
                                ExternOrderId = outboundDto.ExternOrderId,
                                ConsigneeName = outboundDto.ConsigneeName,
                                ConsigneeAddress = (outboundDto.OutboundType == (int)OutboundType.B2C && !string.IsNullOrEmpty(outboundDto.ConsigneeAddress)) ? outboundDto.ConsigneeAddress.Replace(outboundDto.ConsigneeProvince + outboundDto.ConsigneeCity + outboundDto.ConsigneeArea, "") : outboundDto.ConsigneeAddress,
                                ConsigneeProvince = outboundDto.ConsigneeProvince,
                                ConsigneeCity = (outboundDto.ConsigneeCity == "县" || outboundDto.ConsigneeCity == "市辖区") ? outboundDto.ConsigneeProvince : outboundDto.ConsigneeCity,
                                ConsigneeArea = outboundDto.ConsigneeArea,
                                ConsigneePhone = string.IsNullOrEmpty(outboundDto.ConsigneeCellPhone) ? outboundDto.ConsigneePhone : outboundDto.ConsigneeCellPhone,
                                ConsigneeTown = outboundDto.ConsigneeTown,
                                ConsigneeVillage = outboundDto.ConsigneeVillage,
                                PostalCode = outboundDto.PostalCode,
                                CashOnDelivery = outboundDto.CashOnDelivery,
                                ShippingMethod = outboundDto.ShippingMethod,
                                TotalQty = outboundDto.TotalQty,
                                InvoiceType = outboundDto.InvoiceType,
                                Freight = outboundDto.Freight,
                                Source = outboundDto.Source,
                                ServiceStationName = outboundDto.ServiceStationName,
                                Remark = outboundDto.Remark,
                                TotalPrice = outboundDto.TotalPrice,
                                CreateBy = 99999,
                                CreateDate = DateTime.Now,
                                UpdateBy = 99999,
                                UpdateDate = DateTime.Now,
                                Channel = outboundDto.Channel,
                                BatchNumber = outboundDto.BatchNumber,
                                ReceiptSysId = receiptSysId,
                                PurchaseOrder = purchaseOrder,
                                //新增平台订单号和订单折扣
                                PlatformOrder = outboundDto.PlatformOrder,
                                DiscountPrice = outboundDto.DiscountPrice,
                                //新增服务站编码,是否开票，优惠券价格
                                ServiceStationCode = outboundDto.ServiceStationCode,
                                IsInvoice = outboundDto.HasInvoice,
                                CouponPrice = outboundDto.CouponAmount
                            };


                            //var city = outboundDto.ConsigneeProvince + outboundDto.ConsigneeCity;
                            //var address = outboundDto.ConsigneeAddress;
                            //var coordinate = _baseAppService.GetCoordinate(city, address);
                            //if (coordinate != null && coordinate.Status == 0 && coordinate.Result != null && coordinate.Result.location != null)
                            //{
                            //    outbound.Lat = coordinate.Result.location.lat;
                            //    outbound.Lng = coordinate.Result.location.lng;
                            //}

                            TransactionOptions transactionOption = new TransactionOptions();
                            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                            using (
                                TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                    transactionOption))
                            {
                                _crudRepository.Insert(outbound);
                                _crudRepository.SaveChange();
                                _wmsSqlRepository.ThirdPartyInsertOutboundDetail(outboundDetails);
                                scope.Complete();
                            }

                            #region 推送匹配预包装MQ

                            if (outbound.OutboundType != (int)OutboundType.Normal
                                && outbound.OutboundType != (int)OutboundType.B2C
                                && outbound.OutboundType != (int)OutboundType.Return)
                            {
                                MQOrderRuleDto mqOrderRuleDto = new MQOrderRuleDto()
                                {
                                    OrderSysId = outbound.SysId,
                                    OrderNumber = outbound.OutboundOrder,
                                    WarehouseSysId = outbound.WareHouseSysId
                                };
                                MQProcessDto<MQOrderRuleDto> mqDto = new MQProcessDto<MQOrderRuleDto>()
                                {
                                    BussinessDto = mqOrderRuleDto,
                                    BussinessSysId = outbound.SysId,
                                    BussinessOrderNumber = outbound.OutboundOrder,
                                    CurrentUserId = 99999,
                                    CurrentDisplayName = "ECC",
                                    WarehouseSysId = outbound.WareHouseSysId
                                };
                                RabbitWMS.SetRabbitMQAsync(RabbitMQType.InsertOutbound_Prepack, mqDto);
                            }

                            #endregion
                        }
                        else
                        {
                            return rsp;
                        }

                        #endregion

                        //发送邮件通知
                        Guid sysCodeSysId = _crudRepository.FirstOrDefault<syscode>(p => p.SysCodeType == PublicConst.SysCodeTypeReceiptOutboundMail).SysId;
                        string mailTo = _crudRepository.FirstOrDefault<syscodedetail>(p => p.SysCodeSysId == sysCodeSysId && p.Code == "OutboundMail").Descr;
                        EmailHelper.SendMailAsync(PublicConst.NewOutboundSubject, string.Format(PublicConst.NewOutboundMailBody, outbound.OutboundOrder, outbound.OutboundDate), mailTo);
                        MQProcessDto<MQOrderRuleDto> bussinessProcessLogDto = new MQProcessDto<MQOrderRuleDto>()
                        {
                            BussinessSysId = outboundSysId,
                            BussinessOrderNumber = outboundOrder,
                            Descr = "出库单自动分配",
                            CurrentUserId = 99999,
                            CurrentDisplayName = "WMSSystem",
                            WarehouseSysId = warehouse.SysId,
                            BussinessDto = new MQOrderRuleDto()
                            {
                                OrderSysId = outboundSysId,
                                OrderNumber = outboundOrder,
                                CurrentUserId = 99999,
                                CurrentDisplayName = "WMSSystem",
                                WarehouseSysId = warehouse.SysId
                            }
                        };
                        RabbitWMS.SetRabbitMQAsync(RabbitMQType.Outbound_AutoAllocation, bussinessProcessLogDto);

                    }
                    else
                    {
                        return rsp;
                    }
                }
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }

            return rsp;
        }

        /// <summary>
        /// 打印交接单号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TMSBoxNumberDto AddTMSBoxNumber(BatchTMSBoxNumberDto dto)
        {
            var result = new TMSBoxNumberDto() { OutboundPreBulkPackDto = new List<OutboundPreBulkPackDto>() };
            try
            {
                _crudRepository.ChangeDB(dto.WarehouseSysId);
                if (dto == null)
                {
                    throw new Exception("打印信息不能为空！");
                }
                if (dto.BoxNumberList == null || dto.BoxNumberList.Count <= 0)
                {
                    throw new Exception("交接单号列表不能为空！");
                }

                var outbound = _crudRepository.GetQuery<outbound>(x => x.SysId == dto.OutboundSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("交接单的出库单不存在！");
                }

                var list = _crudRepository.GetAllList<outboundtransferorder>(x => x.OutboundSysId == dto.OutboundSysId).ToList();
                var hasRow = list.Where(x => dto.BoxNumberList.Contains(x.BoxNumber)).ToList();
                if (hasRow != null && hasRow.Count > 0)
                {
                    throw new Exception("交接单号为:" + hasRow[0].BoxNumber + "已经存在！");
                }
                var insertList = new List<outboundtransferorder>();
                var numberList = new List<string>();
                foreach (var item in dto.BoxNumberList)
                {
                    var model = new outboundtransferorder()
                    {
                        SysId = Guid.NewGuid(),
                        OutboundOrder = dto.OutboundOrder,
                        OutboundSysId = dto.OutboundSysId,
                        BoxNumber = item,
                        CreateBy = dto.CurrentUserId,
                        CreateDate = DateTime.Now,
                        CreateUserName = dto.CurrentDisplayName,

                        UpdateBy = dto.CurrentUserId,
                        UpdateDate = DateTime.Now,
                        UpdateUserName = dto.CurrentDisplayName,

                        ConsigneeArea = outbound.ConsigneeArea,
                        ServiceStationName = outbound.ServiceStationName,
                        Status = (int)OutboundTransferOrderStatus.New,
                        WareHouseSysId = dto.WarehouseSysId,
                        TransferOrder = dto.OutboundOrder + "-" + item
                    };
                    insertList.Add(model);
                    numberList.Add(model.TransferOrder);
                    result.OutboundPreBulkPackDto.Add(new OutboundPreBulkPackDto() { BoxNumber = item });
                }

                #region 打印箱号之后将散货箱信息同步
                //获取未绑定散货箱的交接单数据 
                //var outboundtransferorderList = list.Where(x => x.PreBulkPackSysId != null).ToList();

                ////获取出库单相关所有的散货封箱单
                ////var preBulkList = _preBulkPackRepository.GetOutboundPreBulkPackList(dto.OutboundSysId);
                ////if (preBulkList != null && preBulkList.Count > 0)
                ////{
                //foreach (var item in insertList)
                //{
                //    var resultDto = new OutboundPreBulkPackDto() { BoxNumber = item.BoxNumber };
                //    foreach (var preBulk in preBulkList)
                //    {
                //        var preBulkPackModel = outboundtransferorderList.Find(x => x.PreBulkPackSysId == preBulk.PreBulkPackSysId);
                //        if (preBulkPackModel != null)
                //        {
                //            continue;
                //        }
                //        //item.PreBulkPackOrder = preBulk.PreBulkPackOrder;
                //        //item.PreBulkPackSysId = preBulk.PreBulkPackSysId;
                //        item.SkuQty = preBulk.SkuQty;
                //        item.Qty = preBulk.Qty;

                //        resultDto.SkuQty = preBulk.SkuQty;
                //        resultDto.Qty = preBulk.Qty;

                //        preBulkList.Remove(preBulk);
                //        break;
                //    }
                //    result.OutboundPreBulkPackDto.Add(resultDto);
                //}
                //}
                #endregion

                _crudRepository.BatchInsert<outboundtransferorder>(insertList);

                #region 出库单生成交接单号之后调用TMS:推送交接单号
                if (outbound.OutboundType == (int)OutboundType.B2B || outbound.OutboundType == (int)OutboundType.Fertilizer || outbound.OutboundType == (int)OutboundType.TransferInventory)
                {
                    var thirdPreBullPackDto = new ThirdPreBullPackDto()
                    {
                        OrderId = outbound.ExternOrderId,
                        OutboundSysId = dto.OutboundSysId,
                        OutboundOrder = dto.OutboundOrder,
                        StorageCases = numberList,
                        CreateDate = DateTime.Now,
                        CreateUserName = dto.CurrentDisplayName,
                        CurrentUserId = dto.CurrentUserId,
                        OrderType = (int)TMSOrderType.B2BOrder
                    };

                    result.ServiceStationName = outbound.ServiceStationName;
                    result.ConsigneeArea = !string.IsNullOrEmpty(outbound.ConsigneeArea) ? outbound.ConsigneeArea : "";
                    result.ConsigneeTown = !string.IsNullOrEmpty(outbound.ConsigneeTown) ? outbound.ConsigneeTown : "";
                    result.OutboundChildType = outbound.OutboundChildType;


                    //如果是移仓出库单，推送TMS时的订单号取移仓单的外部单据号
                    if (outbound.OutboundType == (int)OutboundType.TransferInventory)
                    {
                        var info = _crudRepository.GetQuery<transferinventory>(x => x.TransferInventoryOrder == outbound.ExternOrderId).First();
                        if (info == null)
                        {
                            throw new Exception("根据单号:" + outbound.ExternOrderId + "未找到相关移仓单！");
                        }
                        thirdPreBullPackDto.OrderId = info.ExternOrderId;
                        thirdPreBullPackDto.OrderType = (int)TMSOrderType.TransferOrder;

                        result.ServiceStationName = info.ToWareHouseName;
                        result.ConsigneeArea = "";
                        result.ConsigneeTown = "";
                        result.OutboundChildType = "移仓";
                    }
                    _thirdPartyAppService.PreBullPackSendToTMS(thirdPreBullPackDto);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("打印失败：" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// TMS推送总箱数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse CreateTMSBoxCount(BatchTMSBoxNumberDto dto)
        {

            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var rsp = new CommonResponse() { IsSuccess = true };
            try
            {
                var outbound = _crudRepository.GetQuery<outbound>(x => x.SysId == dto.OutboundSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("获取出库单信息异常");
                }
                var pushDto = new ThirdPartyPushBoxCountDto()
                {
                    OutboundSysId = outbound.SysId,
                    OutboundOrder = outbound.OutboundOrder,
                    OrderId = outbound.ExternOrderId,
                    CurrentUserId = dto.CurrentUserId,
                    EditUserName = dto.CurrentDisplayName,
                    Quantity = dto.BoxCount,
                    OrderType = (int)TMSOrderType.B2BOrder
                };

                if (outbound.OutboundType == (int)OutboundType.TransferInventory)
                { //如果是移仓出库单，推送TMS时的订单号取移仓单的外部单据号
                    var info = _crudRepository.GetQuery<transferinventory>(x => x.TransferInventoryOrder == outbound.ExternOrderId).First();
                    if (info == null)
                    {
                        throw new Exception("根据单号:" + outbound.ExternOrderId + "未找到相关移仓单！");
                    }
                    pushDto.OrderId = info.ExternOrderId;
                    pushDto.OrderType = (int)TMSOrderType.TransferOrder;
                }

                rsp = _thirdPartyAppService.PushBoxCount(pushDto);
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }

            return rsp;
        }

        /// <summary>
        /// 获取库存不足商品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public List<InsufficientStockSkuListDto> GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            return _outboundRepository.GetInsufficientStockSkuList(dto);

        }

        /// <summary>
        /// 增加异常记录数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CommonResponse AddOutboundExceptionService(AddOutboundExceptionDto dto)
        {
            _crudRepository.ChangeDB(dto.WarehouseSysId);
            var rsp = new CommonResponse() { IsSuccess = true };
            try
            {

                if (dto == null || dto.OutboundExceptionDtoList.Count == 0)
                {
                    throw new Exception("提交数据不能为空");
                }
                var outbound = _crudRepository.GetQuery<outbound>(x => x.SysId == dto.OutboundSysId).FirstOrDefault();
                if (outbound == null)
                {
                    throw new Exception("未找到对应的出库单");
                }
                if (outbound.Exception == null || outbound.Exception == false)
                {
                    outbound.Exception = true;
                    _crudRepository.Update<outbound>(outbound);
                }

                var list = _crudRepository.GetAllList<outboundexception>(x => x.OutboundSysId == dto.OutboundSysId).ToList();
                foreach (var item in dto.OutboundExceptionDtoList)
                {
                    var flag = true;
                    foreach (var model in list)
                    {
                        if (model.SysId == item.SysId && item.SysId != new Guid())
                        {
                            model.ExceptionReason = item.ExceptionReason;
                            model.ExceptionQty = item.ExceptionQty;
                            model.ExceptionDesc = item.ExceptionDesc;
                            model.Result = item.Result;
                            model.Department = item.Department;
                            model.Responsibility = item.Responsibility;
                            model.Remark = item.Remark;
                            model.IsSettlement = item.IsSettlement;
                            model.UpdateBy = dto.CurrentUserId;
                            model.UpdateUserName = dto.CurrentDisplayName;
                            model.UpdateDate = DateTime.Now;
                            flag = false;
                            _crudRepository.Update<outboundexception>(model);
                        }
                    }
                    if (flag)
                    {
                        var outboundexception = new outboundexception();
                        outboundexception = item.TransformTo<outboundexception>();
                        outboundexception.SysId = Guid.NewGuid();
                        outboundexception.CreateBy = outboundexception.UpdateBy = dto.CurrentUserId;
                        outboundexception.CreateDate = outboundexception.UpdateDate = DateTime.Now;
                        outboundexception.CreateUserName = outboundexception.UpdateUserName = dto.CurrentDisplayName;
                        _crudRepository.Insert<outboundexception>(outboundexception);
                    }
                }
                rsp.ErrorMessage = "异常记录保存成功";
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public CommonResponse DeleteOutboundException(List<Guid> request, Guid warehouseSysId, Guid outboundSysId)
        {
            _crudRepository.ChangeDB(warehouseSysId);
            var rsp = new CommonResponse() { IsSuccess = true };
            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    _crudRepository.Delete<outboundexception>(request);
                    _crudRepository.SaveChange();

                    var exceptionList = _crudRepository.GetAllList<outboundexception>(x => x.OutboundSysId == outboundSysId).ToList();
                    if (exceptionList.Count == 0)
                    {
                        var outbound = _crudRepository.GetQuery<outbound>(x => x.SysId == outboundSysId).FirstOrDefault();
                        if (outbound == null)
                        {
                            throw new Exception("出库单异常，删除失败");
                        }
                        outbound.Exception = false;
                        _crudRepository.Update<outbound>(outbound);
                    }
                    scope.Complete();
                }


                rsp.ErrorMessage = "删除异常记录成功";
            }
            catch (Exception ex)
            {
                rsp.IsSuccess = false;
                rsp.ErrorMessage = ex.Message;
            }
            return rsp;
        }

        /// <summary>
        /// 关闭或作为入库单时 ，回写更新outbound 对应的 returnqty
        /// </summary>
        /// <param name="request"></param>
        public void CancelOutboundReturnByPurchase(PurchaseForReturnDto request)
        {
            if (!request.FromWareHouseSysId.HasValue)
            {
                throw new Exception("退货出库单仓库不存在");
            }
            _crudRepository.ChangeDB(request.FromWareHouseSysId.Value);

            var outbound = _crudRepository.GetQuery<outbound>(x => x.SysId == request.OutboundSysId).FirstOrDefault();
            if (outbound == null)
            {
                throw new Exception("出库单不存在");
            }

            if (request.purchasedetails != null && request.purchasedetails.Count > 0)
            {
                _outboundRepository.CancelOutboundReturnByPurchase(request);
            }

        }

        /// <summary>
        /// 异步回写出库单退货数量
        /// </summary>
        /// <param name="request"></param>
        public OutboundReturnDto AddOutboundReturnQtyByPurchase(OutboundReturnDto request)
        {
            _crudRepository.ChangeDB(request.WarehouseSysId);
            var outbound = _crudRepository.GetQuery<outbound>(x => x.ExternOrderId == request.ExternOrderId).FirstOrDefault();

            if (outbound != null && request.OutboundReturnDetailDtoList != null && request.OutboundReturnDetailDtoList.Count > 0)
            {
                List<string> otherSkuIdList = request.OutboundReturnDetailDtoList.Select(p => p.OtherSkuId).ToList();
                List<sku> skuList = _crudRepository.GetAllList<sku>(p => otherSkuIdList.Contains(p.OtherId));

                foreach (var item in request.OutboundReturnDetailDtoList)
                {
                    sku sku = skuList.FirstOrDefault(p => p.OtherId == item.OtherSkuId);
                    var outbounddetail = _crudRepository.GetQuery<outbounddetail>(o => o.OutboundSysId == outbound.SysId && o.SkuSysId == sku.SysId).FirstOrDefault();
                    if (outbounddetail.ReturnQty + item.Qty > outbounddetail.ShippedQty)
                    {
                        throw new Exception("商品: " + sku.SkuName + " 可退货数量不足");
                    }
                    outbounddetail.ReturnQty += item.Qty;
                    outbounddetail.UpdateBy = request.CurrentUserId;
                    outbounddetail.UpdateUserName = request.CurrentDisplayName;
                    outbounddetail.UpdateDate = DateTime.Now;
                    _crudRepository.Update(outbounddetail);
                }
                request.OutboundSysId = outbound.SysId;
                request.OutboundOrder = outbound.OutboundOrder;
                //修改对应的出库单状态

                if (outbound.OutboundType == (int)OutboundType.B2C)
                {
                    outbound.IsReturn = (int)OutboundReturnStatus.B2CReturn;
                }
                else
                {
                    outbound.IsReturn = (int)OutboundReturnStatus.PartReturn;
                }
                outbound.UpdateDate = DateTime.Now;
                outbound.UpdateBy = request.CurrentUserId;
                outbound.UpdateUserName = request.CurrentDisplayName;
                _crudRepository.Update(outbound);

            }

            return request;
        }
    }
}

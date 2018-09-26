using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Other;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;

namespace NBK.ECService.WMSReport.Application
{
    public class OtherReadAppService : ApplicationService, IOtherReadAppService
    {
        private IOtherReadRepository _crudRepository = null;
        public OtherReadAppService(IOtherReadRepository crudRepository)
        {
            this._crudRepository = crudRepository;

        }


        #region 出库
        public Pages<OutboundListDto> GetOutboundByPage(OutboundQuery request)
        {
            this._crudRepository.ChangeDB(request.WarehouseSysId);
            var response = _crudRepository.GetOutboundByPage(request);
            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {
                List<Guid> outboundSysIds = response.TableResuls.aaData.Select(p => p.SysId).ToList();
                var outboundDetailList = _crudRepository.GetOutboundDetailBySummary(outboundSysIds, request.WarehouseSysId);
                foreach (var item in response.TableResuls.aaData)
                {
                    var detail = outboundDetailList.Find(x => x.SysId == item.SysId);
                    if (detail != null)
                    {
                        item.DisplayTotalQty = detail.DisplayTotalQty;
                    }
                    else
                    {
                        item.DisplayTotalQty = 0;
                    }
                }
            }
            return response;
        }

        public OutboundViewDto GetOutboundBySysId(Guid sysid, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            var response = _crudRepository.GetOutboundBySysId(sysid, warehouseSysId);
            response.OutboundDetailList = _crudRepository.GetOutboundDetails(sysid, warehouseSysId);
            response.DisplayTotalQty = response.OutboundDetailList.Sum(p => p.DisplayQty);
            return response;
        }

        /// <summary>
        /// 分页获取出库单明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundExceptionDto> GetOutboundDetailList(OutboundExceptionQueryDto request)
        {
            this._crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetOutboundDetailList(request);
        }

        public OutboundPrePackDiffDto GetOutboundPrePackDiff(Guid outboundSysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            OutboundPrePackDiffDto rsp = new OutboundPrePackDiffDto();
            var prePack = _crudRepository.GetQuery<prepack>(p => p.OutboundSysId == outboundSysId, warehouseSysId).FirstOrDefault();
            var outboundDetails = _crudRepository.GetOutboundDetails(outboundSysId, warehouseSysId);
            var prepackDetails = _crudRepository.GetPrePackDetailByOutboundSysId(outboundSysId, warehouseSysId);
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
        public OutboundPrePackDiffDto GetOutboundPreBulkPackDiff(Guid outboundSysId, Guid wareHouseSysId)
        {
            OutboundPrePackDiffDto rsp = new OutboundPrePackDiffDto();
            var transferOrderSysIds = _crudRepository.GetQuery<outboundtransferorder>(p => p.OutboundSysId == outboundSysId).Select(p => p.SysId).ToList();
            if (!transferOrderSysIds.Any())
            {
                return rsp;
            }
            var outboundDetails = _crudRepository.GetOutboundDetails(outboundSysId, wareHouseSysId);
            var transferOrderDetails = _crudRepository.GetTransferOrderDetailBySysIds(transferOrderSysIds);
            DiffDto diffDto = null;
            const string more = "箱数量比出库单多{0}个 请移除";
            const string less = "箱数量比出库单少{0}个 请补充";
            foreach (var outboundDetail in outboundDetails)
            {
                diffDto = null;
                var preBulkPackDetail = transferOrderDetails.FirstOrDefault(p => p.SkuSysId == outboundDetail.SkuSysId);
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
                    transferOrderDetails.Remove(preBulkPackDetail);
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
            if (transferOrderDetails.Where(p => p.Qty != 0).Any())
            {
                foreach (var preBulkPackDetail in transferOrderDetails.Where(p => p.Qty != 0))
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
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public OutboundBoxListDto GetOutboundBox(Guid outboundSysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            var outboundBoxListDto = new OutboundBoxListDto();

            try
            {
                var rslt = new List<OutboundBoxDto>();

                var outboundBoxList = _crudRepository.GetOutboundBox(outboundSysId, warehouseSysId);
                if (outboundBoxList != null && outboundBoxList.Count > 0)
                {
                    //未装箱，无包装商品
                    var noBoxSkuList = new List<OutboundBoxDto>();

                    #region 交接箱数据
                    var prebulkList = outboundBoxList.Where(x => x.CaseQty > 0).GroupBy(x => new { x.BoxSysId, x.BoxName }).Select(x => new OutboundBoxDto
                    {
                        BoxSysId = x.Key.BoxSysId,
                        BoxName = x.Key.BoxName,
                        BoxType = "散件装箱",
                        BoxSkuQty = x.Sum(g => g.CaseQty),
                        BoxSkuCount = x.Count()
                    });
                    rslt.AddRange(prebulkList);
                    #endregion

                    #region 整件+整件有剩余的
                    var prebulkSkuList = from od in outboundBoxList
                                         join pb in prebulkList on od.BoxName equals pb.BoxName
                                         group od.SkuSysId by od.SkuSysId into g
                                         select g.Key;

                    var packSkuList = outboundBoxList.Where(x => !prebulkSkuList.Contains(x.SkuSysId)).ToList();

                    //计算整件，整件剩余
                    var packBoxList = GetPackBoxList(packSkuList, ref rslt);
                    if (packBoxList != null && packBoxList.Count > 0)
                    {
                        noBoxSkuList.AddRange(packBoxList);
                    }
                    #endregion

                    #region 散件装箱剩余
                    var skuList = outboundBoxList.Where(x => prebulkSkuList.Contains(x.SkuSysId)).GroupBy(x => new { x.SkuSysId, x.SkuName, x.Qty, x.FieldValue02, x.FieldValue03 }).Select(x => new OutboundBoxDto
                    {
                        SkuSysId = x.Key.SkuSysId,
                        SkuName = x.Key.SkuName,
                        FieldValue02 = x.Key.FieldValue02,
                        FieldValue03 = x.Key.FieldValue03,
                        Qty = x.Key.Qty,
                    }).ToList();

                    var skuCaseList = outboundBoxList.Where(x => prebulkSkuList.Contains(x.SkuSysId)).GroupBy(x => new { x.SkuSysId, x.SkuName }).Select(x => new OutboundBoxDto
                    {
                        SkuSysId = x.Key.SkuSysId,
                        BoxName = x.Key.SkuName,
                        CaseQty = x.Sum(g => g.CaseQty)
                    }).ToList();

                    var remainCaseList = (from s in skuList
                                          join sc in skuCaseList on s.SkuSysId equals sc.SkuSysId
                                          where (s.Qty - s.CaseQty) > 0
                                          select new OutboundBoxDto
                                          {
                                              SkuSysId = s.SkuSysId,
                                              SkuName = s.SkuName,
                                              Qty = s.Qty - sc.CaseQty,
                                              FieldValue02 = s.FieldValue02,
                                              FieldValue03 = s.FieldValue03
                                          }).ToList();

                    var remainPackBoxList = GetPackBoxList(remainCaseList, ref rslt);
                    if (remainPackBoxList != null && remainPackBoxList.Count > 0)
                    {
                        noBoxSkuList.AddRange(remainPackBoxList);
                    }

                    //将未装箱加入集合
                    rslt.AddRange(noBoxSkuList);
                    #endregion
                }

                outboundBoxListDto.CaseCount += rslt.Where(x => x.BoxType == "散件装箱").Count();
                outboundBoxListDto.CaseCount += rslt.Where(x => x.BoxType == "整件").Sum(x => x.BoxSkuCount);
                outboundBoxListDto.Qty = rslt.Where(x => x.BoxType == "未装箱").Sum(x => x.BoxSkuQty);
                outboundBoxListDto.OutboundBoxDtoList = rslt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return outboundBoxListDto;
        }

        /// <summary>
        /// 计算整件
        /// </summary>
        /// <param name="packSkuList"></param>
        /// <param name="rslt"></param>
        /// <returns></returns>
        private List<OutboundBoxDto> GetPackBoxList(List<OutboundBoxDto> packSkuList, ref List<OutboundBoxDto> rslt)
        {
            var noBoxSkuList = new List<OutboundBoxDto>();
            foreach (var info in packSkuList)
            {
                var packQty = 0;
                if (info.FieldValue03 != null)
                {
                    packQty = (int)info.FieldValue03;
                }
                else if (info.FieldValue02 != null)
                {
                    packQty = (int)info.FieldValue02;
                }

                if (packQty > 0)
                {
                    var packSku = new OutboundBoxDto();
                    packSku.BoxName = info.SkuName;
                    packSku.BoxType = "整件";
                    packSku.BoxSkuCount = Convert.ToInt32(info.Qty / packQty);
                    packSku.BoxSkuQty = packQty * packSku.BoxSkuCount;
                    info.Qty -= packSku.BoxSkuQty;
                    if (packSku.BoxSkuCount > 0)
                    {
                        rslt.Add(packSku);
                    }
                }

                if (info.Qty > 0)
                {
                    var packSku = new OutboundBoxDto();
                    packSku.BoxName = info.SkuName;
                    packSku.BoxType = "未装箱";
                    packSku.BoxSkuCount = 1;
                    packSku.BoxSkuQty = info.Qty;
                    noBoxSkuList.Add(packSku);
                }
            }
            return noBoxSkuList;
        }

        /// <summary>
        /// 根据条件获取出库单信息
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public OutboundViewDto GetOutboundOrderByOrderId(OutboundQuery outboundQuery)
        {
            this._crudRepository.ChangeDB(outboundQuery.WarehouseSysId);
            var query =
                _crudRepository.GetQuery<outbound>(
                    x =>
                        x.OutboundOrder == outboundQuery.OutboundOrder &&
                        x.WareHouseSysId == outboundQuery.WarehouseSysId, outboundQuery.WarehouseSysId);

            if (outboundQuery.WaitPickSearch)
            {
                query =
                    query.Where(
                        x => x.Status == (int)OutboundStatus.New || x.Status == (int)OutboundStatus.PartAllocation);
            }

            return query.FirstOrDefault().JTransformTo<OutboundViewDto>();
        }
        #endregion

        #region 拣货

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public Pages<PickDetailListDto> GetPickDetailListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            this._crudRepository.ChangeDB(pickDetailQuery.WarehouseSysId);
            var response = _crudRepository.GetPickDetailListDtoByPageInfo(pickDetailQuery);
            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {
                List<Guid?> pickDetailSysIds = response.TableResuls.aaData.Select(p => p.SysId).ToList();
                var pickDetailList = _crudRepository.GetSummaryPickDetailListDto(pickDetailSysIds, pickDetailQuery.WarehouseSysId);
                foreach (var item in response.TableResuls.aaData)
                {
                    var detail = pickDetailList.Find(x => x.PickDetailOrder == item.PickDetailOrder);
                    item.OutboundCount = detail.OutboundCount;
                }
            }

            return response;
        }

        /// <summary>
        /// 获取待拣货出库数据
        /// </summary>
        /// <param name="pickDetailQuery"></param>
        /// <returns></returns>
        public Pages<PickOutboundListDto> GetPickOutboundListDtoByPageInfo(PickDetailQuery pickDetailQuery)
        {
            this._crudRepository.ChangeDB(pickDetailQuery.WarehouseSysId);
            var response = _crudRepository.GetPickOutboundListDtoByPageInfo(pickDetailQuery);
            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {
                List<Guid?> outboundSysIds = response.TableResuls.aaData.Select(p => p.SysId).ToList();
                var outboundDetailList = _crudRepository.GetPickOutboundDetailListDto(outboundSysIds, pickDetailQuery.WarehouseSysId);
                foreach (var item in response.TableResuls.aaData)
                {
                    var detail = outboundDetailList.Find(x => x.OutboundSysId == item.SysId);
                    item.SkuTypeQty = detail.SkuTypeQty;
                }
            }
            return response;
        }

        /// <summary>
        /// 根据出库单ID获取异常明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public List<OutboundExceptionDtoList> GetOutbooundExceptionData(Guid sysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetOutbooundExceptionData(sysId);

        }
        #endregion

        #region 入库
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseListDto> GetPurchaseDtoListByPageInfo(PurchaseQuery purchaseQuery)
        {
            this._crudRepository.ChangeDB(purchaseQuery.WarehouseSysId);
            return _crudRepository.GetPurchaseDtoListByPageInfo(purchaseQuery);
        }


        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="purchaseQuery"></param>
        /// <returns></returns>
        public Pages<PurchaseReturnListDto> GetPurchaseReturnDtoListByPageInfo(PurchaseReturnQuery purchaseQuery)
        {
            this._crudRepository.ChangeDB(purchaseQuery.WarehouseSysId);
            return _crudRepository.GetPurchaseReturnDtoListByPageInfo(purchaseQuery);
        }

        /// <summary>
        /// 根据SysCodeId 获取相关数据
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        public PurchaseViewDto GetPurchaseViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            var purchaseViewDto = _crudRepository.GetPurchaseViewDtoBySysId(purchaseSysId, warehouseSysId);
            purchaseViewDto.PurchaseDetailViewDto = _crudRepository.GetPurchaseDetailViewBySysId(purchaseSysId, warehouseSysId);
            var receipt = _crudRepository.GetQuery<receipt>(x => x.ExternalOrder == purchaseViewDto.PurchaseOrder, warehouseSysId).ToList();
            purchaseViewDto.ReceiptPurchaseDto = receipt.JTransformTo<ReceiptPurchaseDto>();

            return purchaseViewDto;
        }

        /// <summary>
        /// 获取退货入库单
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public PurchaseReturnViewDto GetPurchaseReturnViewDtoBySysId(Guid purchaseSysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            var purchaseReturnViewDto = _crudRepository.GetPurchaseReturnViewDtoBySysId(purchaseSysId, warehouseSysId);
            purchaseReturnViewDto.PurchaseDetailReturnViewDto = _crudRepository.GetPurchaseDetailReturnViewBySysId(purchaseSysId, warehouseSysId);
            var receipt = _crudRepository.GetQuery<receipt>(x => x.ExternalOrder == purchaseReturnViewDto.PurchaseOrder, warehouseSysId).ToList();
            purchaseReturnViewDto.ReceiptPurchaseDto = receipt.JTransformTo<ReceiptPurchaseDto>();

            if (purchaseReturnViewDto.FromWareHouseSysId.HasValue)
            {
                var fromWarehouse = _crudRepository.GetQuery<warehouse>(p => p.SysId.Equals(purchaseReturnViewDto.FromWareHouseSysId.Value)).First();
                purchaseReturnViewDto.FromWareHouseName = fromWarehouse.Name;
            }
            return purchaseReturnViewDto;
        }
        #endregion

        #region 收货

        /// <summary>
        /// 获取收货单列表
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        public Pages<ReceiptListDto> GetReceiptList(ReceiptQuery receiptQuery)
        {
            this._crudRepository.ChangeDB(receiptQuery.WarehouseSysId);
            return _crudRepository.GetReceiptListByPaging(receiptQuery);
        }

        /// <summary>
        /// 获取收货单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ReceiptViewDto GetReceiptViewById(Guid sysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            ReceiptViewDto receiptViewDto = _crudRepository.GetReceiptViewById(sysId, warehouseSysId);
            if (receiptViewDto != null)
            {
                receiptViewDto.ReceiptDetailViewDtoList = _crudRepository.GetReceiptDetailViewList(sysId, warehouseSysId);
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
                    ReceiptDetailLotViewDtoList = new List<ReceiptDetailViewDto>(),
                    RelatedReceiptPurchaseDtoList = new List<ReceiptPurchaseDto>()
                };
            }
            return receiptViewDto;
        }

        /// <summary>
        /// 收货清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailViewList(Guid sysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetReceiptDetailViewList(sysId, warehouseSysId);
        }
        #endregion

        /// <summary>
        /// 收货批次清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailLotViewList(Guid sysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetReceiptDetailLotViewList(sysId, warehouseSysId);
        }

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public List<ReceiptDetailViewDto> GetReceiptDetailViewListByCollectionLot(Guid receiptSysId, Guid warehouseSysId)
        {
            this._crudRepository.ChangeDB(warehouseSysId);
            return _crudRepository.GetReceiptDetailViewListByCollectionLot(receiptSysId);
        }

        #region 库存转移
        /// <summary>
        /// 库存转移分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<StockTransferLotListDto> GetStockTransferLotByPage(StockTransferQuery request)
        {
            this._crudRepository.ChangeDB(request.WarehouseSysId);
            return _crudRepository.GetStockTransferLotByPage(request);
        }
        #endregion
    }
}

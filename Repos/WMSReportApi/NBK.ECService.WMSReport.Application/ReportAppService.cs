using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Model.Models;
using NBK.ECService.WMSReport.Repository.Interface;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.DTO.Report;
using System.Linq;
using NBK.ECService.WMSReport.DTO.Other;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NBK.ECService.WMSReport.DTO.Query;

namespace NBK.ECService.WMSReport.Application
{
    public class ReportAppService : ApplicationService, IReportAppService
    {
        private IReportRepository _reportRepository = null;
        private IPackageAppService _packageAppService = null;

        public ReportAppService(IReportRepository reportRepository, IPackageAppService packageAppService)
        {
            this._reportRepository = reportRepository;
            _packageAppService = packageAppService;
        }

        /// <summary>
        /// 货卡查询
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        public InvTransBySkuReportDto GetInvTransBySkuReport(InvTransBySkuReportQuery invTransBySkuReportQuery)
        {
            //if (invTransBySkuReportQuery.SkuNameSearch.IsNull()
            //    && invTransBySkuReportQuery.SkuUPCSearch.IsNull()
            //    && invTransBySkuReportQuery.SkuCodeSearch.IsNull())
            //{
            //    return new InvTransBySkuReportDto
            //    {
            //        InvTranDtoList =
            //            new Pages<InvTranDto>
            //            {
            //                TableResuls = new TableResults<InvTranDto> { aaData = new List<InvTranDto>() }
            //            }
            //    };
            //}

            //var lambda = Wheres.Lambda<sku>();
            //if (invTransBySkuReportQuery != null)
            //{
            //    if (!invTransBySkuReportQuery.SkuNameSearch.IsNull())
            //    {
            //        lambda = lambda.And(p => p.SkuName == invTransBySkuReportQuery.SkuNameSearch);
            //    }
            //    if (!invTransBySkuReportQuery.SkuUPCSearch.IsNull())
            //    {
            //        lambda = lambda.And(p => p.UPC == invTransBySkuReportQuery.SkuUPCSearch);
            //    }
            //    if (!invTransBySkuReportQuery.SkuCodeSearch.IsNull())
            //    {
            //        lambda = lambda.And(p => p.SkuCode == invTransBySkuReportQuery.SkuCodeSearch);
            //    }
            //}
            //var sku = _reportRepository.FirstOrDefault(lambda);
            //if (sku != null)
            //{
            //    var response = new InvTransBySkuReportDto
            //    {
            //        SkuName = sku.SkuName,
            //        SkuUPC = sku.UPC,
            //        SkuDescr = sku.SkuDescr,
            //        InvTranDtoList = _reportRepository.GetInvTranListByPaging(sku.SysId, invTransBySkuReportQuery)
            //    };

            //    //gavin: 原材料单位反转
            //    if (response.InvTranDtoList.TableResuls != null && response.InvTranDtoList.TableResuls.aaData.Count > 0)
            //    {
            //        var pack = _reportRepository.Get<pack>(sku.PackSysId);
            //        bool isconvert = false;

            //        if (pack.InLabelUnit01.HasValue && pack.InLabelUnit01.Value == true)
            //        {
            //            if (pack.FieldValue01 > 0 && pack.FieldValue02 > 0)
            //            {
            //                isconvert = true;
            //                foreach (var item in response.InvTranDtoList.TableResuls.aaData)
            //                {
            //                    item.DisplayQty = Math.Round(((pack.FieldValue02.Value * item.Qty * 1.00m) / pack.FieldValue01.Value), 3);
            //                    item.DisplayAvailableQty = Math.Round(((pack.FieldValue02.Value * item.AvailableQty * 1.00m) / pack.FieldValue01.Value), 3);
            //                }
            //            }
            //        }

            //        if (isconvert == false)
            //        {
            //            foreach (var item in response.InvTranDtoList.TableResuls.aaData)
            //            {
            //                item.DisplayQty = item.Qty;
            //                item.DisplayAvailableQty = item.AvailableQty;
            //            }
            //        }


            //    }

            //    return response;
            //}
            return new InvTransBySkuReportDto();
            //{
            //    InvTranDtoList =
            //        new Pages<InvTranDto> { TableResuls = new TableResults<InvTranDto> { aaData = new List<InvTranDto>() } }
            //};
        }


        /// <summary>
        /// 货卡查询返回list
        /// </summary>
        /// <param name="invTransBySkuReportQuery"></param>
        /// <returns></returns>
        public InvTransSkuListReportDto GetInvTransBySkuListReport(InvTransBySkuReportQuery invTransBySkuReportQuery)
        {
            if (invTransBySkuReportQuery.SkuNameSearch.IsNull()
                && invTransBySkuReportQuery.SkuUPCSearch.IsNull()
                && invTransBySkuReportQuery.SkuCodeSearch.IsNull())
            {
                return new InvTransSkuListReportDto
                {
                    InvTransBySkuReportDto = new List<InvTransBySkuReportDto>(),
                    InvTranDtoList =
                        new Pages<InvTranDto>
                        {
                            TableResuls = new TableResults<InvTranDto> { aaData = new List<InvTranDto>() }
                        }
                };
            }
            var result = new InvTransSkuListReportDto()
            {
                InvTransBySkuReportDto = new List<InvTransBySkuReportDto>(),
                InvTranDtoList = new Pages<InvTranDto>()
            };
            _reportRepository.ChangeDB(invTransBySkuReportQuery.WarehouseSysId);
            result.InvTransBySkuReportDto = _reportRepository.GetSkuInfoByQuery(invTransBySkuReportQuery);
            if (result.InvTransBySkuReportDto.Count == 1)
            {
                result.InvTranDtoList = _reportRepository.GetInvTranListByPaging(result.InvTransBySkuReportDto[0].SysId, invTransBySkuReportQuery);
            }
            else
            {
                result.InvTranDtoList =
                        new Pages<InvTranDto>
                        {
                            TableResuls = new TableResults<InvTranDto> { aaData = new List<InvTranDto>() }
                        };
            }
            return result;
        }


        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLocReportDto> GetInvLocBySkuReport(InvSkuLocReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetInvLocBySkuReport(request);

            return response;
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotReportDto> GetInvLotBySkuReport(InvSkuLotReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetInvLotBySkuReport(request);

            return response;
        }

        /// <summary>
        /// 货位批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotLocLpnReportDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetInvLotLocLpnBySkuReport(request);
            return response;
        }

        public Pages<InvSkuLotReportDto> GetExpiryInvLotBySkuReport(InvSkuLotReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetExpiryInvLotBySkuReport(request);

            return response;
        }

        public Pages<ReceiptDetailReportDto> GetReceiptDetailReport(ReceiptDetailReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetReceiptDetailReport(request);
        }

        public Pages<OutboundDetailReportDto> GetOutboundDetailReport(OutboundDetailReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundDetailReport(request);
        }

        public UploadResultInformation GetOutboundDetailReportByFile(OutboundDetailReportQuery request)
        {
            this._reportRepository.ChangeDB(request.WarehouseSysId);
            var searchResult = this.GetOutboundDetailReport(request);
            XSSFWorkbook book = NPOIExtend.OutboundDetailReportExport(searchResult.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            //FileStream fsm = new FileStream();
            //File.(ms, "application/vnd.ms-excel",
            //    string.Format("出库明细-{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
            var warehouse = _reportRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();
            string warehouseName = warehouse == null ? string.Empty : warehouse.Name;
            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"出库明细-{warehouseName}-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuReportDto> GetInvSkuReport(InvSkuReportQuery request)
        {
            _reportRepository.ChangeDB((Guid)request.SearchWarehouseSysId);
            var response = _reportRepository.GetInvSkuReport(request);
            return response;
        }

        /// <summary>
        /// 仓库进销存报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FinanceInvoicingReportDto> GetFinanceInvoicingReport(FinanceInvoicingReportQueryDto request)
        {
            _reportRepository.ChangeDB((Guid)request.SearchWarehouseSysId);
            var response = _reportRepository.GetFinanceInvoicingReport(request);
            return response;
        }

        /// <summary>
        /// 损益明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<AdjustmentDetailReportDto> GetAdjustmentDetailReport(AdjustmentDetailReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetAdjustmentDetailReport(request);

            return response;
        }

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<PurchaseDetailReportDto> GetPurchaseDetailReport(PurchaseDetailReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetPurchaseDetailReport(request);
            return response;
        }

        public Pages<InboundReportDto> GetInboundReport(InboundReportQuery inboundReportQuery)
        {
            _reportRepository.ChangeDB(inboundReportQuery.WarehouseSysId);
            return _reportRepository.GetInboundReport(inboundReportQuery);
        }


        public Pages<SkuBorrowReportDto> GetSkuBorrowListByPage(SkuBorrowReportQuery query)
        {
            _reportRepository.ChangeDB(query.WarehouseSysId);
            return _reportRepository.GetSkuBorrowListByPage(query);
        }

        public Pages<FrozenSkuReportDto> GetFrozenSkuReport(FrozenSkuReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetFrozenSkuReport(request);
        }

        public UploadResultInformation GetFrozenSkuReportByFile(FrozenSkuReportQuery request)
        {
            this._reportRepository.ChangeDB(request.WarehouseSysId);
            var searchResult = this.GetFrozenSkuReport(request);
            XSSFWorkbook book = NPOIExtend.FrozenSkuReportByFile(searchResult.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            //FileStream fsm = new FileStream();
            //File.(ms, "application/vnd.ms-excel",
            //    string.Format("出库明细-{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
            var warehouse = _reportRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();
            string warehouseName = warehouse == null ? string.Empty : warehouse.Name;
            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"冻结商品明细-{warehouseName}-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }

        /// <summary>
        /// 收发货明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceivedAndSendSkuReportDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuReportQuery request)
        {
            _reportRepository.ChangeDB((Guid)request.SearchWarehouseSysId);
            return _reportRepository.GetReceivedAndSendSkuReport(request);
        }

        public Pages<OutboundHandleDateStatisticsDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundHandleDateStatisticsReport(request);
        }

        public Pages<ReceiptAndDeliveryDateDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetReceiptAndDeliveryDateReport(request);
        }

        public Pages<PurchaseMonitorDto> GetMonitorReportPurchaseDto(MonitorReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetMonitorReportPurchaseDto(request);
        }

        public Pages<OutboundMonitorDto> GetMonitorReportOutBoundDto(MonitorReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetMonitorReportOutBoundDto(request);
        }

        public Pages<WorkMonitorDto> GetMonitorReportWorkDto(MonitorReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetMonitorReportWorkDto(request);
        }

        public Pages<PurchaseReturnMonitorDto> GetMonitorReportPurchaseReturnDto(MonitorReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetMonitorReportPurchaseReturnDto(request);
        }

        public Pages<StockTransferMonitorDto> GetMonitorReportStockTransferDto(MonitorReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetMonitorReportStockTransferDto(request);
        }

        public Pages<SNManageReportDto> GetSNManageReport(SNManageReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetSNManageReport(request);
        }

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundExceptionReportDto> GetOutboundExceptionReport(OutboundExceptionReportQuery request)
        {

            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundExceptionReport(request);
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundBoxReportDto> GetOutboundBoxReport(OutboundBoxReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            var rsp = _reportRepository.GetOutboundBoxReport(request);
            //if (rsp.TableResuls.aaData != null && rsp.TableResuls.aaData.Any())
            //{
            //    var outboundSysIds = rsp.TableResuls.aaData.Select(p => p.OutboundSysId).ToList();
            //    var outboundBoxes = _reportRepository.GetOutboundBox(outboundSysIds, request.WarehouseSysId);
            //    foreach (var item in rsp.TableResuls.aaData)
            //    {
            //        var boxInfos = GetOutboundBox(outboundBoxes.Where(p => p.OutboundSysId == item.OutboundSysId).ToList());
            //        item.WholeCaseQty = boxInfos.Where(x => x.BoxType == "整件").Sum(x => x.BoxSkuCount);
            //        item.ScatteredCaseQty = boxInfos.Where(x => x.BoxType == "散件装箱").Count();
            //    }
            //}
            return rsp;
        }

        private List<OutboundBoxDto> GetOutboundBox(List<OutboundBoxDto> outboundBoxList)
        {
            var rslt = new List<OutboundBoxDto>();
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
            return rslt;
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

        public Pages<BalanceReportDto> GetBalanceReport(BalanceReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetBalanceReport(request);
        }

        public Pages<OutboundPackReportDto> GetOutboundPackReport(OutboundPackReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundPackReport(request);
        }

        public Pages<OutboundPackSkuReportDto> GetOutboundPackSkuReport(OutboundPackSkuReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundPackSkuReport(request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<SkuPackReportListDto> GetSkuPackReport(SkuPackReportQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetSkuPackReport(request);
        }

        /// <summary>
        /// 出库汇总报表查看
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundListDto> GetOutboundSummaryReport(OutboundQuery request)
        {
            this._reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetOutboundSummaryReport(request);
        }


        /// <summary>
        /// 出库汇总报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UploadResultInformation GetOutboundSummaryExport(OutboundQuery request)
        {
            this._reportRepository.ChangeDB(request.WarehouseSysId);
            var response = _reportRepository.GetOutboundSummaryReport(request);

            XSSFWorkbook book = NPOIExtend.OutboundSummaryExport(response.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            var warehouse = _reportRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();
            string warehouseName = warehouse == null ? string.Empty : warehouse.Name;
            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"出库汇总-{warehouseName}-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }

        /// <summary>
        /// 渠道库存报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ChannelInventoryDto> GetChannelInventoryByPage(ChannelInventoryQuery request)
        {
            _reportRepository.ChangeDB(request.WarehouseSysId);
            return _reportRepository.GetChannelInventoryByPage(request);
        }
    }
}
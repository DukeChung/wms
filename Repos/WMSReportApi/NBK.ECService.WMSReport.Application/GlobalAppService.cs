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
using Abp.Domain.Uow;
using NBK.ECService.WMSReport.Utility.Redis;

namespace NBK.ECService.WMSReport.Application
{
    public class GlobalAppService : ApplicationService, IGlobalAppService
    {
        private IGlobalRepository _iglobalRepository = null;

        public GlobalAppService(IGlobalRepository globalRepository)
        {
            _iglobalRepository = globalRepository;
        }

        public List<WareHouseDto> GetAllWarehouse()
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetAllWarehouse();
        }

        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<PurchaseDetailGlobalDto> GetPurchaseDetailReport(PurchaseDetailGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetPurchaseDetailReport(request);
        }

        /// <summary>
        /// 货位库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLocGlobalDto> GetInvLocBySkuReport(InvSkuLocGlobalQuery request)
        {
            if (request.SearchWarehouseSysId != new Guid())
            {
                _iglobalRepository.ChangeDB(request.SearchWarehouseSysId);
            }
            else
            {
                _iglobalRepository.ChangeGlobalDB();
            }

            return _iglobalRepository.GetInvLocBySkuReport(request);
        }

        /// <summary>
        /// 批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotGlobalDto> GetInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            if (request.SearchWarehouseSysId != new Guid())
            {
                _iglobalRepository.ChangeDB(request.SearchWarehouseSysId);
            }
            else
            {
                _iglobalRepository.ChangeGlobalDB();
            }
            return _iglobalRepository.GetInvLotBySkuReport(request);
        }

        /// <summary>
        /// 货位批次库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotLocLpnGlobalDto> GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request)
        {
            if (request.SearchWarehouseSysId != new Guid())
            {
                _iglobalRepository.ChangeDB(request.SearchWarehouseSysId);
            }
            else
            {
                _iglobalRepository.ChangeGlobalDB();
            }
            return _iglobalRepository.GetInvLotLocLpnBySkuReport(request);
        }

        /// <summary>
        /// 获取临过期批次商品库存信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuLotGlobalDto> GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            if (request.SearchWarehouseSysId != new Guid())
            {
                _iglobalRepository.ChangeDB(request.SearchWarehouseSysId);
            }
            else
            {
                _iglobalRepository.ChangeGlobalDB();
            }
            return _iglobalRepository.GetExpiryInvLotBySkuReport(request);
        }

        /// <summary>
        /// 收货明细查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceiptDetailGlobalDto> GetReceiptDetailReport(ReceiptDetailGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetReceiptDetailReport(request);
        }

        /// <summary>
        /// 出库明细查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundDetailGlobalDto> GetOutboundDetailReport(OutboundDetailGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundDetailReport(request);
        }

        public UploadResultInformation GetOutboundDetailReportByFile(OutboundDetailGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var searchResult = _iglobalRepository.GetOutboundDetailReport(request);

            if (searchResult.TableResuls.iTotalDisplayRecords > PublicConst.EachExportDataMaxCount)
            {
                return null;
            }

            XSSFWorkbook book = NPOIExtend.OutboundDetailGlobalExport(searchResult.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            string warehouseName = "全部仓库";

            if (request.SearchWarehouseSysId != Guid.Empty)
            {
                var warehouse = _iglobalRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();
                warehouseName = warehouse == null ? string.Empty : warehouse.Name;
            }
            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"出库明细-{warehouseName}-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<InvSkuGlobalDto> GetInvSkuReport(InvSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetInvSkuReport(request);
        }

        /// <summary>
        /// 收发货明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceivedAndSendSkuGlobalDto> GetReceivedAndSendSkuReport(ReceivedAndSendSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetReceivedAndSendSkuReport(request);
        }

        /// <summary>
        /// 仓库进销存报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FinanceInvoicingGlobalDto> GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetFinanceInvoicingReport(request);
        }

        /// <summary>
        /// 损益明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<AdjustmentDetailGlobalDto> GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetAdjustmentDetailReport(request);
        }

        /// <summary>
        /// 获取系统代码明细
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public List<SelectItem> GetSysCodeDetailList(string sysCodeType, bool isActive)
        {
            _iglobalRepository.ChangeGlobalDB();
            var sysCode = _iglobalRepository.GetQuery<syscode>(x => x.SysCodeType == sysCodeType).ToList();
            if (sysCode.Any())
            {
                var detailList = sysCode.FirstOrDefault().syscodedetails.Where(p => p.IsActive == isActive).OrderBy(x => x.SeqNo).ToList();
                var list = new List<SelectItem>();
                detailList.ForEach(item =>
                {
                    list.Add(new SelectItem()
                    {
                        Text = item.Descr,
                        Value = item.Code
                    });
                });
                return list;
            }
            else
            {
                throw new Exception("系统代码不存在");
            }
        }

        /// <summary>
        /// 入库汇总查询
        /// </summary>
        /// <param name="inboundReportQuery"></param>
        /// <returns></returns>
        public Pages<InboundGlobalDto> GetInboundReport(InboundGlobalQuery inboundReportQuery)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetInboundReport(inboundReportQuery);
        }

        /// <summary>
        /// 移仓单报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<TransferinventoryGlobalDto> GetTransferinventoryReport(TransferinventoryGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetTransferinventoryReport(request);
        }

        /// <summary>
        /// 冻结商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FrozenSkuGlobalDto> GetFrozenSkuReport(FrozenSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetFrozenSkuReport(request);
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundHandleDateStatisticsGlobalDto> GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundHandleDateStatisticsReport(request);
        }

        /// <summary>
        /// 出库处理时间统计表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ReceiptAndDeliveryDateGlobalDto> GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetReceiptAndDeliveryDateReport(request);
        }

        /// <summary>
        /// SN管理报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<SNManageGlobalDto> GetSNManageReport(SNManageGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetSNManageReport(request);
        }

        /// <summary>
        /// 异常报告报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundExceptionGlobalDto> GetOutboundExceptionReport(OutboundExceptionGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundExceptionReport(request);
        }

        /// <summary>
        /// 出库箱数统计报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundBoxGlobalDto> GetOutboundBoxReport(OutboundBoxGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var rsp = _iglobalRepository.GetOutboundBoxReport(request);
            //if (rsp.TableResuls.aaData != null && rsp.TableResuls.aaData.Any())
            //{
            //    var outboundSysIds = rsp.TableResuls.aaData.Select(p => p.OutboundSysId).ToList();
            //    var outboundBoxes = _iglobalRepository.GetOutboundBox(outboundSysIds, request.WarehouseSysId);
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
            _iglobalRepository.ChangeGlobalDB();
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
            _iglobalRepository.ChangeGlobalDB();
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
        /// B2C结算报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<BalanceGlobalDto> GetBalanceReport(BalanceGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetBalanceReport(request);
        }

        /// <summary>
        /// 整散箱装箱明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundPackGlobalDto> GetOutboundPackReport(OutboundPackGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundPackReport(request);
        }

        /// <summary>
        /// 整散箱商品明细报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundPackSkuGlobalDto> GetOutboundPackSkuReport(OutboundPackSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundPackSkuReport(request);
        }

        /// <summary>
        /// 商品包装查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<SkuPackGlobalListDto> GetSkuPackReport(SkuPackGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetSkuPackReport(request);
        }

        /// <summary>
        /// 出库单商品汇总报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundSkuGlobalDto> GetOutboundSkuReport(OutboundSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundSkuReport(request);
        }

        /// <summary>
        /// 出库捡货工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<PickingTimeSpanGlobalDto> GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetPickingTimeSpanReport(request);
        }

        /// <summary>
        /// 出库复核工时统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundTransferOrderGlobalDto> GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundTransferOrderReport(request);
        }

        [UnitOfWork(isTransactional: false)]
        public List<AccessBizMappingDto> GetAccessBizMappingList(bool flag)
        {
            _iglobalRepository.ChangeLogDB();
            var result = new List<AccessBizMappingDto>();

            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_iglobalRepository.GetAccessBizMappingList(), RedisReportSourceKey.AccessBizMappingList);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _iglobalRepository.GetAccessBizMappingList();
                    }, RedisReportSourceKey.AccessBizMappingList);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 化肥出入库蜘蛛网图
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<FertilizerRORadarGlobalDto> GetFertilizerRORadarList(bool flag, FertilizerRORadarGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var result = new List<FertilizerRORadarGlobalDto>();
            try
            {
                if (flag)
                {
                    result = _iglobalRepository.GetFertilizerRORadarList(request);
                    int rMaxQty1 = result.Max(p => p.RQty);
                    int oMaxQty1 = result.Max(p => p.OQty);
                    int max = rMaxQty1 >= oMaxQty1 ? rMaxQty1 : oMaxQty1;
                    result.ForEach(p => p.Max = max);
                    RedisWMS.SetRedis(result, RedisReportSourceKey.FertilizerRORadarList);

                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        var list1 = _iglobalRepository.GetFertilizerRORadarList(request);
                        int rMaxQty = list1.Max(p => p.RQty);
                        int oMaxQty = list1.Max(p => p.OQty);
                        int max2 = rMaxQty >= oMaxQty ? rMaxQty : oMaxQty;
                        list1.ForEach(p => p.Max = max2);
                        return list1;
                    }, RedisReportSourceKey.FertilizerRORadarList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 化肥库存蜘蛛网图
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<FertilizerInvRadarGlobalDto> GetFertilizerInvRadarList(bool flag, FertilizerInvRadarGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var result = new List<FertilizerInvRadarGlobalDto>();
            try
            {
                if (flag)
                {
                    result = _iglobalRepository.GetFertilizerInvRadarList(request);
                    int max1 = result.Max(p => p.Qty);
                    result.ForEach(p => p.Max = max1);
                    RedisWMS.SetRedis(result, RedisReportSourceKey.FertilizerInvRadarList);
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        var list = _iglobalRepository.GetFertilizerInvRadarList(request);
                        int max2 = list.Max(p => p.Qty);
                        list.ForEach(p => p.Max = max2);
                        return list;
                    }, RedisReportSourceKey.FertilizerInvRadarList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 8个环形库存分布图
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false)]
        public List<FertilizerInvPieGlobalDto> GetFertilizerInvPieList(bool flag, FertilizerInvPieGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var result = new List<FertilizerInvPieGlobalDto>();
            try
            {
                if (flag)
                {
                    RedisWMS.SetRedis(_iglobalRepository.GetFertilizerInvPieList(request), string.Format(RedisReportSourceKey.FertilizerInvPieList, request.SkuSysId));
                }
                else
                {
                    result = RedisWMS.CacheResult(() =>
                    {
                        return _iglobalRepository.GetFertilizerInvPieList(request);
                    }, string.Format(RedisReportSourceKey.FertilizerInvPieList, request.SkuSysId));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        [UnitOfWork(isTransactional: false)]
        public Pages<ReturnOrderGlobalDto> GetReturnOrderGlobalReport(ReturnOrderGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetReturnOrderGlobalReport(request);
        }

        public UploadResultInformation GetReturnOrderReportExport(ReturnOrderGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var searchResult = _iglobalRepository.GetReturnOrderGlobalReport(request); ;
            XSSFWorkbook book = NPOIExtend.ReturnOrderGlobalReportExport(searchResult.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            //FileStream fsm = new FileStream();
            //File.(ms, "application/vnd.ms-excel",
            //    string.Format("出库明细-{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd")));

            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"退货单报表-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }

        /// <summary>
        /// 出库汇总报表查看
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<OutboundListGlobalDto> GetOutboundSummaryReport(OutboundGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetOutboundSummaryReport(request);
        }


        /// <summary>
        /// 出库汇总报表导出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UploadResultInformation GetOutboundSummaryExport(OutboundGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            var response = _iglobalRepository.GetOutboundSummaryReport(request);

            if (response != null && response.TableResuls != null && response.TableResuls.aaData.Count > 0)
            {

                if (response.TableResuls.iTotalDisplayRecords > 100000)
                {
                    return null;
                }
            }

            XSSFWorkbook book = NPOIExtend.OutboundSummaryGlobalExport(response.TableResuls.aaData);

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            var warehouse = _iglobalRepository.GetQuery<warehouse>(p => p.SysId == request.WarehouseSysId).FirstOrDefault();
            string warehouseName = warehouse == null ? string.Empty : warehouse.Name;
            UploadResultInformation result = FileUploader.UploadFile(PublicConst.ReportFile, ms, $"出库汇总-{warehouseName}-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", FtpFileType.All);

            return result;
        }


        /// <summary>
        /// 农资出库商品报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<FertilizerOutboundSkuGlobalDto> GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request)
        {
            _iglobalRepository.ChangeGlobalDB();
            return _iglobalRepository.GetFertilizerOutboundSkuReport(request);
        }

        /// <summary>
        /// 渠道库存查询报表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Pages<ChannelInventoryGlobalDto> GetChannelInventoryByPage(ChannelInventoryGlobalQuery request)
        {
            if (request.SearchWarehouseSysId != new Guid())
            {
                _iglobalRepository.ChangeDB(request.SearchWarehouseSysId);
            }
            else
            {
                _iglobalRepository.ChangeGlobalDB();
            }
            return _iglobalRepository.GetChannelInventoryByPage(request);
        }
    }
}

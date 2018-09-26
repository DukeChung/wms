using NBK.AuthServiceUtil;
using NBK.ECService.WMSReport.DTO;
using WMS.Global.Portal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using System.Web.Routing;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using NBK.ECService.WMSReport.DTO.Report;
using NBK.ECService.WMSReport.DTO.Query;
using NBK.ECService.WMSReport.DTO.Base;

namespace WMS.Global.Portal.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        #region  货卡查询
        //[PermissionAuthorize("Report_InvTransBySkuReport")]
        public ActionResult InvTransBySkuReport()
        {
            return View();
        }

        public ActionResult GetInvTransBySkuReport(InvTransBySkuReportQuery invTransBySkuReportQuery)
        {
            invTransBySkuReportQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetInvTransBySkuReport(LoginCoreQuery, invTransBySkuReportQuery);
            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.InvTranDtoList.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.InvTransBySkuReportDto,
                    rsp.ResponseResult.InvTranDtoList.TableResuls.aaData,
                    rsp.ResponseResult.InvTranDtoList.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.InvTranDtoList.TableResuls.iTotalRecords,
                    rsp.ResponseResult.InvTranDtoList.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion 

        #region  货位库存查询
        public ActionResult InvLocBySkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInvLocBySkuReport(InvSkuLocGlobalQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetInvLocBySkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult InvLocBySkuReportExport(InvSkuLocGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetInvLocBySkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.InvLocBySkuReportExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("货位库存导出-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region  批次库存查询
        public ActionResult InvLotBySkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetInvLotBySkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region  货位批次库存查询
        public ActionResult InvLotLocLpnBySkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnGlobalQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetInvLotLocLpnBySkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region  临期批次库存查询
        public ActionResult ExpiryInvLotBySkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetExpiryInvLotBySkuReport(InvSkuLotGlobalQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetExpiryInvLotBySkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region  收货明细查询
        public ActionResult ReceiptDetailReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetReceiptDetailReport(ReceiptDetailGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetReceiptDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult ReceiptDetailReportExport(ReceiptDetailGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetReceiptDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.ReceiptDetailReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("收货明细-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 出库明细
        public ActionResult OutboundDetailReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundDetailReport(OutboundDetailGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetOutboundDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult OutboundDetailReportExport(OutboundDetailGlobalQuery request)
        {
            request.IsExport = true;
            HSSFWorkbook book = new HSSFWorkbook();
            for (int i = 0; i < 5; i++)
            {
                request.iDisplayStart = i * PublicConst.EachQuestDataRowsCount;
                request.iDisplayLength = PublicConst.EachQuestDataRowsCount;
                var rsp = ReportApiClient.GetInstance().GetOutboundDetailReport(request, this.LoginCoreQuery);
                if (rsp.Success == true && rsp.ResponseResult.TableResuls.aaData != null)
                {
                    if (rsp.ResponseResult.TableResuls.iTotalDisplayRecords > PublicConst.EachExportDataMaxCount)
                    {
                        throw new Exception("导出数据条数大于10W,请联系管理员!");
                    }
                    book = NPOIExtend.OutboundDetailReportExport(rsp.ResponseResult.TableResuls.aaData, book, i + 1);
                }
                else
                {
                    if (rsp.Success == false)
                    {
                        throw new Exception(rsp.ApiMessage.ErrorMessage);
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel",
                string.Format("出库明细-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
        }

        public ActionResult OutboundDetailReportExportByFile(OutboundDetailGlobalQuery request)
        {
            request.IsExport = true;
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetOutboundDetailReportByFile(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                if (rsp.ResponseResult == null)
                {
                    return Json(new { success = false, message = "导出数据条数大于10W,请联系管理员!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, filePath = rsp.ResponseResult.HttpFullPathName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  库存汇总报告
        public ActionResult InvSkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInvSkuReport(InvSkuGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetInvSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 导出库存汇总报告
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult InvSkuReportExport(InvSkuGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetInvSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.InvSkuReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("库存汇总报告-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region  仓库进销存报表
        public ActionResult FinanceInvoicingReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }

        public ActionResult GetFinanceInvoicingReport(FinanceInvoicingGlobalQueryDto request)
        {
            var rsp = ReportApiClient.GetInstance().GetFinanceInvoicingReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult FinanceInvoicingReportExport(FinanceInvoicingGlobalQueryDto request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var startTime = request.StartTime.ToString(PublicConst.StartDateFormat);
            var endTime = request.EndTime.ToString(PublicConst.EndDateFormat);
            var rsp = ReportApiClient.GetInstance().GetFinanceInvoicingReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.FinanceInvoicingReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("仓库进销存报表-{0}.xls", startTime + "~" + endTime));
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region  损益明细报表
        public ActionResult AdjustmentDetailReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetAdjustmentDetailReport(AdjustmentDetailGlobalQuery request)
        {
            if (request.CreateDateTo.HasValue)
                request.CreateDateTo = request.CreateDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetAdjustmentDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetAdjustmentLevelSysCode()
        {
            List<SelectListItem> adjustmentLevelList = new List<SelectListItem>();

            var rsp = BaseDataApiClient.GetInstance().SelectItemSysCode("AdjustmentLevel");
            if (rsp.Success && rsp.ResponseResult != null)
            {
                adjustmentLevelList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }

            return Json(new { adjustmentLevelList }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 入库明细报表
        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <returns></returns>
        public ActionResult PurchaseDetailByReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetPurchaseDetailReport(PurchaseDetailGlobalQuery request)
        {
            if (request.LastReceiptDateTo.HasValue)
                request.LastReceiptDateTo = request.LastReceiptDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetPurchaseDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult PurchaseDetailReportExport(PurchaseDetailGlobalQuery request)
        {
            if (request.LastReceiptDateTo.HasValue)
                request.LastReceiptDateTo = request.LastReceiptDateTo.Value.AddDays(1);
            request.iDisplayStart = 0;
            request.iDisplayLength = PublicConst.EachExportDataMaxCount;
            var rsp = ReportApiClient.GetInstance().GetPurchaseDetailReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.PurchaseDetailReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("入库明细报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region  入库汇总报表
        //[PermissionAuthorize("Report_InboundReport")]
        public ActionResult InboundReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInboundReport(InboundGlobalQuery inboundReportQuery)
        {
            var rsp = ReportApiClient.GetInstance().GetInboundReport(LoginCoreQuery, inboundReportQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult InboundReportExport(InboundGlobalQuery inboundReportQuery)
        {
            inboundReportQuery.iDisplayStart = 0;
            inboundReportQuery.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetInboundReport(LoginCoreQuery, inboundReportQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.InboundReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("入库汇总报告-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 冻结商品明细报表
        //[PermissionAuthorize("Report_FrozenSkuReport")]
        public ActionResult FrozenSkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetFrozenSkuReport(FrozenSkuGlobalQuery frozenSkuReportQuery)
        {
            var rsp = ReportApiClient.GetInstance().GetFrozenSkuReport(LoginCoreQuery, frozenSkuReportQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult FrozenSkuReportExportByFile(FrozenSkuGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetFrozenSkuReport(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.FrozenSkuReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("冻结商品报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region SN管理报表
        //[PermissionAuthorize("Report_SNManageReport")]
        public ActionResult SNManageReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetSNManageReport(SNManageGlobalQuery query)
        {
            if (query.PurchaseDateTo.HasValue)
                query.PurchaseDateTo = query.PurchaseDateTo.Value.AddDays(1);

            if (query.OutboundDateTo.HasValue)
                query.OutboundDateTo = query.OutboundDateTo.Value.AddDays(1);

            var rsp = ReportApiClient.GetInstance().GetSNManageReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        #endregion

        #region 收发货明细
        /// <summary>
        /// 收发货明细报表
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceivedAndSendSkuReport()
        {
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetReceivedAndSendSkuReport(ReceivedAndSendSkuGlobalQuery dto)
        {
            if (dto.EndTime.HasValue)
                dto.EndTime = dto.EndTime.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetReceivedAndSendSkuReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult ReceivedAndSendSkuReportExport(ReceivedAndSendSkuGlobalQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 100000;
            if (dto.EndTime.HasValue)
                dto.EndTime = dto.EndTime.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetReceivedAndSendSkuReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.ReceivedAndSendSkuReportExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("收发货明细报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 出库处理时间统计表
        public ActionResult OutboundHandleDateReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsGlobalQuery dto)
        {
            if (dto.ActualShipDateTo.HasValue)
                dto.ActualShipDateTo = dto.ActualShipDateTo.Value.AddDays(1).AddMilliseconds(-1);
            if (dto.CreateDateTo.HasValue)
                dto.CreateDateTo = dto.CreateDateTo.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetOutboundHandleDateStatisticsReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult OutboundHandleDateStatisticsExport(OutboundHandleDateStatisticsGlobalQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 100000;
            if (dto.ActualShipDateTo.HasValue)
                dto.ActualShipDateTo = dto.ActualShipDateTo.Value.AddDays(1).AddMilliseconds(-1);
            if (dto.CreateDateTo.HasValue)
                dto.CreateDateTo = dto.CreateDateTo.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetOutboundHandleDateStatisticsReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundHandleDateStatisticsExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库处理时间统计表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 收货、上架时间统计表
        public ActionResult ReceiptAndDeliveryDateReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }


        [SetUserInfo]
        public ActionResult GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateGlobalQuery dto)
        {
            if (dto.CreateDateTo.HasValue)
                dto.CreateDateTo = dto.CreateDateTo.Value.AddDays(1).AddMilliseconds(-1);
            if (dto.AuditingDateTo.HasValue)
                dto.AuditingDateTo = dto.AuditingDateTo.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetReceiptAndDeliveryDateReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult ReceiptAndDeliveryDateExport(ReceiptAndDeliveryDateGlobalQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 1000000;
            if (dto.CreateDateTo.HasValue)
                dto.CreateDateTo = dto.CreateDateTo.Value.AddDays(1).AddMilliseconds(-1);
            if (dto.AuditingDateTo.HasValue)
                dto.AuditingDateTo = dto.AuditingDateTo.Value.AddDays(1).AddMilliseconds(-1);
            var rsp = ReportApiClient.GetInstance().GetReceiptAndDeliveryDateReport(this.LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.ReceiptAndDeliveryDateExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("收货上架时间统计表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 监控报表

        public ActionResult MonitorReport()
        {
            return View();
        }

        public ActionResult GetMonitorReportPurchaseDto()
        {
            MonitorReportQuery request = new MonitorReportQuery();
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            request.iDisplayStart = Request["page"] != null ? int.Parse(Request["page"]) : 1;
            request.iDisplayLength = Request["rows"] != null ? int.Parse(Request["rows"]) : 50;
            request.orderBy = Request["sidx"] == null ? "" : Request["sidx"];
            request.desc = Request["sord"] == null ? "" : Request["sord"];

            var rsp = ReportApiClient.GetInstance().GetMonitorReportPurchaseDto(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    records = rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    page = request.iDisplayStart,
                    total = rsp.ResponseResult.TableResuls.iTotalDisplayRecords / request.iDisplayLength + 1,
                    rows = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetMonitorReportOutBoundDto()
        {
            MonitorReportQuery request = new MonitorReportQuery();
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            request.iDisplayStart = Request["page"] != null ? int.Parse(Request["page"]) : 1;
            request.iDisplayLength = Request["rows"] != null ? int.Parse(Request["rows"]) : 50;
            request.orderBy = Request["sidx"] == null ? "" : Request["sidx"];
            request.desc = Request["sord"] == null ? "" : Request["sord"];

            var rsp = ReportApiClient.GetInstance().GetMonitorReportOutBoundDto(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    records = rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    page = request.iDisplayStart,
                    total = rsp.ResponseResult.TableResuls.iTotalDisplayRecords / request.iDisplayLength + 1,
                    rows = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetMonitorReportWorkDto()
        {
            MonitorReportQuery request = new MonitorReportQuery();
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            request.iDisplayStart = Request["page"] != null ? int.Parse(Request["page"]) : 1;
            request.iDisplayLength = Request["rows"] != null ? int.Parse(Request["rows"]) : 50;
            request.orderBy = Request["sidx"] == null ? "" : Request["sidx"];
            request.desc = Request["sord"] == null ? "" : Request["sord"];

            var rsp = ReportApiClient.GetInstance().GetMonitorReportWorkDto(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    records = rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    page = request.iDisplayStart,
                    total = rsp.ResponseResult.TableResuls.iTotalDisplayRecords / request.iDisplayLength + 1,
                    rows = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetMonitorReportPurchaseReturnDto()
        {
            MonitorReportQuery request = new MonitorReportQuery();
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            request.iDisplayStart = Request["page"] != null ? int.Parse(Request["page"]) : 1;
            request.iDisplayLength = Request["rows"] != null ? int.Parse(Request["rows"]) : 50;
            request.orderBy = Request["sidx"] == null ? "" : Request["sidx"];
            request.desc = Request["sord"] == null ? "" : Request["sord"];

            var rsp = ReportApiClient.GetInstance().GetMonitorReportPurchaseReturnDto(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    records = rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    page = request.iDisplayStart,
                    total = rsp.ResponseResult.TableResuls.iTotalDisplayRecords / request.iDisplayLength + 1,
                    rows = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetStockTransferDto()
        {
            MonitorReportQuery request = new MonitorReportQuery();
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            request.iDisplayStart = Request["page"] != null ? int.Parse(Request["page"]) : 1;
            request.iDisplayLength = Request["rows"] != null ? int.Parse(Request["rows"]) : 50;
            request.orderBy = Request["sidx"] == null ? "" : Request["sidx"];
            request.desc = Request["sord"] == null ? "" : Request["sord"];

            var rsp = ReportApiClient.GetInstance().GetMonitorReportStockTransferDto(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    records = rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    page = request.iDisplayStart,
                    total = rsp.ResponseResult.TableResuls.iTotalDisplayRecords / request.iDisplayLength + 1,
                    rows = rsp.ResponseResult.TableResuls.aaData
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion

        #region 异常汇总报告
        //[PermissionAuthorize("Report_OutboundExceptionReport")]
        public ActionResult OutboundExceptionReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetOutboundExceptionReport(OutboundExceptionGlobalQuery query)
        {
            var rsp = ReportApiClient.GetInstance().GetOutboundExceptionReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult OutboundExceptionReportExport(OutboundExceptionGlobalQuery query)
        {
            query.iDisplayStart = 0;
            query.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetOutboundExceptionReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundExceptionReportExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库异常汇总报告-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 出库箱数统计报表
        //[PermissionAuthorize("Report_OutboundBoxReport")]
        public ActionResult OutboundBoxReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundBoxReport(OutboundBoxGlobalQuery query)
        {
            if (query.CreateDateEnd.HasValue)
            {
                query.CreateDateEnd = query.CreateDateEnd.Value.AddDays(1).AddMilliseconds(-1);
            }
            if (query.ActualShipDateEnd.HasValue)
            {
                query.ActualShipDateEnd = query.ActualShipDateEnd.Value.AddDays(1).AddMilliseconds(-1);
            }
            var rsp = ReportApiClient.GetInstance().GetOutboundBoxReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult OutboundBoxReportExport(OutboundBoxGlobalQuery query)
        {
            if (query.CreateDateEnd.HasValue)
            {
                query.CreateDateEnd = query.CreateDateEnd.Value.AddDays(1).AddMilliseconds(-1);
            }
            if (query.ActualShipDateEnd.HasValue)
            {
                query.ActualShipDateEnd = query.ActualShipDateEnd.Value.AddDays(1).AddMilliseconds(-1);
            }
            HSSFWorkbook book = new HSSFWorkbook();
            for (int i = 0; i < 5; i++)
            {
                query.iDisplayStart = i * PublicConst.EachQuestDataRowsCount;
                query.iDisplayLength = PublicConst.EachQuestDataRowsCount;
                var rsp = ReportApiClient.GetInstance().GetOutboundBoxReport(LoginCoreQuery, query);
                if (rsp.Success == true && rsp.ResponseResult.TableResuls.aaData != null && rsp.ResponseResult.TableResuls.aaData.Count > 0)
                {
                    book = NPOIExtend.OutboundBoxReportExport(rsp.ResponseResult.TableResuls.aaData, book, i);
                }
                else
                {
                    break;
                }
            }

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel",
                string.Format("出库箱数统计报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
        }
        #endregion

        #region B2C结算报表

        public ActionResult BalanceReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetBalanceReport(BalanceGlobalQuery query)
        {
            var rsp = ReportApiClient.GetInstance().GetBalanceReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult BalanceReportExport(BalanceGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetBalanceReport(LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.BalanceReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("B2C结算报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 整散箱装箱明细报表

        public ActionResult OutboundPackReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetOutboundPackReport(OutboundPackGlobalQuery query)
        {
            if (query.ActualShipDateTo.HasValue)
                query.ActualShipDateTo = query.ActualShipDateTo.Value.AddDays(1);

            var rsp = ReportApiClient.GetInstance().GetOutboundPackReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult OutboundPackReportExport(OutboundPackGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetOutboundPackReport(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundPackReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("整散箱装箱明细报表统计-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        public ActionResult OutboundPackSkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetOutboundPackSkuReport(OutboundPackSkuGlobalQuery query)
        {
            if (query.ActualShipDateTo.HasValue)
                query.ActualShipDateTo = query.ActualShipDateTo.Value.AddDays(1);

            var rsp = ReportApiClient.GetInstance().GetOutboundPackSkuReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult OutboundPackSkuReportExport(OutboundPackSkuGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetOutboundPackSkuReport(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundPackSkuReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("整散箱商品明细报表统计-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 商品包装查询报表
        public ActionResult SkuPackReport()
        {
            return View();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSkuPackReport(SkuPackGlobalQuery dto)
        {
            var rsp = ReportApiClient.GetInstance().GetSkuPackReport(LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SkuPackReportExport(SkuPackGlobalQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetSkuPackReport(LoginCoreQuery, dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.SkuPackReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("商品包装报表统计-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 出库汇总报表

        public ActionResult OutboundSummaryReport(string createDateFrom = null, string createDateTo = null)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.CreateDateFrom = createDateFrom;
            ViewBag.CreateDateTo = createDateTo;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundByPage(OutboundGlobalQuery request)
        {
            if (!string.IsNullOrEmpty(request.OutboundOrder))
                request.OutboundOrder.Replace('，', ',');
            if (request.CreateDateTo.HasValue)
                request.CreateDateTo = request.CreateDateTo.Value.AddDays(1);
            if (request.AuditingDateTo.HasValue)
                request.AuditingDateTo = request.AuditingDateTo.Value.AddDays(1);
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            if (request.Region.Trim() == "请输入")
                request.Region = string.Empty;
            if (request.DepartureDateTo.HasValue)
                request.DepartureDateTo = request.DepartureDateTo.Value.AddDays(1);

            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = ReportApiClient.GetInstance().GetOutboundByPage(request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [SetUserInfo]
        public ActionResult OutboundSummaryExport(OutboundGlobalQuery request)
        {
            request.IsExport = true;
            if (!string.IsNullOrEmpty(request.OutboundOrder))
                request.OutboundOrder.Replace('，', ',');
            if (request.CreateDateTo.HasValue)
                request.CreateDateTo = request.CreateDateTo.Value.AddDays(1);
            if (request.AuditingDateTo.HasValue)
                request.AuditingDateTo = request.AuditingDateTo.Value.AddDays(1);
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            if (request.Region.Trim() == "请输入")
                request.Region = string.Empty;
            if (request.DepartureDateTo.HasValue)
                request.DepartureDateTo = request.DepartureDateTo.Value.AddDays(1);

            request.WarehouseSysId = CurrentUser.WarehouseSysId;

            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().OutboundSummaryExport(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { success = true, filePath = rsp.ResponseResult.HttpFullPathName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region 移仓单报表
        public ActionResult TransferinventoryReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetTransferinventoryReport(TransferinventoryGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetTransferinventoryReport(request, LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion

        #region 出库单商品汇总报表

        public ActionResult OutboundSkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }


        [SetUserInfo]
        public ActionResult GetOutboundSkuReport(OutboundSkuGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetOutboundSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult OutboundSkuReportExport(OutboundSkuGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetOutboundSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundSkuReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库单商品汇总报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region 出库捡货工时报表

        public ActionResult PickingTimeSpanReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetPickingTimeSpanReport(PickingTimeSpanGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetPickingTimeSpanReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult PickingTimeSpanReportExport(PickingTimeSpanGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetPickingTimeSpanReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.PickingTimeSpanReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库捡货工时报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 出库复核工时报表

        public ActionResult OutboundTransferOrderReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundTransferOrderReport(OutboundTransferOrderGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetOutboundTransferOrderReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult OutboundTransferOrderReportExport(OutboundTransferOrderGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetOutboundTransferOrderReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundTransferOrderReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库捡货工时报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 退货单报表
        public ActionResult ReturnOrderReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        [SetUserInfo]
        public ActionResult GetReturnOrderReport(ReturnOrderGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetReturnOrderReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult ReturnOrderReportExportByFile(ReturnOrderGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetReturnOrderReportExport(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { success = true, filePath = rsp.ResponseResult.HttpFullPathName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 农资出库商品报表

        public ActionResult FertilizerOutboundSkuReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        //[SetUserInfo]
        public ActionResult GetFertilizerOutboundSkuReport(FertilizerOutboundSkuGlobalQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetFertilizerOutboundSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult FertilizerOutboundSkuReportExport(FertilizerOutboundSkuGlobalQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 100000;
            var rsp = ReportApiClient.GetInstance().GetFertilizerOutboundSkuReport(request, this.LoginCoreQuery);
            if (rsp.ResponseResult.TableResuls != null)
            {

                HSSFWorkbook book = NPOIExtend.FertilizerOutboundSkuReportExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("农资出库商品报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 渠道库存查询
        /// <summary>
        /// 渠道库存
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelInventoryReport()
        {
            ViewBag.WarehouseList = AllWareHouse.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetChannelInventoryByPage(ChannelInventoryGlobalQuery query)
        {
            var rsp = ReportApiClient.GetInstance().GetChannelInventoryByPage(query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.TableResuls.aaData,
                    rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.TableResuls.iTotalRecords,
                    rsp.ResponseResult.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult ChannelInventoryExport(ChannelInventoryGlobalQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetChannelInventoryByPage(dto);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.ChannelInventoryExport(rsp.ResponseResult.TableResuls.aaData);

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("渠道库存报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using System.Web.Routing;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.DTO.Report;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        [PermissionAuthorize("Report_InvTransBySkuReport")]
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

        public ActionResult ViewInvTransDocOrder(Guid sysId, string docOrder)
        {
            string actionName = null;
            string controllerName = null;
            RouteValueDictionary routeValues = null;
            if (docOrder.StartsWith("GR", StringComparison.OrdinalIgnoreCase))
            {   //
                actionName = "ReceiptView";
                controllerName = "Receipt";
                routeValues = new RouteValueDictionary { { "sysId", sysId } };
            }
            else if (docOrder.StartsWith("PN", StringComparison.OrdinalIgnoreCase))
            {   //
                actionName = "VanningView";
                controllerName = "Vanning";
                var rsp = OutboundApiClient.GetInstance().GetVanningSysIdByVanningDetailSysId(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
                if (rsp.Success && rsp.ResponseResult.HasValue)
                {
                    routeValues = new RouteValueDictionary { { "sysId", rsp.ResponseResult.Value } };
                }
            }
            else if (docOrder.StartsWith("OB", StringComparison.OrdinalIgnoreCase))
            {   //出库单
                actionName = "OutboundView";
                controllerName = "Outbound";
                routeValues = new RouteValueDictionary { { "sysId", sysId } };
            }
            else if (docOrder.StartsWith("PL", StringComparison.OrdinalIgnoreCase))
            {   //损益单
                actionName = "AjustmentView";
                controllerName = "Ajustment";
                routeValues = new RouteValueDictionary { { "sysId", sysId } };
            }

            else if (docOrder.StartsWith("PO", StringComparison.OrdinalIgnoreCase))
            {
                if (docOrder.Contains("-"))
                {
                    //收货单
                    actionName = "ReceiptView";
                    controllerName = "Receipt";
                    routeValues = new RouteValueDictionary { { "sysId", sysId } };
                }
                else
                {
                    //入库单
                    actionName = "PurchaseView";
                    controllerName = "Purchase";
                    routeValues = new RouteValueDictionary { { "sysId", sysId } };
                }
            }

            else if (docOrder.StartsWith("BZ", StringComparison.OrdinalIgnoreCase))
            {   //入库单
                actionName = "AssemblyView";
                controllerName = "Assembly";
                routeValues = new RouteValueDictionary { { "sysId", sysId } };
            }
            else if (docOrder.StartsWith("BO", StringComparison.OrdinalIgnoreCase))
            {
                actionName = "SkuBorrowView";
                controllerName = "SkuBorrow";
                routeValues = new RouteValueDictionary { { "sysId", sysId } };
            }
            return RedirectToAction(actionName, controllerName, routeValues);
        }

        #region 货位库存查询
        public ActionResult InvLocBySkuReport()
        {
            return View();
        }

        public ActionResult GetInvLocBySkuReport(InvSkuLocReportQuery request)
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

        public ActionResult InvLocBySkuReportExport(InvSkuLocReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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

        #region 批次库存查询
        public ActionResult InvLotBySkuReport()
        {
            return View();
        }

        public ActionResult GetInvLotBySkuReport(InvSkuLotReportQuery request)
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

        public ActionResult InvLotLocLpnBySkuReport()
        {
            return View();
        }

        public ActionResult GetInvLotLocLpnBySkuReport(InvSkuLotLocLpnReportQuery request)
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

        public ActionResult ExpiryInvLotBySkuReport()
        {
            return View();
        }

        public ActionResult GetExpiryInvLotBySkuReport(InvSkuLotReportQuery request)
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

        public ActionResult ReceiptDetailReport()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetReceiptDetailReport(ReceiptDetailReportQuery request)
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
        public ActionResult ReceiptDetailReportExport(ReceiptDetailReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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

        #region 出库明细
        public ActionResult OutboundDetailReport()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundDetailReport(OutboundDetailReportQuery request)
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
        public ActionResult OutboundDetailReportExport(OutboundDetailReportQuery request)
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
                        return Content("<script>alert('导出数据条数大于10W, 请联系管理员!');location.href='/Report/OutboundDetailReport';</script>");
                    }
                    book = NPOIExtend.OutboundDetailReportExport(rsp.ResponseResult.TableResuls.aaData, book, i + 1);
                }
                else
                {
                    if (rsp.Success == false)
                    {
                        return Content("<script>alert('" + rsp.ApiMessage.ErrorMessage + "');location.href='/Report/OutboundDetailReport';</script>");
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel",
                string.Format("出库明细-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
        }

        public ActionResult OutboundDetailReportExportByFile(OutboundDetailReportQuery request)
        {
            request.IsExport = true;
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetOutboundDetailReportByFile(request, this.LoginCoreQuery);
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

        public ActionResult InvSkuReport()
        {
            //var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            //if (rsp.Success && rsp.ResponseResult != null)
            //{
            //    ViewBag.WarehouseList = rsp.ResponseResult;
            //}
            ViewBag.WarehouseList = CurrentUser.WareHouseList.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetInvSkuReport(InvSkuReportQuery request)
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
        public ActionResult InvSkuReportExport(InvSkuReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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

        public ActionResult FinanceInvoicingReport()
        {
            //var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            //if (rsp.Success && rsp.ResponseResult != null)
            //{
            //    ViewBag.WarehouseList = rsp.ResponseResult;
            //}
            ViewBag.WarehouseList = CurrentUser.WareHouseList.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }

        public ActionResult GetFinanceInvoicingReport(FinanceInvoicingReportQueryDto request)
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

        public ActionResult FinanceInvoicingReportExport(FinanceInvoicingReportQueryDto request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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

        public ActionResult AdjustmentDetailReport()
        {
            return View();
        }

        public ActionResult GetAdjustmentDetailReport(AdjustmentDetailReportQuery request)
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

        #region 入库明细报表
        /// <summary>
        /// 入库明细报表
        /// </summary>
        /// <returns></returns>
        public ActionResult PurchaseDetailByReport()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetPurchaseDetailReport(PurchaseDetailReportQuery request)
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

        public ActionResult PurchaseDetailReportExport(PurchaseDetailReportQuery request)
        {
            if (request.LastReceiptDateTo.HasValue)
                request.LastReceiptDateTo = request.LastReceiptDateTo.Value.AddDays(1);
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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


        [PermissionAuthorize("Report_InboundReport")]
        public ActionResult InboundReport()
        {
            return View();
        }

        public ActionResult GetInboundReport(InboundReportQuery inboundReportQuery)
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

        public ActionResult InboundReportExport(InboundReportQuery inboundReportQuery)
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

        [PermissionAuthorize("Report_FrozenSkuReport")]
        public ActionResult FrozenSkuReport()
        {
            return View();
        }

        public ActionResult GetFrozenSkuReport(FrozenSkuReportQuery frozenSkuReportQuery)
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

        public ActionResult FrozenSkuReportExportByFile(FrozenSkuReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            var rsp = ReportApiClient.GetInstance().GetFrozenSkuReportByFile(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { success = true, filePath = rsp.ResponseResult.HttpFullPathName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [PermissionAuthorize("Report_SNManageReport")]
        public ActionResult SNManageReport()
        {
            return View();
        }

        public ActionResult GetSNManageReport(SNManageReportQuery query)
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


        #region 收发货明细
        /// <summary>
        /// 收发货明细报表
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceivedAndSendSkuReport()
        {
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            ViewBag.WarehouseList = CurrentUser.WareHouseList.Select(p => new SelectItem() { Text = p.Name, Value = p.SysId.ToString() }).ToList();
            return View();
        }

        public ActionResult GetReceivedAndSendSkuReport(ReceivedAndSendSkuReportQuery dto)
        {
            //if (dto != null)
            //{
            //    if (dto.SeachWarehouseSysId == null)
            //    {
            //        dto.SeachWarehouseSysIdList = CurrentUser.WareHouseList.Select(x => x.SysId).ToList();
            //    }
            //}
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


        public ActionResult ReceivedAndSendSkuReportExport(ReceivedAndSendSkuReportQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 1000000;
            //if (dto != null)
            //{
            //    if (dto.SeachWarehouseSysId == null)
            //    {
            //        dto.SeachWarehouseSysIdList = CurrentUser.WareHouseList.Select(x => x.SysId).ToList();
            //    }
            //}
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
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundHandleDateStatisticsReport(OutboundHandleDateStatisticsQuery dto)
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
        public ActionResult OutboundHandleDateStatisticsExport(OutboundHandleDateStatisticsQuery dto)
        {
            dto.iDisplayStart = 0;
            dto.iDisplayLength = 1000000;
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
            ViewBag.StartDate = DateTime.Now.AddDays(-7).ToString(PublicConst.DateFormat);
            ViewBag.EndDate = DateTime.Now.ToString(PublicConst.DateFormat);
            return View();
        }


        [SetUserInfo]
        public ActionResult GetReceiptAndDeliveryDateReport(ReceiptAndDeliveryDateQuery dto)
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
        public ActionResult ReceiptAndDeliveryDateExport(ReceiptAndDeliveryDateQuery dto)
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
        [PermissionAuthorize("Report_OutboundExceptionReport")]
        public ActionResult OutboundExceptionReport()
        {
            return View();
        }

        public ActionResult GetOutboundExceptionReport(OutboundExceptionReportQuery query)
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

        public ActionResult OutboundExceptionReportExport(OutboundExceptionReportQuery query)
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
        [PermissionAuthorize("Report_OutboundBoxReport")]
        public ActionResult OutboundBoxReport()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundBoxReport(OutboundBoxReportQuery query)
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
        public ActionResult OutboundBoxReportExport(OutboundBoxReportQuery query)
        {
            query.iDisplayStart = 0;
            query.iDisplayLength = 1000000;
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
                HSSFWorkbook book = NPOIExtend.OutboundBoxReportExport(rsp.ResponseResult.TableResuls.aaData);
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel",
                    string.Format("出库箱数统计报表-{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region B2C结算报表

        public ActionResult BalanceReport()
        {
            return View();
        }

        public ActionResult GetBalanceReport(BalanceReportQuery query)
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

        public ActionResult BalanceReportExport(BalanceReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
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
            return View();
        }

        public ActionResult GetOutboundPackReport(OutboundPackReportQuery query)
        {
            if (query.ActualShipDateTo.HasValue)
                query.ActualShipDateTo = query.ActualShipDateTo.Value.AddDays(1);

            var rsp = ReportApiClient.GetInstance().GetOutboundPackReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                int i = 1;
                rsp.ResponseResult.TableResuls.aaData.ForEach(p =>
                {
                    p.Index = i++;
                });
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

        public ActionResult OutboundPackReportExport(OutboundPackReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetOutboundPackReport(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundPackReportExport(rsp.ResponseResult.TableResuls.aaData, CurrentUser.WarehouseName);

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
            return View();
        }

        public ActionResult GetOutboundPackSkuReport(OutboundPackSkuReportQuery query)
        {
            if (query.ActualShipDateTo.HasValue)
                query.ActualShipDateTo = query.ActualShipDateTo.Value.AddDays(1);

            var rsp = ReportApiClient.GetInstance().GetOutboundPackSkuReport(LoginCoreQuery, query);
            if (rsp.ResponseResult.TableResuls != null)
            {
                int i = 1;
                rsp.ResponseResult.TableResuls.aaData.ForEach(p =>
                {
                    p.Index = i++;
                });
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

        public ActionResult OutboundPackSkuReportExport(OutboundPackSkuReportQuery request)
        {
            request.iDisplayStart = 0;
            request.iDisplayLength = 1000000;
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            var rsp = ReportApiClient.GetInstance().GetOutboundPackSkuReport(this.LoginCoreQuery, request);
            if (rsp.ResponseResult.TableResuls != null)
            {
                HSSFWorkbook book = NPOIExtend.OutboundPackSkuReportExport(rsp.ResponseResult.TableResuls.aaData, CurrentUser.WarehouseName);

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
        public ActionResult GetSkuPackReport(SkuPackReportQuery dto)
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
        public ActionResult SkuPackReportExport(SkuPackReportQuery dto)
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
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                ViewBag.WarehouseList = rsp.ResponseResult;
            }
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundByPage(OutboundQuery request)
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
        public ActionResult OutboundSummaryExport(OutboundQuery request)
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
            request.iDisplayLength = PublicConst.EachExportDataMaxCount;
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

        #region 渠道库存报表
        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelInventoryReport()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetChannelInventoryByPage(ChannelInventoryQuery request)
        {
            var rsp = ReportApiClient.GetInstance().GetChannelInventoryByPage(request);
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
        public ActionResult ChannelInventoryExport(ChannelInventoryQuery dto)
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
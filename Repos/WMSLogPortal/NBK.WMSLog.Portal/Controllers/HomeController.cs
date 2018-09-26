using NBK.ECService.WMSLog.DTO;
using NBK.ECService.WMSLog.Utility;
using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInboundBizLogByDays()
        {
            var rsp = LogApiClient.GetInstance().GetInboundBizLogByDays(LoginCoreQuery, PublicConst.ReportDays);

            if (rsp.Success)
            {
                return Json(new { success = true, result = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOutboundBizLogByDays()
        {
            var rsp = LogApiClient.GetInstance().GetOutboundBizLogByDays(LoginCoreQuery, PublicConst.ReportDays);

            if (rsp.Success)
            {
                return Json(new { success = true, result = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHomePageAccessLogStatistic()
        {
            var rsp = LogApiClient.GetInstance().GetHomePageAccessLogStatistic(LoginCoreQuery, PublicConst.ReportDays);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHomePageInterfaceLogStatistic()
        {
            var rsp = LogApiClient.GetInstance().GetHomePageInterfaceLogStatistic(LoginCoreQuery, PublicConst.ReportDays);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetApiProcessResult()
        {
            var rsp = LogApiClient.GetInstance().GetApiProcessResult(LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true, result = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHomePageSummaryLog(SummaryLogQuery summaryLogQuery)
        {
            var rsp = LogApiClient.GetInstance().GetHomePageSummaryLog(LoginCoreQuery, summaryLogQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewLogDetail(Guid sysId, int logType)
        {
            if (logType == 400)
            {
                return View("AccessLogView");
            }
            else if (logType == 500)
            {
                return View("BusinessLogView");
            }
            else
            {
                return View("InterfaceLogView");
            }
        }

        public ActionResult GetAccessLogViewDto(Guid sysId)
        {
            var rsp = LogApiClient.GetInstance().GetAccessLogViewDto(LoginCoreQuery, sysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBusinessLogViewDto(Guid sysId)
        {
            var rsp = LogApiClient.GetInstance().GetBusinessLogViewDto(LoginCoreQuery, sysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInterfaceLogViewDto(Guid sysId)
        {
            var rsp = LogApiClient.GetInstance().GetInterfaceLogViewDto(LoginCoreQuery, sysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHomePageMaxElapsedTimeLog()
        {
            var rsp = LogApiClient.GetInstance().GetHomePageMaxElapsedTimeLog(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetHomePageMaxFrequencyLog()
        {
            var rsp = LogApiClient.GetInstance().GetHomePageMaxFrequencyLog(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewFrequencyDetail(string descr)
        {
            return View("FrequencyDetailView");
        }

        public ActionResult GetFrequencyDetailDto(string descr)
        {
            var rsp = LogApiClient.GetInstance().GetFrequencyDetailDto(LoginCoreQuery, descr);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 检查库存数据
        /// <summary>
        /// 查询有差异的库存(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockDataSet(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockDataSet(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 3张库存表可用数量比较
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockAvailableQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockAvailableQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询有差异的库存分配数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockAllocatedQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockAllocatedQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询有差异的库存拣货数量(3张库存表差异)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockPickedQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockPickedQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// invLot和invLotLocLpn表商品相同批次数量差异
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockSkuLotQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockSkuLotQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// invSkuLoc和invLotLocLpn表商品相同货位数量差异
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockSkuLocQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetStockSkuLocQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询入库，库存，出库的差异
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDiffReceiptInvOut(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetDiffReceiptInvOut(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  库存分配数量和拣货明细分配数量比较
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDiffInvAndPickDetailAllocatedQty(int type)
        {
            var rsp = CheckStockApiClient.GetInstance().GetDiffInvAndPickDetailAllocatedQty(LoginCoreQuery, type);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
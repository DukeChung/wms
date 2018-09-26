using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using WMS.Global.Portal.Models;
using WMS.Global.Portal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMSReport.DTO;

namespace WMS.Global.Portal.Controllers
{
    [Authorize]
    public class MapController : BaseController
    {
        #region 仓库业务分部
        /// <summary>
        /// 仓库业务分部
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryServiceStation()
        {
            return View();
        }


        /// <summary>
        /// 获取仓库业务数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHistoryServiceStationData(int page)
        {
            var rsp = HomeApiClient.GetInstance().GetHistoryServiceStationData(page);
            if (rsp.Success)
            {
                return Json(new { success = true, data = rsp.ResponseResult, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        /// <summary>
        /// 最近仓库发货
        /// </summary>
        /// <returns></returns>
        public ActionResult RealTimeDelivery()
        {
            return View();
        }


        #region 仓库业务分部
        /// <summary>
        /// 仓库服务站关系图
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseStationRelation()
        {
            return View();
        }

        public ActionResult GetWarehouseStationRelation()
        {
            var rsp = HomeApiClient.GetInstance().GetWarehouseStationRelation();
            if (rsp.Success)
            {
                return Json(new { success = true, data = rsp.ResponseResult, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion

        /// <summary>
        /// 仓库日历订单
        /// </summary>
        /// <returns></returns>
        public ActionResult CalendarOrder()
        {
            ViewBag.Month = DateTime.Now.Month;
            ViewBag.Year = DateTime.Now.Year;
            return View();
        }


        /// <summary>
        /// 每日出入库统计
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDailyEventDataCountInfo(string startDate, string endDate)
        {
            var rsp = HomeApiClient.GetInstance().GetDailyEventDataCountInfo(startDate, endDate);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CalendarDataByDate(string date)
        {
            var rsp = HomeApiClient.GetInstance().GetCalendarDataByDate(date);

            var model = new List<CalendarDataByDateDto>();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }


        #region 仓库作业时间统计
        /// <summary>
        /// 仓库作业时间分布和仓库作业类型时间分布
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkDistribution()
        {
            return View();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWorkDistributionData()
        {
            var rsp = HomeApiClient.GetInstance().GetWorkDistributionData();
            if (rsp.Success)
            {
                return Json(new { success = true, data = rsp.ResponseResult, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetWorkDistributionByWarehouse(Guid sysId, int index)
        {
            var rsp = HomeApiClient.GetInstance().GetWorkDistributionByWarehouse(sysId);
            if (rsp.Success)
            {
                return Json(new { success = true, data = rsp.ResponseResult, index = index, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetWorkDistributionPieData()
        {
            var rsp = HomeApiClient.GetInstance().GetWorkDistributionPieData();
            if (rsp.Success)
            {
                return Json(new { success = true, data = rsp.ResponseResult, message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}
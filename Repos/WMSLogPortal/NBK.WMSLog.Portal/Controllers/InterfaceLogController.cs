using NBK.ECService.WMSLog.DTO;
using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    [Authorize]
    public class InterfaceLogController : BaseController
    {
        public ActionResult InterfaceLogList()
        {
            return View();
        }

        public ActionResult GetInterfaceLogList(InterfaceLogQuery interfaceLogQuery)
        {
            var rsp = LogApiClient.GetInstance().GetInterfaceLogList(LoginCoreQuery, interfaceLogQuery);
            if (rsp != null && rsp.Success && rsp.ResponseResult != null)
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


        public ActionResult InvokeInterfaceLogApi(InterfaceLogQuery request)
        {
            var rsp = LogApiClient.GetInstance().InvokeInterfaceLogApi(LoginCoreQuery, request);
            if (rsp != null && rsp.ResponseResult)
            {
                return Json(new
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 接口统计
        /// </summary>
        /// <returns></returns>
        public ActionResult InterfaceStatistic()
        {
            return View();
        }

        public ActionResult GetInterfaceStatisticByPage(InterfaceStatisticQuery dtoQuery)
        {
            var rsp = LogApiClient.GetInstance().GetInterfaceStatisticByPage(LoginCoreQuery, dtoQuery);
            if (rsp != null && rsp.Success && rsp.ResponseResult != null)
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
        /// 重新插入采购单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult InsertOutbound(InterfaceLogQuery request)
        {
            var rsp = LogApiClient.GetInstance().InsertOutbound(LoginCoreQuery, request);
            if (rsp != null && rsp.ResponseResult)
            {
                return Json(new
                {
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
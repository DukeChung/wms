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
    public class AccessLogController : BaseController
    {
        public ActionResult AccessLogList()
        {
            return View();
        }

        public ActionResult GetAccessLogList(AccessLogQuery accessLogQuery)
        {
            var rsp = LogApiClient.GetInstance().GetAccessLogList(LoginCoreQuery, accessLogQuery);
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSGlobalReportPortal.Controllers
{
    public class ReportController : Controller
    {
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
    }
}
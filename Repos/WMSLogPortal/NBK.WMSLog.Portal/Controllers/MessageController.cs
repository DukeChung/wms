using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        public ActionResult SendMessage()
        {
            return View();
        }

        public ActionResult GetWarehouseList()
        {
            var rsp = LogApiClient.GetInstance().GetWarehouseList(LoginCoreQuery, CurrentUser.UserId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
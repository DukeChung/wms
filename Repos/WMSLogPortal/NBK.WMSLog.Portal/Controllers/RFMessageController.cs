using NBK.ECService.WMSLog.DTO;
using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    public class RFMessageController : BaseController
    {
        // GET: RFMessage
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult PushMessage(PushMessageQuery query)
        {
            var rsp = PushMessageApiClient.GetInstance().PushMessage(query);
            if (rsp.Success)
            {
                return Json(new { Success = true,Message= rsp.Content }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.Content }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
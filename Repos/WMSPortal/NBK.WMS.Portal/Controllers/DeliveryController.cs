using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;

namespace NBK.WMS.Portal.Controllers
{
    public class DeliveryController : BaseController
    {
        // GET: Delivery
     

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ScanVanning()
        {
            return View();
        }

        /// <summary>
        /// 获取快递箱号
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDeliveryBoxByOrderNumber(string type,string orderNumber)
        {
            
            var rsp = OutboundApiClient.GetInstance().GetDeliveryBoxByOrderNumber(type, orderNumber, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    ScanDeliveryDto = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message= rsp.ApiMessage.ErrorMessage
                }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult SaveDeliveryByVanningSysId(string sysIds)
        {
            var rsp = OutboundApiClient.GetInstance().SaveDeliveryByVanningSysId(sysIds.ToGuidList(), CurrentUser.UserName, CurrentUser.UserId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    ScanDeliveryDto = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = rsp.ApiMessage.ErrorMessage
                }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class CommonController : BaseController
    {
        public ActionResult ChooseSkuFromSameUPCPartial(string upc, string skuName, string skuCode)
        {
            ViewBag.UPC = upc;
            ViewBag.skuName = skuName;
            ViewBag.skuCode = skuCode;
            return PartialView();
            //return View();
        }

        public ActionResult GetSkuListWithUPC(DuplicateUPCChooseQuery request)
        {
            if (!string.IsNullOrEmpty(request.ExcludeSkuSysIdString))
            {
                request.ExcludeSkuSysId = request.ExcludeSkuSysIdString.Split(',').ToList();
            }
            var rsp = BaseDataApiClient.GetInstance().GetSkuAndSkuPackListByUPC(LoginCoreQuery, request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new
                {
                    aaData = rsp.ResponseResult.OrderBy(p => p.SkuName),
                    iTotalDisplayRecords = rsp.ResponseResult.Count,
                    iTotalRecords = rsp.ResponseResult.Count,
                    sEcho = 0

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult DemoChooseSku()
        {
            return View();
        }
    }
}
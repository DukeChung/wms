using NBK.ECService.WMSReport.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Global.Portal.Services;

namespace WMS.Global.Portal.Controllers
{
    public class FertilizerController : Controller
    {
        // GET: Fertilizer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFertilizerRORadarList(FertilizerRORadarGlobalQuery request)
        {
            var rsp = FertilizerApiClient.GetInstance().GetFertilizerRORadarList(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var indicator = new List<object>();
                foreach (var item in rsp.ResponseResult)
                {
                    indicator.Add(new { name = item.SkuName, max = item.Max });
                }
                var rdata = rsp.ResponseResult.Select(p => p.RQty).ToList();
                var odata = rsp.ResponseResult.Select(p => p.OQty).ToList();
                return Json(new { Success = true, Indicator = indicator, RData = rdata, OData = odata }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFertilizerInvRadarList(FertilizerInvRadarGlobalQuery request)
        {
            var rsp = FertilizerApiClient.GetInstance().GetFertilizerInvRadarList(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var indicator = new List<object>();
                foreach (var item in rsp.ResponseResult)
                {
                    indicator.Add(new { name = item.SkuName, max = item.Max });
                }
                var data = rsp.ResponseResult.Select(p => p.Qty).ToList();
                return Json(new { Success = true, Indicator = indicator, Data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFertilizerInvPieList(FertilizerInvPieGlobalQuery request)
        {
            var rsp = FertilizerApiClient.GetInstance().GetFertilizerInvPieList(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var data = new List<object>();
                foreach (var item in rsp.ResponseResult)
                {
                    data.Add(new { name = item.WarehouseName, value = item.Qty });
                }
                return Json(new { Success = true, LegendData = rsp.ResponseResult.Select(p=>p.WarehouseName).ToList(), Data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
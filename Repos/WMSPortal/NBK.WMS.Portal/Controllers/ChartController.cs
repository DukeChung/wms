using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.WMS.Portal.Controllers
{
    public class ChartController : BaseController
    {
        /// <summary>
        /// 出库入库单据占比
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPurchaseAndOutboundChart()
        {
            var rsp = ChartApiClient.GetInstance().GetPurchaseAndOutboundChart(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                rsp.ResponseResult.ChartDate = DateTime.Now.ToString(PublicConst.DateTimeFormat);
                return Json(new
                {
                    success =true,
                    ChartData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success=false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAdventSkuChart()
        {
            var rsp = ChartApiClient.GetInstance().GetAdventSkuChartTop5(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            { 
                return Json(new
                {
                    success = true,
                    ChartData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSkuReceiptOutboundChartAfter10()
        {
            var rsp = ChartApiClient.GetInstance().GetSkuReceiptOutboundChartAfter10(this.LoginCoreQuery, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    ChartData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOutboundAndReturnCharDataOfLastTenDays()
        {
            var rsp = ChartApiClient.GetInstance().GetOutboundAndReturnCharDataOfLastTenDays(LoginCoreQuery, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                CharDataDto charDataDto = new CharDataDto { datasets = new DataSet[2] };
                charDataDto.labels = rsp.ResponseResult.OrderBy(p => p.DisplayOrder).Select(p => p.Date.ToShortDateString()).ToArray();
                charDataDto.datasets[0] = new DataSet
                {
                    label = "近十日订单",
                    backgroundColor = "#b5b8cf",
                    pointBorderColor = "#fff",
                    data = rsp.ResponseResult.OrderBy(p => p.DisplayOrder).Select(p => p.OutboundTotalCount).ToArray()
                };
                charDataDto.datasets[1] = new DataSet
                {
                    label = "近十日退货",
                    backgroundColor = "rgba(26,179,148,0.5)",
                    borderColor = "rgba(26,179,148,0.7)",
                    pointBackgroundColor = "rgba(26,179,148,1)",
                    pointBorderColor = "#fff",
                    data = rsp.ResponseResult.OrderBy(p => p.DisplayOrder).Select(p => p.ReturnTotalCount).ToArray()
                };
                return Json(new { Success = true, Data = charDataDto }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 预包装看板
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPrePackBoard()
        {
            var rsp = ChartApiClient.GetInstance().GetPrePackBoardTop12(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    ChartData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = false,
            }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 超过三天未收货的入库单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExceedThreeDaysPurchase()
        {
            var rsp = ChartApiClient.GetInstance().GetExceedThreeDaysPurchase(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    Success = true,
                    ChartData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Success = false,
            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetToDayOrderStatusTotal()
        {
            var rsp = ChartApiClient.GetInstance().GetToDayOrderStatusTotal(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    TotalCount = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = false,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
#region Using

using System.Linq;
using System.Text;
using System.Web.Mvc;
using WMS.Report.Portal.Services;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;

#endregion

namespace WMS.Report.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: home/index
        public ActionResult Index()
        {
            return View();
        }

        ///// <summary>
        ///// 最新仓库发货分布
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult WareHouse()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 系统流量进度条
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetProgressBarData()
        //{
        //    var progressBarSummary = HomeApiClient.GetInstance().GetProgressBarSummaryDto();
        //    if (progressBarSummary.Success && progressBarSummary.ResponseResult != null)
        //    {
        //        return Json(new { Success = true, Data = progressBarSummary.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetPieChartSummaryData()
        //{

        //    var pieChartSummary = HomeApiClient.GetInstance().GetPieChartSummaryDto();
        //    if (pieChartSummary.Success && pieChartSummary.ResponseResult != null)
        //    {
        //        return Json(new { Success = true, Data = pieChartSummary.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //public ActionResult GetSparkLineSummaryDto()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetSparkLineSummaryDto();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetFlotChartSummaryDto()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetFlotChartSummaryDto();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult Social()
        //{
        //    return View();
        //}

        //// GET: home/inbox
        //public ActionResult Inbox()
        //{
        //    return View();
        //}

        //// GET: home/widgets
        //public ActionResult Widgets()
        //{
        //    return View();
        //}

        //// GET: home/chat
        //public ActionResult Chat()
        //{
        //    return View();
        //}

        //public string GetBirdsEysSource()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetBirdsEysSource();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        return rsp.ResponseResult;

        //    }
        //    else
        //    {
        //        return "[]";
        //    }
        //}

        ///// <summary>
        ///// 获取当前出库信息
        ///// </summary>
        ///// <returns></returns>
        //public string GetCurrentStars()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetNewOutbound();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        StringBuilder outboundJson = new StringBuilder();
        //        var result = string.Empty;

        //        if (rsp.ResponseResult.Any())
        //        {
        //            foreach (var info in rsp.ResponseResult)
        //            {
        //                outboundJson.AppendFormat("[{0},{1},8],", info.Lng, info.Lat);
        //            }
        //        }
        //        if (outboundJson.Length > 0)
        //        {
        //            result = outboundJson.ToString().Substring(0, outboundJson.Length - 1);
        //        }
        //        return "[" + result + "]";
        //    }
        //    else
        //    {
        //        return "[]";
        //    }
        //}

        //public ActionResult GetDailyEventDataCountInfo()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetDailyEventDataCountInfo();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetApiProcessResult()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetApiProcessResult();

        //    if (rsp.Success)
        //    {
        //        return Json(new { success = true, result = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetServiceStationReceiptInfo(ServiceStationReceiptQuery request)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetServiceStationReceiptInfo(request);
        //    if (rsp.ResponseResult.TableResuls != null)
        //    {
        //        return Json(new
        //        {
        //            rsp.ResponseResult.TableResuls.aaData,
        //            rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
        //            rsp.ResponseResult.TableResuls.iTotalRecords,
        //            rsp.ResponseResult.TableResuls.sEcho

        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        //public ActionResult GetTurnoverSkuPage(BaseQuery baseQuery)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetTurnoverSkuPage(baseQuery);
        //    if (rsp.ResponseResult.TableResuls != null)
        //    {
        //        return Json(new
        //        {
        //            rsp.ResponseResult.TableResuls.aaData,
        //            rsp.ResponseResult.TableResuls.iTotalDisplayRecords,
        //            rsp.ResponseResult.TableResuls.iTotalRecords,
        //            rsp.ResponseResult.TableResuls.sEcho

        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        //public ActionResult InvSkuChange()
        //{
        //    return View();
        //}

        //public ActionResult GetInvSkuChangeByPage(InvSkuChangeReportQuery request)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetInvSkuChangeReport(request);
        //    if (rsp.ResponseResult != null && rsp.ResponseResult.TableResuls != null)
        //    {
        //        return Json(rsp.ResponseResult.TableResuls, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 商品发货量最大排名
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ActionResult GetMaxSkuShipmentsRank(BaseQuery request)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetMaxSkuShipmentsRank(request);
        //    if (rsp.ResponseResult != null)
        //    {
        //        return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 商品发货量最小排名
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ActionResult GetMinSkuShipmentsRank(BaseQuery request)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetMinSkuShipmentsRank(request);
        //    if (rsp.ResponseResult != null)
        //    {
        //        return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 获取仓库业务数据
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult RealTimeData()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetCurrentOutbound();
        //    if (rsp.Success)
        //    {
        //        return Json(new { success = true, data = rsp.ResponseResult, message = "操作成功" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 获取最近入库数量
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetReceiptListData()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetReceiptListData();
        //    if (rsp.Success)
        //    {
        //        return Json(new { success = true, date = rsp.ResponseResult.Dates, data = rsp.ResponseResult.ReceiptOutboundList, message = "操作成功" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 获取最近出库数量
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetOutboundListData()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetOutboundListData();
        //    if (rsp.Success)
        //    {
        //        return Json(new { success = true, date = rsp.ResponseResult.Dates, data = rsp.ResponseResult.ReceiptOutboundList, message = "操作成功" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult ChannelInventory(int whflag)
        //{
        //    ViewBag.WHFlag = whflag;
        //    return PartialView();
        //}

        //[ValidateInput(false)]
        //public ActionResult GetChannelInventoryData(int whflag)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetChannelInventoryData(whflag);
        //    if (rsp != null && rsp.Success)
        //    {
        //        rsp.ResponseResult.ForEach(p => p.Channel = string.Format("{0} {1}", p.Channel, p.SumQty));
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 仓库业务分布
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult HistoryServiceStation()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 近七天各渠道发货 入库 情况 （数量）
        ///// </summary>
        ///// <param name="type">类型：1发货，2入库</param>
        ///// <returns></returns>
        //public ActionResult EachChannelInvOut(int flag, int type)
        //{
        //    ViewBag.TypeFlag = flag;
        //    ViewBag.Type = type;
        //    return View();
        //}

        //[ValidateInput(false)]
        //public ActionResult GetEachChannelInvOut(ChannelInvOutChartQueryDto dto)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetEachChannelInvOut(dto);
        //    if (rsp != null && rsp.Success)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 库存种类分布 （入 出 剩）
        ///// </summary>
        ///// <param name="flag"></param>
        ///// <returns></returns>
        //public ActionResult InventoryTypeDistrib(int flag)
        //{
        //    ViewBag.Flag = flag;
        //    return View();
        //}


        //[ValidateInput(false)]
        //public ActionResult GetInventoryTypeDistrib(int flag)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetInventoryTypeDistrib(flag);
        //    if (rsp != null && rsp.Success)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        ///// <summary>
        ///// 仓库作业分布
        ///// </summary>
        ///// <param name="flag"></param>
        ///// <returns></returns>
        //public ActionResult WorkDistribution(int flag)
        //{
        //    ViewBag.Flag = flag;
        //    return View();
        //}


        //[ValidateInput(false)]
        //public ActionResult GetWorkDistribution(int flag)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetWorkDistribution(flag);
        //    if (rsp != null && rsp.Success)
        //    {
        //        return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}
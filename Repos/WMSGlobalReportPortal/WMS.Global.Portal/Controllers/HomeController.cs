using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using WMS.Global.Portal.Models;
using WMS.Global.Portal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMSReport.DTO;

namespace WMS.Global.Portal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region 头部出库，入库百分比统计
        /// <summary>
        /// 获取采购入库，B2B出库，B2C出库总数量
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSparkLineSummaryDto()
        {
            var rsp = HomeApiClient.GetInstance().GetSparkLineSummaryDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPurchaseTypePieDto()
        {
            var rsp = HomeApiClient.GetInstance().GetPurchaseTypePieDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 顶部出库单据类型占比
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOutboundTypePieDto()
        {
            var rsp = HomeApiClient.GetInstance().GetOutboundTypePieDto();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 全局收发存统计
        /// <summary>
        /// 全局收发存统计
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockInOutData()
        {
            var rsp = HomeApiClient.GetInstance().GetStockInOutData();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region  每个仓库收货，出库，库存总数量
        /// <summary>
        /// 获取所有仓库总收货分布
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWareHouseReceiptQtyList()
        {
            var rsp = HomeApiClient.GetInstance().GetWareHouseReceiptQtyList();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取所有仓库总出库分布
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWareHouseOutboundQtyList()
        {
            var rsp = HomeApiClient.GetInstance().GetWareHouseOutboundQtyList();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 获取所有仓库剩余库存
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWareHouseQtyList()
        {
            var rsp = HomeApiClient.GetInstance().GetWareHouseQtyList();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  库龄，滞销Top10，畅销Top10
        /// <summary>
        /// 库龄分析占比
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockAgeGroup()
        {
            var rsp = HomeApiClient.GetInstance().GetStockAgeGroup();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 畅销Top10
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSkuSellingTop10()
        {
            var rsp = HomeApiClient.GetInstance().GetSkuSellingTop10();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 滞销top10
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSkuUnsalableTop10()
        {
            var rsp = HomeApiClient.GetInstance().GetSkuUnsalableTop10();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 渠道占比，服务站发货top10，近期退货top10
        /// <summary>
        /// 渠道占比
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChannelPieData()
        {
            var rsp = HomeApiClient.GetInstance().GetChannelPieData();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServiceStationOutboundTopTen()
        {
            var rsp = HomeApiClient.GetInstance().GetServiceStationOutboundTopTen();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取最新10个退货入库收货完成的单子
        /// </summary>
        /// <returns></returns>
        public ActionResult GetReturnPurchase()
        {
            var rsp = HomeApiClient.GetInstance().GetReturnPurchase();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        /// <summary>
        /// 仓库业务系统访问统计
        /// </summary>
        /// <returns></returns>
        public ActionResult WMSBizAccessGlobal()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAccessBizList()
        {
            var rsp = HomeApiClient.GetInstance().GetAccessBizList();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                var dataList = rsp.ResponseResult;
                
                AccessBizGlobalDto data = new AccessBizGlobalDto();
                data.nodes.Add(new AccessBizGlobalNodesDto() { name = "全部"});

                data.nodes.AddRange(dataList.GroupBy(p => p.FirstBizName).Select(p => new AccessBizGlobalNodesDto() { name = p.Key }));
                data.nodes.AddRange(dataList.GroupBy(p => p.SecondBizName).Select(p => new AccessBizGlobalNodesDto() { name = p.Key }));
                //data.nodes.AddRange(dataList.GroupBy(p => p.BizName).Select(p => new AccessBizGlobalNodesDto() { name = p.Key }));


                var firstBizNameList = dataList.GroupBy(p => p.FirstBizName).Select(p =>  p.Key );
                
                var secondBizNameList = dataList.GroupBy(p => p.SecondBizName).Select(p => p.Key);

                var bizNameList = dataList.GroupBy(p => p.BizName).Select(p => p.Key);

                foreach (var item in firstBizNameList)
                {
                    int value = dataList.Where(p => p.FirstBizName.Equals(item, StringComparison.OrdinalIgnoreCase)).Sum(q => q.Count);
                    data.links.Add(new AccessBizGlobalLinksDto() { source = "全部",target = item ,value = value });
                }

                foreach (var item in secondBizNameList)
                {
                    int value = dataList.Where(p => p.SecondBizName.Equals(item, StringComparison.OrdinalIgnoreCase)).Sum(q => q.Count);
                    data.links.Add(new AccessBizGlobalLinksDto() { source = dataList.First(p => p.SecondBizName == item).FirstBizName, target = item, value = value });
                }

                //foreach (var item in bizNameList)
                //{
                //    int value = dataList.Where(p => p.BizName.Equals(item, StringComparison.OrdinalIgnoreCase)).Sum(q => q.Count);
                //    data.links.Add(new AccessBizGlobalLinksDto() { source = dataList.First(p => p.BizName == item).SecondBizName, target = item, value = value });
                //}

                return Json(new { Success = true, Data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        //public ActionResult GetAccessBizList()
        //{
        //    var rsp = HomeApiClient.GetInstance().GetAccessBizList();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        var data = rsp.ResponseResult.GroupBy(p => p.FirstBizName).Select(p => new { Name = p.Key, Count = p.Sum(q => q.Count) });

        //        return Json(new { Success = true, Data = data }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetAccessBizListByPage(AccessBizGlobalQuery query)
        //{
        //    var rsp = HomeApiClient.GetInstance().GetAccessBizList();
        //    if (rsp.Success && rsp.ResponseResult != null)
        //    {
        //        var dataList = rsp.ResponseResult.OrderByDescending(p => p.Count);
        //        return Json(new
        //        {
        //            aaData = dataList,
        //            iTotalDisplayRecords = rsp.ResponseResult.Count,
        //            iTotalRecords = rsp.ResponseResult.Count,
        //            //rsp.ResponseResult.TableResuls.sEcho

        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}
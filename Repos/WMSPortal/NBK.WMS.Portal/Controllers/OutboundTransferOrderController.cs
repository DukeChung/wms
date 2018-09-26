using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;


namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class OutboundTransferOrderController : BaseController
    {
        /// <summary>
        /// 交接管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.PrintSettingCase = PublicConst.PrintSettingCase;
            return View();
        }

        /// <summary>
        /// 获取交接管理数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetOutboundTransferOrderByPage(OutboundTransferOrderQuery request)
        {

            var rsp = OrderManegerApiClient.GetInstance().GetOutboundTransferOrderByPage(request, this.LoginCoreQuery);
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

        /// <summary>
        /// 交界明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult OutboundTransferOrderView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OrderManegerApiClient.GetInstance().GetDataBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new OutboundTransferOrderDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        public ActionResult GetOutboundTransferBox(string sysIds)
        {
            var rsp = OrderManegerApiClient.GetInstance().GetOutboundTransferBox(sysIds.ToGuidList(), CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            return Json(new { Success = rsp.Success, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult UpdateTransferOrder(Guid sysId)
        {
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OrderManegerApiClient.GetInstance().GetDataBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new OutboundTransferOrderDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        /// <summary>
        /// 获取出库单所有交接单号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult GetOutboundTransferOrder(OutboundTransferOrderQuery dto)
        {
            var rsp = OrderManegerApiClient.GetInstance().GetOutboundTransferOrder(dto, this.LoginCoreQuery);
            return Json(new { Success = rsp.Success, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateTransferOrderSku(OutboundTransferOrderMoveDto dto)
        {
            var rsp = OrderManegerApiClient.GetInstance().UpdateTransferOrderSku(dto, this.LoginCoreQuery);
            return Json(new { Success = rsp.Success, Message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}
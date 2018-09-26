using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    public class TransferinventoryController : BaseController
    {
        public ActionResult Index()
        {
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                ViewBag.WarehouseList = rsp.ResponseResult;
            }
            return View();
        }


        [SetUserInfo]
        public ActionResult GetTransferinventoryByPage(TransferinventoryQuery request)
        {
            if (!string.IsNullOrEmpty(request.TransferInventoryOrder))
                request.TransferInventoryOrder.Replace('，', ',');
            if (request.TransferOutboundDateTo.HasValue)
                request.TransferOutboundDateTo = request.TransferOutboundDateTo.Value.AddDays(1);
            if (request.TransferInboundDateTo.HasValue)
                request.TransferInboundDateTo = request.TransferInboundDateTo.Value.AddDays(1);

            var rsp = OrderManegerApiClient.GetInstance().GetTransferinventoryByPage(request);
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
        /// 移仓单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult TransferinventoryView(Guid sysId)
        {
            var response = OrderManegerApiClient.GetInstance().GetTransferinventoryBySysId(sysId, CurrentUser.WarehouseSysId);
            var model = new TransferInventoryViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            };
            return View(model);
        }

        /// <summary>
        /// 作废出移仓单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult ObsoleteTransferinventory(TransferinventoryUpdateQuery dto)
        {
            dto.CurrentDisplayName = CurrentUser.UserName;
            dto.CurrentUserId = CurrentUser.UserId;
            dto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OrderManegerApiClient.GetInstance().ObsoleteTransferinventory(dto);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
    }

}
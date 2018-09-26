using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class FrozenController : BaseController
    {
        public ActionResult FrozenRequest()
        {
            var rsp = BaseDataApiClient.GetInstance().SelectZone(CurrentUser.WarehouseSysId);
            var zoneList = new List<SelectItem>();
            if (rsp.Success)
            {
                zoneList = rsp.ResponseResult;
            }
            ViewBag.ZoneList = zoneList;

            rsp = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId);
            var locList = new List<SelectItem>();
            if (rsp.Success && rsp.ResponseResult != null)
            {
                locList = rsp.ResponseResult;
            }
            ViewBag.LocationList = locList;

            return View();
        }

        public ActionResult GetSelectLocationList(string zoneSysId)
        {
            var rsp = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId, zoneSysId);

            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFrozenRequestSkuByPage(FrozenRequestQuery request)
        {
            if (request.Loc == "请选择")
                request.Loc = null;
            var rsp = InventoryApiClient.GetInstance().GetFrozenRequestSkuByPage(request,LoginCoreQuery);
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

        public ActionResult SaveFrozenRequest(FrozenRequestDto request)
        {
            if (request.Type == (int)FrozenType.Sku)
            {
                request.SkuList = request.skus.ToGuidList();
            }
            var rsp = InventoryApiClient.GetInstance().SaveFrozenRequest(request, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FrozenMaintain()
        {
            var rsp = BaseDataApiClient.GetInstance().SelectZone(CurrentUser.WarehouseSysId);
            var zoneList = new List<SelectItem>();
            if (rsp.Success)
            {
                zoneList = rsp.ResponseResult;
            }
            ViewBag.ZoneList = zoneList;

            return View();
        }

        public ActionResult GetFrozenRequestList(FrozenListQuery request)
        {
            var rsp = InventoryApiClient.GetInstance().GetFrozenRequestList(request, LoginCoreQuery);
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

        public ActionResult UnFrozenRequest(FrozenRequestDto request)
        {
            var rsp = InventoryApiClient.GetInstance().UnFrozenRequest(request, LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FrozenDetail(Guid sysId)
        {
            ViewBag.StockFrozenSysId = sysId;
            return View();
        }

        public ActionResult GetFrozenDetailList(FrozenRequestQuery request)
        {
            var rsp = InventoryApiClient.GetInstance().GetFrozenDetailByPage(request, LoginCoreQuery);
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
    }
}
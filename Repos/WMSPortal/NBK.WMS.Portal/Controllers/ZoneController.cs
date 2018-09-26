using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class ZoneController : BaseController
    {
        [PermissionAuthorize("BaseData_Zone")]
        public ActionResult ZoneList()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult GetZoneList(ZoneQuery zoneQuery)
        {
            zoneQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().GetZoneList(LoginCoreQuery, zoneQuery);
            if (rsp.Success && rsp.ResponseResult.TableResuls != null)
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

        public ActionResult AddZone()
        {
            return View();
        }

        public ActionResult SaveAddZone(ZoneDto zoneDto)
        {
            zoneDto.CreateBy = CurrentUser.UserId;
            zoneDto.UpdateBy = CurrentUser.UserId;
            zoneDto.CreateUserName = CurrentUser.DisplayName;
            zoneDto.UpdateUserName = CurrentUser.DisplayName;
            zoneDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().AddZone(LoginCoreQuery, zoneDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditZone(string sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetZoneById(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return View(rsp.ResponseResult);
            }
            return View();
        }

        public ActionResult SaveEditZone(ZoneDto zoneDto)
        {
            zoneDto.UpdateBy = CurrentUser.UserId;
            zoneDto.UpdateUserName = CurrentUser.DisplayName;
            zoneDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().EditZone(LoginCoreQuery, zoneDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteZone(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteZone(LoginCoreQuery, sysIds.ToGuidList(), CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        private void InitializeViewBag()
        {
            List<SelectListItem> isActiveList = new List<SelectListItem>();
            isActiveList.Add(new SelectListItem { Text = "请选择" });
            isActiveList.Add(new SelectListItem { Text = "是", Value = "true" });
            isActiveList.Add(new SelectListItem { Text = "否", Value = "false" });
            ViewBag.IsActiveList = isActiveList;
        }

        public ActionResult GetSelectZoneList()
        {
            var rsp = BaseDataApiClient.GetInstance().SelectZone(CurrentUser.WarehouseSysId , null);

            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}
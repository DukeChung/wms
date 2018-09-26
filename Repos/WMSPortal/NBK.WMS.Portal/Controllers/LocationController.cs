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
    public class LocationController : BaseController
    {
        [PermissionAuthorize("BaseData_Location")]
        public ActionResult LocationList()
        {
            ViewBag.PrintName = PublicConst.PrintSettingUPC;
            InitializeViewBag();
            InitializeViewBag(true);
            return View();
        }

        public ActionResult GetLocationList(LocationQuery locationQuery)
        {
            locationQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().GetLocationList(LoginCoreQuery, locationQuery);
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

        public ActionResult AddLocation()
        {
            InitializeViewBag(false);
            return View();
        }

        public ActionResult SaveAddLocation(LocationDto locationDto)
        {
            locationDto.CreateBy = CurrentUser.UserId;
            locationDto.UpdateBy = CurrentUser.UserId;
            locationDto.CreateUserName = CurrentUser.DisplayName;
            locationDto.UpdateUserName = CurrentUser.DisplayName;
            locationDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().AddLocation(LoginCoreQuery, locationDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditLocation(string sysId)
        {
            InitializeViewBag(false);
            var rsp = BaseDataApiClient.GetInstance().GetLocationById(LoginCoreQuery, sysId,CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return View(rsp.ResponseResult);
            }
            return View();
        }

        public ActionResult SaveEditLocation(LocationDto locationDto)
        {
            locationDto.UpdateBy = CurrentUser.UserId;
            locationDto.UpdateUserName = CurrentUser.DisplayName;
            locationDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().EditLocation(LoginCoreQuery, locationDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteLocation(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteLocation(LoginCoreQuery, sysIds.ToGuidList(),CurrentUser.WarehouseSysId);
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

        private void InitializeViewBag(bool hasOptionLabel)
        {
            List<SelectListItem> zoneList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                zoneList.Add(new SelectListItem());
            }
            ZoneQuery zoneQuery = new ZoneQuery { iDisplayStart = 0, iDisplayLength = 500, mDataProp_1 = "ZoneCode", IsActiveSearch = true, WarehouseSysId = CurrentUser.WarehouseSysId };
            var rsp1 = BaseDataApiClient.GetInstance().GetZoneList(LoginCoreQuery, zoneQuery);
            if (rsp1.Success && rsp1.ResponseResult.TableResuls != null)
            {
                zoneList.AddRange(rsp1.ResponseResult.TableResuls.aaData.Select(p => new SelectListItem
                {
                    Text = p.ZoneCode,
                    Value = p.SysId.ToString()
                }));
            }
            ViewBag.ZoneList = zoneList;

            List<SelectListItem> locUsageList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                locUsageList.Add(new SelectListItem());
            }
            var rsp2 = BaseDataApiClient.GetInstance().SelectItemSysCode("LocUsage");
            if (rsp2.Success && rsp2.ResponseResult != null)
            {
                locUsageList.AddRange(rsp2.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LocUsageList = locUsageList;

            List<SelectListItem> locCategoryList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                locCategoryList.Add(new SelectListItem());
            }
            var rsp3 = BaseDataApiClient.GetInstance().SelectItemSysCode("LocCategory");
            if (rsp3.Success && rsp3.ResponseResult != null)
            {
                locCategoryList.AddRange(rsp3.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LocCategoryList = locCategoryList;
        }
        
    }
}
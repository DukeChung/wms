using NBK.AuthServiceUtil;
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
    public class WorkUserController : BaseController
    {
        //[PermissionAuthorize("BaseData_Zone")]
        public ActionResult WorkUserList()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult GetWorkUserList(WorkUserQuery workUserQuery)
        {
            workUserQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().GetWorkUserList(LoginCoreQuery, workUserQuery);
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

        public ActionResult AddWorkUser()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult SaveAddWorkUser(WorkUserDto workUserDto)
        {
            workUserDto.CreateBy = CurrentUser.UserId;
            workUserDto.CreateUserName = CurrentUser.DisplayName;
            workUserDto.UpdateBy = CurrentUser.UserId;
            workUserDto.UpdateUserName = CurrentUser.DisplayName;
            workUserDto.WorkStatus = (int)UserWorkStatus.Free;
            var rsp = BaseDataApiClient.GetInstance().AddWorkUser(LoginCoreQuery, workUserDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditWorkUser(Guid sysId)
        {
            ViewBag.SysId = sysId;
            InitializeViewBag();
            return View();
        }

        public ActionResult GetWorkUserById(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetWorkUserById(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEditWorkUser(WorkUserDto workUserDto)
        {
            workUserDto.CreateBy = CurrentUser.UserId;
            workUserDto.CreateUserName = CurrentUser.DisplayName;
            workUserDto.UpdateBy = CurrentUser.UserId;
            workUserDto.UpdateUserName = CurrentUser.DisplayName;
            var rsp = BaseDataApiClient.GetInstance().EditWorkUser(LoginCoreQuery, workUserDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteWorkUser(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteWorkUser(LoginCoreQuery, sysIds.ToGuidList(), CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        private void InitializeViewBag()
        {
            List<SelectListItem> isActiveList = new List<SelectListItem>();
            isActiveList.Add(new SelectListItem { Text = "是", Value = "true" });
            isActiveList.Add(new SelectListItem { Text = "否", Value = "false" });
            ViewBag.IsActiveList = isActiveList;

            List<SelectListItem> workTypeList = new List<SelectListItem>();
            workTypeList.Add(new SelectListItem { Text = UserWorkType.Receipt.ToDescription(), Value = ((int)UserWorkType.Receipt).ToString() });
            workTypeList.Add(new SelectListItem { Text = UserWorkType.Shelve.ToDescription(), Value = ((int)UserWorkType.Shelve).ToString() });
            workTypeList.Add(new SelectListItem { Text = UserWorkType.Picking.ToDescription(), Value = ((int)UserWorkType.Picking).ToString() });
            ViewBag.WorkTypeList = workTypeList;

            List<SelectListItem> workStatusList = new List<SelectListItem>();
            workStatusList.Add(new SelectListItem { Text = UserWorkStatus.Working.ToDescription(), Value = ((int)UserWorkStatus.Working).ToString() });
            workStatusList.Add(new SelectListItem { Text = UserWorkStatus.Free.ToDescription(), Value = ((int)UserWorkStatus.Free).ToString() });
            ViewBag.WorkStatusList = workStatusList;
        }
    }
}
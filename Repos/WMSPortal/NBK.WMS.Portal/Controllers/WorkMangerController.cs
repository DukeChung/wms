using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;
using NBK.WMS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.AuthServiceUtil;

namespace NBK.WMS.Portal.Controllers
{
    public class WorkMangerController : BaseController
    {
        // GET: Work
        public ActionResult Index()
        {
            return View();
        }
        [SetUserInfo]
        public ActionResult GetWorkByPage(WorkQueryDto request)
        {
            var rsp = VASApiClient.GetInstance().GetWorkByPage(request);
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
        /// 查看
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult WorkView(Guid sysId)
        {
            var response = VASApiClient.GetInstance().GetWorkBySysId(sysId, CurrentUser.WarehouseSysId);
            var model = new WorkDetailDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            return View(model);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult UpdateWork(Guid sysId)
        {
            List<SelectListItem> selectUserList = new List<SelectListItem>();
            var workUserDto = new WorkUserQuery();
            workUserDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().GetWorkUsers(LoginCoreQuery, workUserDto);
            if (rsp.Success)
            {
                rsp.ResponseResult.ForEach(p =>
                {
                    selectUserList.Add(new SelectListItem() { Text = p.WorkUserName, Value = p.SysId.ToString() });
                });
            }
            ViewBag.UserList = selectUserList;
            var response = VASApiClient.GetInstance().GetWorkBySysId(sysId, CurrentUser.WarehouseSysId);
            var model = new WorkDetailDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult UpdateWorkInfo(WorkUpdateDto request)
        {
            request.CurrentUserId = CurrentUser.UserId;
            request.CurrentDisplayName = CurrentUser.DisplayName;
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = VASApiClient.GetInstance().UpdateWorkInfo(request, this.LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "更新成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        public ActionResult CancelWork(string sysIds)
        {
            CancelWorkDto dto = new CancelWorkDto
            {
                SysIds = sysIds.ToGuidList(),
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId
            };
            var rsp = VASApiClient.GetInstance().CancelWork(LoginCoreQuery, dto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

    }
}
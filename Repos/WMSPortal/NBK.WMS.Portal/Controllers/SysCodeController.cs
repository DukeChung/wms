using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class SysCodeController : BaseController
    {
        public SysCodeController()
        {

        }

        // GET: SysCode
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSysCodeDtoList(SysCodeQuery sysCodeQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetSysCodeDtoList(sysCodeQuery);
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
        /// 获取系统代码明细
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSysCodeDetailDtoList(Guid sysCodeSysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetSysCodeDetailDtoList(sysCodeSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    aaData = rsp.ResponseResult
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditSysCode(Guid sysId)
        {
            ViewBag.SysId = sysId;
            return View("Edit"); 
        }

        /// <summary>
        /// 初始化SysCodeDetail 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult InitSysCodeDetail(Guid sysId, Guid? sysCodeSysDetailSysId)
        {
            ViewBag.SysCodeSysId = sysId;
            var sysCodeDetailDto = new SysCodeDetailDto();
            if (sysCodeSysDetailSysId.HasValue)
            {
                var rsp = BaseDataApiClient.GetInstance().GetSysCodeDetailDtoById(sysCodeSysDetailSysId.Value);
                if (rsp.Success)
                {
                    sysCodeDetailDto = rsp.ResponseResult;
                }
                else
                {
                    //异常
                }
            }

            return PartialView("_SysCodeDetail", sysCodeDetailDto);
        }

        /// <summary>
        /// 初始化SysCodeDetail 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteSysCodeDetail(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteSysCodeDetailByIdList(sysIds.ToGuidList());
            if (rsp.Success)
            {
                return Json(new { success = true, message = "删除成功!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据SysId获取相关数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetSysCodeBySysId(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetSysCodeDtoById(sysId);
            if (rsp.Success)
            {
                return Json(new {success = true, SysCodeDto = rsp.ResponseResult}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 保存SysCode
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveSysCode(SysCodeDto sysCodeDto)
        {
            var rsp = BaseDataApiClient.GetInstance().SaveSysCode(sysCodeDto);
            if (rsp.Success)
            {
                return Json(new { success = true, SysCodeDto = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存SysCodeDetail
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public ActionResult UpdateSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        { 
            sysCodeDetailDto.UpdateBy = CurrentUser.UserId;
            sysCodeDetailDto.UpdateUserName = CurrentUser.DisplayName; 
            var rsp = BaseDataApiClient.GetInstance().UpdateSysCodeDetail(sysCodeDetailDto);
            if (rsp.Success)
            {
                return Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {success = false, message = rsp.ApiMessage.ErrorMessage}, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增SysCodeDetail
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        /// <returns></returns>
        public ActionResult InsertSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            sysCodeDetailDto.CreateBy = CurrentUser.UserId;
            sysCodeDetailDto.UpdateBy = CurrentUser.UserId;
            sysCodeDetailDto.UpdateUserName = CurrentUser.DisplayName;
            sysCodeDetailDto.CreateUserName = CurrentUser.DisplayName;
            var rsp = BaseDataApiClient.GetInstance().InsertSysCodeDetail(sysCodeDetailDto);
            if (rsp.Success)
            {
                return Json(new { success = true, SysCodeDto = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
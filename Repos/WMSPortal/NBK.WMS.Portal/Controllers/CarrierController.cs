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
    public class CarrierController : BaseController
    {
        [PermissionAuthorize("BaseData_Carrier")]
        public ActionResult CarrierList()
        {
            InitializeViewBag();
            return View();
        }

        public ActionResult GetCarrierList(CarrierQuery carrierQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetCarrierList(LoginCoreQuery, carrierQuery);
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

        public ActionResult AddCarrier()
        {
            return View();
        }

        public ActionResult SaveAddCarrier(CarrierDto carrierDto)
        {
            carrierDto.CreateBy = CurrentUser.UserId;
            carrierDto.UpdateBy = CurrentUser.UserId;
            carrierDto.CreateUserName = CurrentUser.DisplayName;
            carrierDto.UpdateUserName = CurrentUser.DisplayName;
            var rsp = BaseDataApiClient.GetInstance().AddCarrier(LoginCoreQuery, carrierDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditCarrier(string sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetCarrierById(LoginCoreQuery, sysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return View(rsp.ResponseResult);
            }
            return View();
        }

        public ActionResult SaveEditCarrier(CarrierDto carrierDto)
        {
            carrierDto.UpdateBy = CurrentUser.UserId;
            var rsp = BaseDataApiClient.GetInstance().EditCarrier(LoginCoreQuery, carrierDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCarrier(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteCarrier(LoginCoreQuery, sysIds.ToGuidList());
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
    }
}
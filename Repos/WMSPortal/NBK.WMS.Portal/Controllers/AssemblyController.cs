using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.WMS.Portal.Services;
using FortuneLab.WebApiClient;
using NBK.ECService.WMS.DTO;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class AssemblyController : BaseController
    {
        public ActionResult AssemblyList()
        {
            return View();
        }

        public ActionResult GetAssemblyStatusList()
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem { Text = "请选择", Value = null });
            statusList.Add(new SelectListItem { Text = AssemblyStatus.New.ToDescription(), Value = ((int)AssemblyStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = AssemblyStatus.Assembling.ToDescription(), Value = ((int)AssemblyStatus.Assembling).ToString() });
            statusList.Add(new SelectListItem { Text = AssemblyStatus.Finished.ToDescription(), Value = ((int)AssemblyStatus.Finished).ToString() });
            //statusList.Add(new SelectListItem { Text = AssemblyOrderStatus.Voided.ToDescription(), Value = ((int)AssemblyOrderStatus.Voided).ToString() });
            return Json(new { Success = true, Data = statusList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssemblyList(AssemblyQuery assemblyQuery)
        {
            var rsp = VASApiClient.GetInstance().GetAssemblyList(LoginCoreQuery, assemblyQuery);
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

        public ActionResult AssemblyView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.SysId = sysId;
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        public ActionResult GetAssemblyViewDtoById(Guid sysId)
        {
            var rsp = VASApiClient.GetInstance().GetAssemblyViewDtoById(LoginCoreQuery, sysId,CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintAssemblyRCMDPickDetail(Guid sysId)
        {
            var rsp = VASApiClient.GetInstance().UpdateAssemblyStatus(LoginCoreQuery, sysId, AssemblyStatus.Assembling, CurrentUser.UserId, CurrentUser.UserName,CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CancelAssemblyPicking(Guid sysId)
        {
            var rsp = VASApiClient.GetInstance().CancelAssemblyPicking(LoginCoreQuery, sysId, CurrentUser.UserId, CurrentUser.UserName, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "撤销领料成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FinishAssemblyOrder(AssemblyFinishDto assemblyFinishDto)
        {
            var rsp = VASApiClient.GetInstance().FinishAssemblyOrder(LoginCoreQuery, assemblyFinishDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAssembly()
        {
            return View();
        }

        public ActionResult GetSkuListForAssembly(AssemblySkuQuery request)
        {
            var rsp = VASApiClient.GetInstance().GetSkuListForAssembly(request, LoginCoreQuery);
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

        public ActionResult AddAssemblyService(AddAssemblyDto request)
        {
            var rsp = VASApiClient.GetInstance().AddAssembly(request, LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request)
        {
            var rsp = VASApiClient.GetInstance().GetWeighSkuListForAssembly(request, LoginCoreQuery);
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

        public ActionResult SaveAssemblySkuWeight(AssemblyWeightSkuRequest request)
        {
            var rsp = VASApiClient.GetInstance().SaveAssemblySkuWeight(request, LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetInventoryList(RFInventoryQuery request)
        {
            var rsp = VASApiClient.GetInstance().GetInventoryList(LoginCoreQuery, request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LocIsExist(LocationQuery request)
        {
            var rsp = VASApiClient.GetInstance().LocIsExist(LoginCoreQuery, request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                if (rsp.ResponseResult.IsSucess)
                {
                    return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AssemblyScanPickDetail(RFAssemblyPickDetailDto request)
        {
            var rsp = VASApiClient.GetInstance().AssemblyScanPickDetail(LoginCoreQuery, request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                if (rsp.ResponseResult.IsSucess)
                {
                    return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AssemblyScanShelves(RFAssemblyScanShelvesDto request)
        {
            var rsp = VASApiClient.GetInstance().AssemblyScanShelves(LoginCoreQuery, request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                if (rsp.ResponseResult.IsSucess)
                {
                    return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
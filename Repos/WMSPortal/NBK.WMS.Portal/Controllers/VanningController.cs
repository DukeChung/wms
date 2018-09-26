using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using NBK.ECService.WMS.Utility;

namespace NBK.WMS.Portal.Controllers
{
    //[Authorize]
    public class VanningController : BaseController
    {
        // GET: Vanning
        public ActionResult Index()
        {
            return View();
        }

        [SetUserInfo]
        public ActionResult GetVanningList(VanningQueryDto vanningQueryDto)
        {
            var rsp = OutboundApiClient.GetInstance().GetVanningList(vanningQueryDto);
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

        public ActionResult VanningOperation()
        {
            ViewData["PrintUserName"] = CurrentUser.DisplayName;
            ViewBag.PrintNameBoxLable = PublicConst.PrintSetting1018;
            ViewBag.PrintNamePackingDetail = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rspExpress = BaseDataApiClient.GetInstance().GetExpressListByIsActive();
            var rsp = BaseDataApiClient.GetInstance().GetContainerListByIsActive(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                ViewBag.containerList = rsp.ResponseResult;
            }
            if (rspExpress.Success)
            {
                ViewBag.ExpressList = rspExpress.ResponseResult;
            }
            return View();
        }

        /// <summary>
        /// 获取装箱
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ActionResult GetVanningOperationDtoByOrder(string orderNumber)
        {
            var rsp = OutboundApiClient.GetInstance().GetVanningOperationDtoByOrder(orderNumber, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                rsp.ResponseResult.PickDetailSumOperationDto = rsp.ResponseResult.PickDetailSumDto;
                rsp.ResponseResult.UnitQty = 1;
                rsp.ResponseResult.Weight = 0.00m;
                rsp.ResponseResult.ScanTotalSkuCount = 0;
                rsp.ResponseResult.ScanTotalOrderCount = 0;
                rsp.ResponseResult.VanningPickDetailDto = new List<VanningPickDetailDto>();
                rsp.ResponseResult.VanningPickDetailOperationDto = new List<VanningPickDetailOperationDto>();
                rsp.ResponseResult.VanningDetailOperationDto = new List<VanningDetailOperationDto>();
                return Json(new
                {
                    success = true,
                    VanningOperationDto = rsp.ResponseResult

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = rsp.ApiMessage.ErrorMessage

                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SaveOperation(List<VanningDetailOperationDto> vanningDetailOperationDto)
        {
            var actionType = this.Request["actionType"];
            //foreach (var vd in vanningDetailOperationDto)
            //{
            //    vd.CreateBy = CurrentUser.UserId;
            //    vd.CreateUserName = CurrentUser.DisplayName;
            //    vd.UpdateBy = CurrentUser.UserId;
            //    vd.UpdateUserName = CurrentUser.DisplayName;
            //    if (vd.VanningPickDetailDto != null)
            //    {
            //        foreach (var vpd in vd.VanningPickDetailDto)
            //        {
            //            vpd.CreateBy = CurrentUser.UserId;
            //            vpd.CreateUserName = CurrentUser.DisplayName;
            //            vpd.UpdateBy = CurrentUser.UserId;
            //            vpd.UpdateUserName = CurrentUser.DisplayName;
            //        }
            //    }
            //}

            var rsp = OutboundApiClient.GetInstance().SaveVanningDetailOperationDto(vanningDetailOperationDto, actionType, CurrentUser.DisplayName, CurrentUser.UserId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    VanningDetailDto = rsp.ResponseResult,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = rsp.ApiMessage.ErrorMessage

                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HandoverGroupList()
        {
            return View();
        }

        public ActionResult GetHandoverGroupByPage(HandoverGroupQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GetHandoverGroupByPage(request, this.LoginCoreQuery);
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

        public ActionResult HandoverGroupDetail(string handoverGroupOrder)
        {
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OutboundApiClient.GetInstance().GetHandoverGroupByOrder(handoverGroupOrder, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new HandoverGroupDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }

            return View(model);
        }

        public ActionResult VanningView(Guid sysId)
        {
            ViewData["PrintUserName"] = CurrentUser.DisplayName;
            ViewBag.PrintNameBoxLable = PublicConst.PrintSetting1018;
            ViewBag.PrintNamePackingDetail = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ViewBag.VanningSysId = sysId;
            return View();
        }

        public ActionResult GetVanningViewById(VanningViewQuery vanningViewQuery)
        {
            var rsp = OutboundApiClient.GetInstance().GetVanningViewById(LoginCoreQuery, vanningViewQuery);
            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.VanningDetailViewDtoList.TableResuls != null)
            {
                return Json(new
                {
                    rsp.ResponseResult.VanningOrder,
                    rsp.ResponseResult.VanningTypeText,
                    rsp.ResponseResult.StatusText,
                    rsp.ResponseResult.OutboundOrder,
                    rsp.ResponseResult.VanningDateText,
                    rsp.ResponseResult.Status,
                    rsp.ResponseResult.VanningDetailViewDtoList.TableResuls.aaData,
                    rsp.ResponseResult.VanningDetailViewDtoList.TableResuls.iTotalDisplayRecords,
                    rsp.ResponseResult.VanningDetailViewDtoList.TableResuls.iTotalRecords,
                    rsp.ResponseResult.VanningDetailViewDtoList.TableResuls.sEcho

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 交接标签打印
        /// </summary>
        /// <returns></returns>
        public ActionResult HandoverGroupLable()
        {
            return View();
        }

        /// <summary>
        /// 取消装箱
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult CancelVanning(VanningCancelDto vanningCancelDto)
        {
            var rsp = OutboundApiClient.GetInstance().CancelVanning(vanningCancelDto, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { success = true, isasyn = rsp.ResponseResult.IsAsyn, message = rsp.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage == null ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
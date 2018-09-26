using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;

namespace NBK.WMS.Portal.Controllers
{
    //[Authorize]
    public class PickDetailController : BaseController
    {
        // GET: PickDetail
        public ActionResult Index()
        {
            ViewBag.PrintNameOrder = PublicConst.PrintSettingA4;
            ViewBag.PrintNameBatch = PublicConst.PrintSettingA4;
            ViewBag.UserName = CurrentUser.DisplayName;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        /// <summary>
        /// 获取拣货单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPickDetailList(PickDetailQuery pickDetailQuery)
        {
            pickDetailQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GetPickDetailListDto(pickDetailQuery);
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
        /// 获取需要拣货的出库单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPickOutboundList(PickDetailQuery pickDetailQuery)
        {
            if (pickDetailQuery.EndOutboundDateSearch.HasValue)
                pickDetailQuery.EndOutboundDateSearch = pickDetailQuery.EndOutboundDateSearch.Value.AddDays(1);
            pickDetailQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GetPickOutboundListDto(pickDetailQuery);
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
        /// 创建拣货单
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePickDetail()
        {
            ViewBag.PrintNameOrder = PublicConst.PrintSettingA4;
            ViewBag.PrintNameBatch = PublicConst.PrintSettingA4;
            ViewBag.UserName = CurrentUser.DisplayName;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().SelectItemSysCode(PublicConst.SysCodeTypePickRule);
            var carrier = BaseDataApiClient.GetInstance().GetExpressListByIsActive();
            if (carrier.Success)
            {
                ViewBag.CarrList = carrier.ResponseResult;
            }
            return View(rsp.ResponseResult);
        }

        /// <summary>
        /// 取消拣货
        /// </summary>
        /// <param name="pickDetailOrders"></param>
        /// <returns></returns>
        public ActionResult CancelPickDetail(string pickDetailOrders)
        {
            var cancelPickDetailDto = new CancelPickDetailDto()
            {
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId,
                PickDetailOrderList = pickDetailOrders.Split(',').ToList()
            };
            var rsp = OutboundApiClient.GetInstance().CancelPickDetail(cancelPickDetailDto);

            if (rsp.Success)
            {
                return Json(new { success = true, message = "取消拣货完成"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage}, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 根据拣货规则生成拣货明细啥s
        /// </summary>
        /// <param name="createPickDetailRuleDto">生成拣货明细</param>
        /// <returns></returns>
        public ActionResult GeneratePickDetailByPickRule(CreatePickDetailRuleDto createPickDetailRuleDto)
        {
            createPickDetailRuleDto.CurrentUserId = CurrentUser.UserId;
            createPickDetailRuleDto.CurrentDisplayName = CurrentUser.DisplayName;
            createPickDetailRuleDto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GeneratePickDetailByPickRule(createPickDetailRuleDto);
         
            if (rsp.Success)
            {
                return Json(new { success = true, message = "拣货完成,开始打印拣货单!", pickDetailOrders= rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "拣货失败,"+rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 取消拣货数量
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public ActionResult CancelPickQty(Guid sysId, int qty, string storageCase)
        {
            var cancelPickQtyDto = new CancelPickQtyDto()
            {
                SysId = sysId,
                Qty = qty,
                StorageCase = storageCase == null ? null : storageCase.Trim(),
                CurrentUserId = CurrentUser.UserId,
                CurrentDisplayName = CurrentUser.DisplayName,
                WarehouseSysId = CurrentUser.WarehouseSysId
            };
            var rsp = OutboundApiClient.GetInstance().CancelPickQty(cancelPickQtyDto);

            if (rsp.Success)
            {
                return Json(new { success = true, message = "取消拣货完成" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 拣货
        /// </summary>
        /// <param name="pickingOperationDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult SavePickingOperation(PickingOperationDto pickingOperationDto)
        {
            var rsp = OutboundApiClient.GetInstance().SavePickingOperation(pickingOperationDto);

            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "拣货完成" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取拣货单明细
        /// </summary>
        /// <param name="pickingOperationQuery"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetPickingOperationDetails(PickingOperationQuery pickingOperationQuery)
        {
            var rsp = OutboundApiClient.GetInstance().GetPickingOperationDetails(pickingOperationQuery);

            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.Utility;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class OutboundController : BaseController
    {
        public ActionResult OutboundMaintain(string createDateFrom = null, string createDateTo = null)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.CreateDateFrom = createDateFrom;
            ViewBag.CreateDateTo = createDateTo;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                ViewBag.WarehouseList = rsp.ResponseResult;
            }
            return View();
        }

        public ActionResult OutboundFertilizer(string createDateFrom = null, string createDateTo = null)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.CreateDateFrom = createDateFrom;
            ViewBag.CreateDateTo = createDateTo;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                ViewBag.WarehouseList = rsp.ResponseResult;
            }
            return View();
        }

        [SetUserInfo]
        public ActionResult GetOutboundByPage(OutboundQuery request)
        {
            if (!string.IsNullOrEmpty(request.OutboundOrder))
                request.OutboundOrder.Replace('，', ',');
            if (request.CreateDateTo.HasValue)
                request.CreateDateTo = request.CreateDateTo.Value.AddDays(1);
            if (request.AuditingDateTo.HasValue)
                request.AuditingDateTo = request.AuditingDateTo.Value.AddDays(1);
            if (request.ActualShipDateTo.HasValue)
                request.ActualShipDateTo = request.ActualShipDateTo.Value.AddDays(1);
            if (request.Region.Trim() == "请输入")
                request.Region = string.Empty;
            if (request.DepartureDateTo.HasValue)
                request.DepartureDateTo = request.DepartureDateTo.Value.AddDays(1);

            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GetOutboundByPage(request);
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

        public ActionResult OutboundView(Guid sysId)
        {
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.PrintSettingCase = PublicConst.PrintSettingCase;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var response = OutboundApiClient.GetInstance().GetOutboundBySysId(sysId, CurrentUser.WarehouseSysId);
            var model = new OutboundViewDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }
            return View(model);
        }

        public ActionResult BatchOutbound()
        {
            ViewData["OutboundGroup"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            ViewBag.CurrentUserName = CurrentUser.DisplayName;
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        public ActionResult GetSkuList(SkuQuery skuQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetSkuList(LoginCoreQuery, skuQuery);
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

        [SetUserInfo]
        public ActionResult BatchOutboundCreate(OutboundBatchDto request)
        {
            var rsp = OutboundApiClient.GetInstance().BatchOutboundCreate(request, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true, outboundOrder = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 快速发货
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult OutboundQuickDelivery(OutboundQuickDeliveryDto outboundQuickDeliveryDto)
        {
            var rsp = OutboundApiClient.GetInstance().OutboundQuickDelivery(outboundQuickDeliveryDto, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { success = true, isasyn = rsp.ResponseResult.IsAsyn, message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage == null ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 分配发货
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult OutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            var rsp = OutboundApiClient.GetInstance().OutboundAllocationDelivery(outboundAllocationDeliveryDto, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { success = true, isasyn = rsp.ResponseResult.IsAsyn, message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage == null ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 分配发货检查差异
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult CheckOutboundAllocationDelivery(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            var rsp = OutboundApiClient.GetInstance().CheckOutboundAllocationDelivery(LoginCoreQuery, outboundAllocationDeliveryDto);
            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.Success ? rsp.ResponseResult.Message : rsp.ApiMessage.ErrorMessage, ErrorMessage = rsp.Success ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取部分发货商品明细
        /// </summary>
        /// <param name="outboundAllocationDeliveryDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetPartShipmentSkuList(OutboundAllocationDeliveryDto outboundAllocationDeliveryDto)
        {
            var rsp = OutboundApiClient.GetInstance().GetPartShipmentSkuList(LoginCoreQuery, outboundAllocationDeliveryDto);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新出库明细备注
        /// </summary>
        /// <param name="partShipmentMemoDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult SavePartShipmentMemo(PartShipmentMemoDto partShipmentMemoDto)
        {
            var rsp = OutboundApiClient.GetInstance().SavePartShipmentMemo(LoginCoreQuery, partShipmentMemoDto);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 作废出库单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult ObsoleteOutbound(OutboundOperateDto outboundDto)
        {
            var rsp = OutboundApiClient.GetInstance().ObsoleteOutbound(outboundDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        public ActionResult OutboundReturn(OutboundOperateDto outboundDto)
        {
            var rsp = OutboundApiClient.GetInstance().OutboundReturn(outboundDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 部分退货入库
        /// </summary>
        /// <param name="outboundViewDto"></param>
        /// <returns></returns>
        public ActionResult OutboundPartReturn(OutboundPartReturnDto outboundPartReturnDto)
        {
            var rsp = OutboundApiClient.GetInstance().OutboundPartReturn(outboundPartReturnDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消出库
        /// </summary>
        /// <param name="outboundDto"></param>
        /// <returns></returns>
        public ActionResult OutboundCancel(OutboundOperateDto outboundDto)
        {
            var rsp = OutboundApiClient.GetInstance().OutboundCancel(outboundDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取出库单预包装差异
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetOutboundPrePackDiff(Guid outboundSysId)
        {
            var rsp = OutboundApiClient.GetInstance().GetOutboundPrePackDiff(LoginCoreQuery, outboundSysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取出库单散货箱差异
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        public ActionResult GetOutboundPreBulkPackDiff(Guid outboundSysId)
        {
            var rsp = OutboundApiClient.GetInstance().GetOutboundPreBulkPackDiff(LoginCoreQuery, outboundSysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取出库单整件或者散件装箱数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetOutboundBox(Guid outboundSysId)
        {
            var rsp = OutboundApiClient.GetInstance().GetOutboundBox(LoginCoreQuery, outboundSysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult BindPrePack(OutboundBindQuery query)
        {
            var rsp = OutboundApiClient.GetInstance().BindPrePackOrder(query, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult UnBindPrePack(OutboundBindQuery query)
        {
            var rsp = OutboundApiClient.GetInstance().UnBindPrePackOrder(query, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegionListByName(string name)
        {
            var rsp = BaseDataApiClient.GetInstance().GetRegionListByName(LoginCoreQuery, name);
            if (rsp != null && rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult.ToArray() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegionIntactBySysId(Guid regionSysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetRegionIntactBySysId(LoginCoreQuery, regionSysId);
            if (rsp != null && rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        [SetUserInfo]
        public ActionResult CreateBoxNumber(BatchTMSBoxNumberDto dto)
        {
            var rsp = OutboundApiClient.GetInstance().AddTMSBoxNumber(dto, LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { Success = true, Data = rsp.ResponseResult, Message = "生成成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfo]
        public ActionResult CreateTMSBoxCount(BatchTMSBoxNumberDto dto)
        {
            var rsp = OutboundApiClient.GetInstance().CreateTMSBoxCount(dto, LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { Success = true, Message = "生成成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfo]
        public ActionResult GetInsufficientStockSkuList(OutboundAllocationDeliveryDto dto)
        {
            var rsp = OutboundApiClient.GetInstance().GetInsufficientStockSkuList(dto, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(new { success = true, data = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckExistsSN(List<string> snList)
        {
            var rsp = InboundApiClient.GetInstance().CheckDuplicateSN(snList, "Outbound", CurrentUser.WarehouseSysId);
            if (rsp != null && rsp.Success)
            {
                //if (rsp.ResponseResult != null && rsp.ResponseResult.DuplicateList.Count > 0)
                //{
                //    //string errorMessage = string.Join(",", rsp.ResponseResult.ToArray());
                //    return Json(new { success = false, duplicateList = rsp.ResponseResult.DuplicateList }, JsonRequestBehavior.AllowGet);
                //}
                return Json(new { success = true, rsp.ResponseResult.NotExistsList, rsp.ResponseResult.OutboundList, rsp.ResponseResult.NormalList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 异常登记
        /// <summary>
        /// 异常登记
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult ExceptionView(Guid sysId)
        {
            ViewBag.OutboundSysId = sysId;
            return View();
        }

        /// <summary>
        /// 分页获取出库单明细列表数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetOutboundDetailList(OutboundExceptionQueryDto request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().GetOutboundDetailList(request);
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
        /// 添加或更新异常数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult AddOutboundExceptionService(AddOutboundExceptionDto dto)
        {
            dto.CurrentUserId = CurrentUser.UserId;
            dto.CurrentDisplayName = CurrentUser.DisplayName;
            dto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OutboundApiClient.GetInstance().AddOutboundExceptionService(dto, this.LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { Success = true, Message = rsp.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = rsp.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据出库单ID获取异常记录
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetOutbooundExceptionData(Guid sysId)
        {
            var response = OutboundApiClient.GetInstance().GetOutbooundExceptionData(sysId, CurrentUser.WarehouseSysId);
            if (response.Success)
            {
                return Json(new { Success = true, Data = response.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteOutboundException(string sysIdList, Guid sysId)
        {
            var list = sysIdList.ToGuidList();
            if (list != null && list.Count > 0)
            {
                var rsp = OutboundApiClient.GetInstance().DeleteOutboundException(list, CurrentUser.WarehouseSysId, sysId, this.LoginCoreQuery);
                return Json(new { Success = rsp.ResponseResult.IsSuccess, Message = rsp.ResponseResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
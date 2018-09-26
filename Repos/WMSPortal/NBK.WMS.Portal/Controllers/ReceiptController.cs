using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Services;
using NBK.ECService.WMS.Utility;
using NBK.AuthServiceUtil;
using Newtonsoft.Json;

namespace NBK.WMS.Portal.Controllers
{
    //[Authorize]
    public class ReceiptController : BaseController
    {
        // GET: Receipt
        [PermissionAuthorize("Inbound_Receipt")]
        public ActionResult ReceiptList(string receiptDateFrom = null, string receiptDateTo = null)
        {
            InitializeViewBag(true);
            ViewBag.ReceiptDateFrom = receiptDateFrom;
            ViewBag.ReceiptDateTo = receiptDateTo;
            return View();
        }

        public ActionResult GetReceiptList(ReceiptQuery receiptQuery)
        {
            if (receiptQuery.ReceiptDateToSearch.HasValue)
                receiptQuery.ReceiptDateToSearch = receiptQuery.ReceiptDateToSearch.Value.AddDays(1);
            receiptQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InboundApiClient.GetInstance().GetReceiptList(LoginCoreQuery, receiptQuery);
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

        public ActionResult ReceiptView(string sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.PrintCaseName = PublicConst.PrintSettingCase;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ReceiptViewDto model = null;
            var rsp = InboundApiClient.GetInstance().GetReceiptViewById(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        /// <summary>
        /// 收货清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetReceiptDetailViewList(string sysId)
        {
            var rsp = InboundApiClient.GetInstance().GetReceiptDetailViewList(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        /// <summary>
        /// 收货批次清单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult GetReceiptDetailLotViewList(string sysId)
        {
            var rsp = InboundApiClient.GetInstance().GetReceiptDetailLotViewList(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        /// <summary>
        /// 原材料收货
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptMaterialOperation()
        {
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var receiptOperationDto = TempData["ReceiptOperationDto"] as ReceiptOperationDto;
            return View(receiptOperationDto);
        }


        /// <summary>
        /// 农资化肥收货
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptFertilizerOperation()
        {
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var receiptOperationDto = TempData["ReceiptOperationDto"] as ReceiptOperationDto;
            return View(receiptOperationDto);
        }

        /// <summary>
        /// 入库作业
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptOperation()
        {
            ViewBag.PrintName = PublicConst.PrintSettingUPC;
            ViewBag.PrintSettingZS = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        /// <summary>
        /// 根据入库单号获取入库作业
        /// </summary>
        /// <returns></returns>
        public ActionResult GetReceiptOperationByOrderNumber(string orderNumber)
        {
            var rsp = InboundApiClient.GetInstance().GetReceiptOperationByOrderNumber(orderNumber, CurrentUser.DisplayName, CurrentUser.UserId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                //初始化页面部分数据
                rsp.ResponseResult.LotTemplateValueDtos = new List<LotTemplateValueDto>();
                rsp.ResponseResult.UnitQty = 1;
                rsp.ResponseResult.SkuCurrentDto = new SkuCurrentDto();

                var IsMaterial = rsp.ResponseResult.PurchaseDetailViewDto.Where(x => x.IsMaterial != null && x.IsMaterial == true);
                if (rsp.ResponseResult.PurcharType == (int)PurchaseType.Material)
                {
                    TempData["ReceiptOperationDto"] = rsp.ResponseResult;
                    return Json(new
                    {
                        success = true,
                        PurcharType = (int)PurchaseType.Material

                    }, JsonRequestBehavior.AllowGet);
                }
                else if (rsp.ResponseResult.PurcharType == (int)PurchaseType.Fertilizer)
                {
                    TempData["ReceiptOperationDto"] = rsp.ResponseResult;
                    return Json(new
                    {
                        success = true,
                        PurcharType = (int)PurchaseType.Fertilizer

                    }, JsonRequestBehavior.AllowGet);
                }
                else if (IsMaterial.Any())
                {
                    TempData["ReceiptOperationDto"] = rsp.ResponseResult;
                    return Json(new
                    {
                        success = true,
                        PurcharType = (int)PurchaseType.Material

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        ReceiptOperationDto = rsp.ResponseResult

                    }, JsonRequestBehavior.AllowGet);
                }

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

        /// <summary>
        /// 保存收货数据
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        public ActionResult SaveReceiptOperation(string dataStr)
        {
            ReceiptOperationDto receiptOperationDto = JsonConvert.DeserializeObject<ReceiptOperationDto>(dataStr);
            receiptOperationDto.CurrentDisplayName = CurrentUser.DisplayName;
            receiptOperationDto.CurrentUserId = CurrentUser.UserId;
            var rsp = InboundApiClient.GetInstance().SaveReceiptOperation(receiptOperationDto);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,

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

        /// <summary>
        /// 保存原材料收货数据
        /// </summary>
        /// <param name="receiptOperationDto"></param>
        /// <returns></returns>
        public ActionResult SaveReceiptOperations(ReceiptOperationDto receiptOperationDto)
        {
            receiptOperationDto.CurrentDisplayName = CurrentUser.DisplayName;
            receiptOperationDto.CurrentUserId = CurrentUser.UserId;
            var rsp = InboundApiClient.GetInstance().SaveReceiptOperation(receiptOperationDto);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,

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

        /// <summary>
        /// 根据采购订单号获取入库单信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateReceipt(string orderNumber)
        {
            ViewBag.PrintName = PublicConst.PrintSettingZS;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InboundApiClient.GetInstance().CreateReceiptByPoOrder(orderNumber, CurrentUser.DisplayName, CurrentUser.UserId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return View("ReceiptPrint", rsp.ResponseResult);
            }
            else
            {
                //跳转异常页面
                return null;
            }
        }

        /// <summary>
        /// 打印收货单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="receiptOrder"></param>
        /// <returns></returns>
        public ActionResult PrintReceipt(Guid sysId, string receiptOrder)
        {
            var rsp = InboundApiClient.GetInstance().UpdateReceiptStatus(sysId, ReceiptStatus.Print, CurrentUser.DisplayName, CurrentUser.UserId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    message = receiptOrder + "收货单等待收货",

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = receiptOrder + "收货单出现异常,请确认收货单状态",

                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取Receipt SkuUPC 是空的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPurchaseDetailSkuUPCIsNull(Guid purchaseSysId)
        {
            var rsp = InboundApiClient.GetInstance().GetPurchaseDetailSkuByUpcIsNull(purchaseSysId, CurrentUser.WarehouseSysId);
            return Json(new
            {
                success = true,
                PurchaseDetailSkuDto = rsp.ResponseResult

            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存Sku属性
        /// </summary>
        /// <returns></returns>
        public ActionResult SavePurchaseDetailSkuStyle(PurchaseDetailSkuDto purchaseDetailSkuDto)
        {
            var rsp = InboundApiClient.GetInstance().SavePurchaseDetailSkuStyle(purchaseDetailSkuDto);
            if (rsp != null && rsp.Success)
            {
                return Json(new
                {
                    success = true,
                    PurchaseDetailSkuDto = rsp.ResponseResult

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

        private void InitializeViewBag(bool hasOptionLabel)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                statusList.Add(new SelectListItem { Text = "请选择" });
            }
            //statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.Init), Value = ((int)ReceiptStatus.Init).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.New), Value = ((int)ReceiptStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.Print), Value = ((int)ReceiptStatus.Print).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.Receiving), Value = ((int)ReceiptStatus.Receiving).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.Received), Value = ((int)ReceiptStatus.Received).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Receipt((int)ReceiptStatus.Cancel), Value = ((int)ReceiptStatus.Cancel).ToString() });
            ViewBag.StatusList = statusList;

            List<SelectListItem> shelvesStatusList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                shelvesStatusList.Add(new SelectListItem { Text = "请选择" });
            }
            shelvesStatusList.Add(new SelectListItem { Text = ShelvesStatus.NotOnShelves.ToDescription(), Value = ((int)ShelvesStatus.NotOnShelves).ToString() });
            shelvesStatusList.Add(new SelectListItem { Text = ShelvesStatus.Shelves.ToDescription(), Value = ((int)ShelvesStatus.Shelves).ToString() });
            shelvesStatusList.Add(new SelectListItem { Text = ShelvesStatus.Finish.ToDescription(), Value = ((int)ShelvesStatus.Finish).ToString() });
            ViewBag.ShelvesStatusList = shelvesStatusList;
        }

        #region 上架
        /// <summary>
        /// 检查商品是否存在于收货明细中
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult CheckReceiptDetailSku(ScanShelvesDto request)
        {
            var rsp = InboundApiClient.GetInstance().CheckReceiptDetailSku(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult ScanShelves(ScanShelvesDto request)
        {
            var rsp = InboundApiClient.GetInstance().ScanShelves(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 自动上架
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult AutoShelves(ScanShelvesDto scanShelvesDto)
        {
            var rsp = InboundApiClient.GetInstance().AutoShelves(scanShelvesDto);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        /// <summary>
        /// 获取推荐货位
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetAdviceToLoc(RFShelvesQuery request)
        {
            var rsp = InboundApiClient.GetInstance().GetAdviceToLoc(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 判断货位是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult LocIsExist(LocationQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = BaseDataApiClient.GetInstance().LocIsExist(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetInventoryList(RFInventoryQuery request)
        {
            var rsp = InboundApiClient.GetInstance().GetInventoryList(request);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 取消上架
        /// </summary>
        /// <param name="cancelShelvesDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult CancelShelves(CancelShelvesDto cancelShelvesDto)
        {
            var rsp = InboundApiClient.GetInstance().CancelShelves(LoginCoreQuery, cancelShelvesDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 领料分拣
        public ActionResult PickingMaterial(string sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            ReceiptViewDto model = null;
            var rsp = InboundApiClient.GetInstance().GetReceiptViewById(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                model = rsp.ResponseResult;
            }
            return View(model);
        }

        /// <summary>
        /// 领料分拣
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult SavePickingMaterial(PickingMaterialDto picking)
        {
            var rsp = InboundApiClient.GetInstance().PickingMaterial(picking, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(new { success = true, isasyn = rsp.ResponseResult.IsAsyn, message = rsp.ResponseResult.Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage == null ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PrintPickingMaterialList(string sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.ReceiptSysId = sysId;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            return View();
        }

        public ActionResult GetPickingMaterialList(PickingMaterialQuery pickingMaterialQuery)
        {
            var rsp = InboundApiClient.GetInstance().GetPickingMaterialList(pickingMaterialQuery, LoginCoreQuery);
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
        #endregion

        public ActionResult CheckDuplicateSN(List<string> snList)
        {
            var rsp = InboundApiClient.GetInstance().CheckDuplicateSN(snList, "Receive", CurrentUser.WarehouseSysId);
            if (rsp != null && rsp.Success)
            {
                if (rsp.ResponseResult.DuplicateList != null && rsp.ResponseResult.DuplicateList.Count > 0)
                {
                    //string errorMessage = string.Join(",", rsp.ResponseResult.ToArray());
                    return Json(new { success = false, duplicateList = rsp.ResponseResult.DuplicateList }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 批次采集
        public ActionResult ReceiptDetailCollectionLot(string sysId, string receiptOrder)
        {
            ViewBag.PrintName = PublicConst.PrintSettingCase;
            ViewBag.ReceiptSysId = sysId;
            ViewBag.ReceiptOrder = receiptOrder;
            return View();
        }

        /// <summary>
        /// 批次采集时获取收货清单明细
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        public ActionResult GetReceiptDetailViewListByCollectionLot(string sysId)
        {
            var rsp = InboundApiClient.GetInstance().GetReceiptDetailViewListByCollectionLot(LoginCoreQuery, sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        /// <summary>
        /// 收货批次
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GetReceiptDetailCollectionLotViewList(ReceiptCollectionLotQuery request)
        {
            var rsp = InboundApiClient.GetInstance().GetReceiptDetailCollectionLotViewList(request, LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        /// <summary>
        /// 保存批次采集
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult SaveReceiptDetailLot(ReceiptCollectionLotDto request)
        {
            var rsp = InboundApiClient.GetInstance().SaveReceiptDetailLot(request, this.LoginCoreQuery);

            if (rsp.Success && rsp.ResponseResult != null && rsp.ResponseResult.IsSuccess)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, Message = rsp.ApiMessage == null ? rsp.ResponseResult.ErrorMessage : rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
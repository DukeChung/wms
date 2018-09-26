using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using NBK.WMS.Portal.Services;

namespace NBK.WMS.Portal.Controllers
{
    public class PurchaseController : BaseController
    {
        // GET: Purchase
        public ActionResult Index(string from)
        {
            if (!string.IsNullOrEmpty(from) && from == "home")
            {
                ViewBag.AuditingDateTo = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            }
            InitializeViewBag(true);
            return View();
        }



        public ActionResult PurchaseReturn(string from)
        {
            if (!string.IsNullOrEmpty(from) && from == "home")
            {
                ViewBag.AuditingDateTo = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            }
            InitializeViewBag(true, true);
            return View();
        } 

        public ActionResult GetPurchaseDtoList(PurchaseQuery purchaseQuery)
        {
            purchaseQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            if (purchaseQuery.AuditingDateTo.HasValue)
                purchaseQuery.AuditingDateTo = purchaseQuery.AuditingDateTo.Value.AddDays(1).AddMilliseconds(-1);

            var rsp = InboundApiClient.GetInstance().GetPurchaseDtoList(purchaseQuery);
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

        public ActionResult GetPurchaseDtoReturnList(PurchaseReturnQuery purchaseQuery)
        {
            purchaseQuery.WarehouseSysId = CurrentUser.WarehouseSysId;
            if (purchaseQuery.AuditingDateTo.HasValue)
                purchaseQuery.AuditingDateTo = purchaseQuery.AuditingDateTo.Value.AddDays(1).AddMilliseconds(-1);

            var rsp = InboundApiClient.GetInstance().GetPurchaseDtoReturnList(purchaseQuery);
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
        /// 查看采购单
        /// </summary>
        /// <returns></returns>
        public ActionResult PurchaseView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InboundApiClient.GetInstance().GetPurchaseViewDtoBySysId(sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                //跳转异常页面
                return null;
            }
        }

        /// <summary>
        /// 查看退货入库单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public ActionResult PurchaseReturnView(Guid sysId)
        {
            ViewBag.PrintName = PublicConst.PrintSettingA4;
            ViewBag.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InboundApiClient.GetInstance().GetPurchaseReturnViewDtoBySysId(sysId, CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                //跳转异常页面
                return null;
            }
        }

        public ActionResult AppointBatchNumber(string sysIds, string batchNumber)
        {
            var rsp = InboundApiClient.GetInstance().AppointBatchNumber(sysIds, batchNumber, CurrentUser.WarehouseSysId);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        private void InitializeViewBag(bool hasOptionLabel, bool IsPurchaseReturn = false)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                statusList.Add(new SelectListItem { Text = "请选择" });
            }
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.New), Value = ((int)PurchaseStatus.New).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.InReceipt), Value = ((int)PurchaseStatus.InReceipt).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.PartReceipt), Value = ((int)PurchaseStatus.PartReceipt).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.Finish), Value = ((int)PurchaseStatus.Finish).ToString() });
            //statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.StopReceipt), Value = ((int)PurchaseStatus.StopReceipt).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.Void), Value = ((int)PurchaseStatus.Void).ToString() });
            statusList.Add(new SelectListItem { Text = ConverStatus.Purchase((int)PurchaseStatus.Close), Value = ((int)PurchaseStatus.Close).ToString() });
            ViewBag.StatusList = statusList;

            List<SelectListItem> typeList = new List<SelectListItem>();
            if (hasOptionLabel)
            {
                typeList.Add(new SelectListItem { Text = "请选择" });
            }
            if (!IsPurchaseReturn)
            {
                typeList.Add(new SelectListItem { Text = PurchaseType.Purchase.ToDescription(), Value = ((int)PurchaseType.Purchase).ToString() });
                //typeList.Add(new SelectListItem { Text = PurchaseType.Return.ToDescription(), Value = ((int)PurchaseType.Return).ToString() });
                typeList.Add(new SelectListItem { Text = PurchaseType.Material.ToDescription(), Value = ((int)PurchaseType.Material).ToString() });
                typeList.Add(new SelectListItem { Text = PurchaseType.TransferInventory.ToDescription(), Value = ((int)PurchaseType.TransferInventory).ToString() });
                typeList.Add(new SelectListItem { Text = PurchaseType.Fertilizer.ToDescription(), Value = ((int)PurchaseType.Fertilizer).ToString() });
            }
            else
            {
                typeList.Add(new SelectListItem { Text = PurchaseType.Return.ToDescription(), Value = ((int)PurchaseType.Return).ToString() });
            }

            ViewBag.TypeList = typeList;
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                ViewBag.WarehouseList = rsp.ResponseResult;
            }

        }

        #region 批量采购

        public ActionResult PurchaseBatch()
        {
            List<SelectListItem> vendorList = new List<SelectListItem>();
            var rsp = InboundApiClient.GetInstance().SelectItemVendor(LoginCoreQuery);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                vendorList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.VendorList = vendorList;
            return View();
        }

        public ActionResult GetPurchaseBatch()
        {
            PurchaseBatchDto purchaseBatchDto = new PurchaseBatchDto
            {
                VendorSysId = Guid.Empty,
                PurchaseDateText = DateTime.Now.Date.ToString("yyyy/MM/dd"),
                PoGroup = DateTime.Now.ToString("yyyyMMddHHmmss"),
                PurchaseDetailBatchDto = new List<PurchaseDetailBatchDto>()
            };
            return Json(new { Success = true, ViewModel = purchaseBatchDto }, JsonRequestBehavior.AllowGet);
        }

        [SetUserInfo]
        public ActionResult SaveBatchPurchaseAndReceipt(PurchaseBatchDto purchaseBatchDto)
        {
            var rsp = InboundApiClient.GetInstance().SaveBatchPurchaseAndReceipt(LoginCoreQuery, purchaseBatchDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 作废采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult ObsoletePurchase(PurchaseOperateDto purchaseDto)
        {
            var rsp = InboundApiClient.GetInstance().ObsoletePurchase(purchaseDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 关闭采购单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult ClosePurchase(PurchaseOperateDto purchaseDto)
        {
            var rsp = InboundApiClient.GetInstance().ClosePurchase(purchaseDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 入库单生成质检单
        /// </summary>
        /// <param name="purchaseDto"></param>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult GenerateQcOrderByPurchase(PurchaseQcDto purchaseQcDto)
        {
            var rsp = InboundApiClient.GetInstance().GenerateQcOrderByPurchase(purchaseQcDto);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoShelves(PurchaseOperateDto purchaseDto)
        {
            var rsp = InboundApiClient.GetInstance().AutoShelves(purchaseDto, LoginCoreQuery);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消收货（入库单整单取消）
        /// </summary>
        /// <returns></returns>
        [SetUserInfo]
        public ActionResult CancelReceiptByPurchase(ReceiptCancelDto receiptCancelDto)
        {
            var rsp = InboundApiClient.GetInstance().CancelReceiptByPurchase(receiptCancelDto, this.LoginCoreQuery);

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
        /// 修改业务类型（指定上下行）
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="businessType"></param>
        /// <returns></returns>
        public ActionResult UpdatePurchaseBusinessTypeBySysId(string sysIds, string businessType)
        {
            var rsp = InboundApiClient.GetInstance().UpdatePurchaseBusinessTypeBySysId(sysIds, businessType, CurrentUser.WarehouseSysId);
            if (rsp != null && rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}
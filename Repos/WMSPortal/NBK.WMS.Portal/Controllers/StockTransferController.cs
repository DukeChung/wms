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
    public class StockTransferController : BaseController
    {
        public ActionResult StockTransferRequest()
        {
            return View();
        }
        [SetUserInfoAttribute]
        public ActionResult GetStockTransferLotByPage(StockTransferQuery request)
        {
            if (request.ExpiryDateTo.HasValue)
                request.ExpiryDateTo = request.ExpiryDateTo.Value.AddDays(1);
            if (request.ProduceDateTo.HasValue)
                request.ProduceDateTo = request.ProduceDateTo.Value.AddDays(1);


            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockTransferLotByPage(request, this.LoginCoreQuery);
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

        public ActionResult StockTransferAdd(Guid sysId)
        {
            var response = InventoryApiClient.GetInstance().GetStockTransferBySysId(sysId,CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new StockTransferDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }

            List<SelectListItem> locationList = new List<SelectListItem>();
            locationList.Add(new SelectListItem());
            var rsp = BaseDataApiClient.GetInstance().SelectLocation(LoginCoreQuery, CurrentUser.WarehouseSysId);
            if (rsp.Success && rsp.ResponseResult != null)
            {
                locationList.AddRange(rsp.ResponseResult.Select(p => new SelectListItem
                {
                    Text = p.Text,
                    Value = p.Value
                }));
            }
            ViewBag.LocationList = locationList;

            return View(model);
        }

        [SetUserInfoAttribute]
        public ActionResult CreateStockTransfer(StockTransferDto st)
        {
            
            var rsp = InventoryApiClient.GetInstance().CreateStockTransfer(st, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StockTransferList()
        {
            ViewBag.PrintCaseName = PublicConst.PrintSettingCase;
            return View();
        }

        public ActionResult GetStockTransferOrderByPage(StockTransferQuery request)
        {
            request.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = InventoryApiClient.GetInstance().GetStockTransferOrderByPage(request, this.LoginCoreQuery);
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

        public ActionResult StockTransferOperation(Guid sysId)
        {
            var request = new StockTransferDto() { SysId = sysId ,CurrentUserId = CurrentUser.UserId, CurrentDisplayName = CurrentUser.DisplayName, WarehouseSysId = CurrentUser.WarehouseSysId };
            var rsp = InventoryApiClient.GetInstance().StockTransferOperation(request , this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StockTransferCancel(Guid sysId)
        {
            var request = new StockTransferDto() { SysId = sysId, CurrentUserId = CurrentUser.UserId, CurrentDisplayName = CurrentUser.DisplayName, WarehouseSysId = CurrentUser.WarehouseSysId };
            var rsp = InventoryApiClient.GetInstance().StockTransferCancel(request, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StockTransferView(Guid sysId)
        {
            var response = InventoryApiClient.GetInstance().GetStockTransferOrderBySysId(sysId, CurrentUser.WarehouseSysId, this.LoginCoreQuery);
            var model = new StockTransferDto();
            if (response.Success)
            {
                model = response.ResponseResult;
            }

            return View(model);
        }
    }
}
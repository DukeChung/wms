using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.AuthServiceReference;
using NBK.WMS.Portal.Models;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class WarehouseController : BaseController
    {
        public ActionResult UserWarehouseOperation()
        {
            List<ApplicationUser> userList = AuthorizeManager.GetAllSystemUser();
            List<SelectListItem> selectUserList = new List<SelectListItem>();
            userList.ForEach(p => {
                selectUserList.Add(new SelectListItem() { Text = p.DisplayName, Value = p.UserId.ToString()});
            });
            ViewBag.UserList = selectUserList;

            return View();
        }

        [SetUserInfo]
        public ActionResult GetNoAssignedWarehouse(UserWarehouseQuery request)
        {
            var rsp = BaseDataApiClient.GetInstance().GetNoAssignedWarehouse(request,LoginCoreQuery);
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

        [SetUserInfo]
        public ActionResult GetAssignedWarehouse(UserWarehouseQuery request)
        {
            var rsp = BaseDataApiClient.GetInstance().GetAssignedWarehouse(request, LoginCoreQuery);
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

        [SetUserInfo]
        public ActionResult SetAssignedWarehouse(UserWarehouseDto request)
        {
            var rsp = BaseDataApiClient.GetInstance().SetAssignedWarehouse(request, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfo]
        public ActionResult SetNoAssignedWarehouse(UserWarehouseDto request)
        {
            var rsp = BaseDataApiClient.GetInstance().SetNoAssignedWarehouse(request, this.LoginCoreQuery);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetWarehouseList()
        {
            var rsp = InventoryApiClient.GetInstance().SelectItemWarehouse(LoginCoreQuery);
            if (rsp.Success)
            {
                return Json(new { success = true,warehouseList = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
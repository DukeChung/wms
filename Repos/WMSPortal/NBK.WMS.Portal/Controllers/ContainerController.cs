using NBK.AuthServiceUtil;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMS.Portal.Controllers
{
    [Authorize]
    public class ContainerController : BaseController
    {
        public ActionResult ContainerMaintain()
        {
            return View();
        }

        public ActionResult GetContainerList(ContainerQuery request)
        {
            var rsp = BaseDataApiClient.GetInstance().GetContainerList(request);
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

        public ActionResult DeleteContainer(string sysIdList)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteContainer(sysIdList,CurrentUser.WarehouseSysId);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

       [PermissionAuthorizeAttribute("BaseData_Container_Create")]
        public ActionResult AddContainer()
        {
            return View(new ContainerDto() { IsActive = true });
        }

        public ActionResult AddContainerService(ContainerDto container)
        {
            var rsp = BaseDataApiClient.GetInstance().AddContainer(container);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateContainer(Guid sysid)
        {
            ViewBag.ContainerSysId = sysid;

            return View();
        }

        public ActionResult GetContainerBySysId(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetContainerBySysId(sysId,CurrentUser.WarehouseSysId);

            if (rsp.Success)
            {
                return Json(new { success = true, Container = rsp.ResponseResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [SetUserInfoAttribute]
        public ActionResult UpdateContainerService(ContainerDto container)
        {
            var rsp = BaseDataApiClient.GetInstance().UpdateContainer(container);

            if (rsp.Success)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
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
    public class ComponentController : BaseController
    {
        //[PermissionAuthorize("BaseData_Component")]
        public ActionResult ComponentList()
        {
            return View();
        }

        public ActionResult GetComponentListByPaging(ComponentQuery componentQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetComponentListByPaging(componentQuery, LoginCoreQuery);
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

        //[PermissionAuthorize("BaseData_Component_Edit")]
        public ActionResult EditComponent(Guid sysId)
        {
            ViewBag.SysId = sysId;
            return View();
        }

        public ActionResult GetComponentById(Guid sysId)
        {
            var rsp = BaseDataApiClient.GetInstance().GetComponentById(new ComponentQuery() { SysId = sysId }, LoginCoreQuery);
            if (rsp.ResponseResult != null)
            {
                return Json(rsp.ResponseResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}
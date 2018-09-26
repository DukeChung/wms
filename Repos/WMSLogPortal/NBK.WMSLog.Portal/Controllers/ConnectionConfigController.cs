using NBK.ECService.WMSLog.DTO;
using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    public class ConnectionConfigController : BaseController
    {
        public ActionResult ConfigOperation()
        {
            List<ConnectionStringDto> list = ConnectionConfigApiClient.GetInstance().GetAllWarehouseInfo(LoginCoreQuery).ResponseResult;
            ViewData["operationList"] = list;
            return View();
        }

        public JsonResult GetConfig(string warehouseSysId)
        {
            try
            {
                ConnectionStringDto dto = ConnectionConfigApiClient.GetInstance().GetConfig(LoginCoreQuery, warehouseSysId).ResponseResult;
                return Json(new { Success = true, ConnectionString = dto.ConnectionString, ConnectionStringRead = dto.ConnectionStringRead }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SaveConfig(string warehouseSysId, string connectionString, string connectionStringRead)
        {
            try
            {
                bool result = ConnectionConfigApiClient.GetInstance().UpdateWarehouseConnectionString(LoginCoreQuery, warehouseSysId, connectionString, connectionStringRead).ResponseResult;
                return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
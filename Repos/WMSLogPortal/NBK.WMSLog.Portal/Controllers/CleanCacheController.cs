using NBK.ECService.WMS.Utility;
using NBK.ECService.WMSLog.Utility;
using NBK.ECService.WMSLog.Utility.Enum;
using NBK.WMSLog.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBK.WMSLog.Portal.Controllers
{
    public class CleanCacheController : BaseController
    {

        public ActionResult SynOperation()
        {
            Dictionary<int, string> operationDic = new Dictionary<int, string>();
            foreach (SynEnum item in Enum.GetValues(typeof(SynEnum)))
            {
                operationDic.Add((int)item, EnumHelper.ToDescription(item));
            }
            ViewData["operationDic"] = operationDic; 
            return View();
        }

        public JsonResult GetRedis(int SynType)
        {
            try
            {
                string key = string.Empty;
                switch (SynType)
                {
                    case (int)SynEnum.SynchroAll: 

                        break;
                    case (int)SynEnum.SynchroMenu:
                        key = RedisSourceKey.RedisMenuList;
                        break;
                    case (int)SynEnum.CleanUserLoginRedis:
                        key = RedisSourceKey.RedisLoginUserList;
                        break;
                    case (int)SynEnum.SynchroSku:
                        key = RedisSourceKey.RedisSkuList;
                        break;
                    case (int)SynEnum.SynchroPack:
                        key = RedisSourceKey.RedisPack;
                        break;
                    case (int)SynEnum.SynchroLottemplate:
                         
                        break;
                    case (int)SynEnum.SynchroWarehouse:
                        key = RedisSourceKey.RedisWareHouseList;
                        break;
                    default:
                        break;
                }
                string redisStr = CleanCacheApiClient.GetInstance().GetRedis(LoginCoreQuery, key).ResponseResult;
                return Json(new { Success = true, RedisInfo = redisStr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult CallSynInterface(int SynType)
        {
            try
            {
                switch (SynType)
                {
                    case (int)SynEnum.SynchroAll:
                        CleanCacheApiClient.GetInstance().SynchroAll();
                        break;
                    case (int)SynEnum.SynchroMenu:
                        CleanCacheApiClient.GetInstance().SynchroMenu();
                        break;
                    case (int)SynEnum.CleanUserLoginRedis:
                        CleanCacheApiClient.GetInstance().CleanUserLoginRedis();
                        break;
                    case (int)SynEnum.SynchroSku:
                        CleanCacheApiClient.GetInstance().SynchroSku();
                        break;
                    case (int)SynEnum.SynchroPack:
                        CleanCacheApiClient.GetInstance().SynchroPack();
                        break;
                    case (int)SynEnum.SynchroLottemplate:
                        CleanCacheApiClient.GetInstance().SynchroLottemplate();
                        break;
                    case (int)SynEnum.SynchroWarehouse:
                        CleanCacheApiClient.GetInstance().SynchroWarehouse();
                        break;
                    default:
                        break;
                }

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet); 
            } 
        }

    }
}
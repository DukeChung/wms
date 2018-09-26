using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBK.ECService.WMS.DTO;
using NBK.WMS.Portal.Services;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.Utility;

namespace NBK.WMS.Portal.Controllers
{
    public class RuleSettingController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PreOrderRule()
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().GetPreOrderRuleByWarehouseSysId(CurrentUser.WarehouseSysId);
            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                return View();
            }
        }

        public ActionResult OutboundRule()
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().GetOutboundRuleByWarehouseSysId(CurrentUser.WarehouseSysId);

            #region 出库排序规则
            List<SelectListItem> deliverySortRulesList = new List<SelectListItem>();
            deliverySortRulesList.Add(new SelectListItem { Text = "无规则", Value = "0" });
            foreach (int info in Enum.GetValues(typeof(DeliverySortRules)))
            {
                var strText = ((DeliverySortRules)info).ToDescription();
                deliverySortRulesList.Add(new SelectListItem { Text = strText, Value = info.ToString() });
            }
            ViewBag.DeliverySortRulesList = deliverySortRulesList;
            #endregion

            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                return View();
            }
        }

        public ActionResult WorkRule()
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().GetWorkRuleByWarehouseSysId(CurrentUser.WarehouseSysId);

            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                return View();
            }

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveWorkRule(WorkRuleDto workRuleDto)
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().SaveWorkRule(workRuleDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveOutboundRule(OutboundRuleDto outboundRuleDto)
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().SaveOutboundRule(outboundRuleDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult SavePreOrderRule(PreOrderRuleDto preOrderRuleDto)
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().SavePreOrderRule(preOrderRuleDto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #region 加工规则
        public ActionResult AssemblyRule()
        {
            var rsp = OrderRuleSettingApiClient.GetInstance().GetAssemblyRuleWarehouseSysId(CurrentUser.WarehouseSysId);

            #region 加工排序规则
            List<SelectListItem> deliverySortRulesList = new List<SelectListItem>();
            deliverySortRulesList.Add(new SelectListItem { Text = "无规则", Value = "0" });
            foreach (int info in Enum.GetValues(typeof(DeliveryAssemblyRule)))
            {
                var strText = ((DeliveryAssemblyRule)info).ToDescription();
                deliverySortRulesList.Add(new SelectListItem { Text = strText, Value = info.ToString() });
            }
            ViewBag.DeliverySortRulesList = deliverySortRulesList;
            #endregion

            if (rsp.Success)
            {
                return View(rsp.ResponseResult);
            }
            else
            {
                return View();
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveAssemblyRule(AssemblyRuleDto dto)
        {
            dto.CurrentUserId = CurrentUser.UserId;
            dto.CurrentDisplayName = CurrentUser.UserName;
            dto.WarehouseSysId = CurrentUser.WarehouseSysId;
            var rsp = OrderRuleSettingApiClient.GetInstance().SaveAssemblyRule(dto);
            if (rsp.Success)
            {
                return Json(new { Success = true, Message = "操作成功!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
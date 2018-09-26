using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneLab.WebApiClient;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.WMS.Portal.Services;

namespace NBK.WMS.Portal.Controllers
{

    public class LotTemplateController : BaseController
    {
        public LotTemplateController()
        {
            
        }
        // GET: LotTemplate
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLotTempListDto(LotTemplateQuery lotTemplateQuery)
        {
            var rsp = BaseDataApiClient.GetInstance().GetLotTempListDto(lotTemplateQuery);
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
        /// 新增模板
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLotTemplate()
        {
            return View("Add");
        }

        

        /// <summary>
        /// 新增模板
        /// </summary>
        /// <returns></returns>
        public ActionResult EditLotTemplate(Guid sysId)
        {
            ViewBag.SysId = sysId;

            var rsp = BaseDataApiClient.GetInstance().GetLotTemplateDtoById(sysId);
            if (rsp.Success)
            {
                return View("Edit", rsp.ResponseResult);
            }
            else
            {
               // 异常页面
                return null;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="lotTemplateDto"></param> 
        /// <param name="actionName"></param>
        /// <returns></returns>
        public ActionResult SaveLotTemplate(LotTemplateDto lotTemplateDto,string actionName)
        {
            lotTemplateDto.SeqNo09 = 9;
            lotTemplateDto.Lot09 = "SN";
            var rsp = new ApiResponse<string>();
            if (actionName == "Add")
            {
                 rsp = BaseDataApiClient.GetInstance().InsertLotTemplate(lotTemplateDto);
            }
            else
            {
                rsp = BaseDataApiClient.GetInstance().UpdateLotTemplate(lotTemplateDto);
            }
            if (rsp.Success)
            {
                return Json(new { success = true, message = "保存成功！" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "保存失败," + rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteLotTemplate(string sysIds)
        {
            var rsp = BaseDataApiClient.GetInstance().DeleteLotTemplate(sysIds.ToGuidList());
            if (rsp.Success)
            {
                return Json(new { success = true, message = "删除成功！" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "删除失败,"+rsp.ApiMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using FortuneLab.ECService.Securities.Filters;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{

    /// <summary>
    /// 批次模板
    /// </summary>
    [RoutePrefix("api/BaseData/LotTemplate")]
    [AccessLog]
    public class LotTemplateController : AbpApiController
    {
        private ILotTemplateAppService _lotTemplateAppService;
        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="lotTemplateAppService"></param>
        public LotTemplateController(ILotTemplateAppService lotTemplateAppService)
        {
            _lotTemplateAppService = lotTemplateAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void LotTemplatApi() { }

        /// <summary>
        /// 根据ID获取SysCode
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetLotTemplateDtoById")]
        public LotTemplateDto GetLotTemplateDtoById(Guid sysId)
        {
            return _lotTemplateAppService.GetLotTemplateDtoById(sysId);
        }

        /// <summary>
        /// 删除批次模板
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteLotTemplate")]
        public void DeleteLotTemplate(List<Guid> sysId)
        {
            _lotTemplateAppService.DeleteLotTemplate(sysId);
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="lotTemplateQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetLotTemplatList")]
        public Pages<LotTemplateListDto> GetLotTemplateDtoListByPageInfo(LotTemplateQuery lotTemplateQuery)
        {
           return  _lotTemplateAppService.GetLotTemplateDtoListByPageInfo(lotTemplateQuery);
        }

        /// <summary>
        /// 新增批次模板
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertLotTemplate")]
        public void InsertLotTemplate(LotTemplateDto lotTemplateDto)
        {
             _lotTemplateAppService.InsertLotTemplate(lotTemplateDto);
        }

        /// <summary>
        /// 更新批次模板
        /// </summary>
        /// <param name="lotTemplateDto"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateLotTemplate")]
        public void UpdateLotTemplate(LotTemplateDto lotTemplateDto)
        {
            _lotTemplateAppService.UpdateLotTemplate(lotTemplateDto);
        }


        /// <summary>
        /// 获取下拉 批次模板
        /// </summary>
        /// <param name="lotCode"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSelectLotTemplate")]
        public List<SelectItem> GetSelectLotTemplate(string lotCode=null)
        {
           return _lotTemplateAppService.GetSelectLotTemplate(lotCode);
        }
    }
}
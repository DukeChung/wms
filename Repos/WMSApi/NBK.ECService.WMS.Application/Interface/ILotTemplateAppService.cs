using System;
using System.Collections.Generic;
using Abp.Application.Services;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ILotTemplateAppService : IApplicationService
    {
        /// <summary>
        /// 根据CodeType 获取相关信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        LotTemplateDto GetLotTemplateDtoById(Guid sysId);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="lotTemplateQuery"></param>
        /// <returns></returns>
        Pages<LotTemplateListDto> GetLotTemplateDtoListByPageInfo(LotTemplateQuery lotTemplateQuery);

        /// <summary>
        /// 更新LotTemplate
        /// </summary>
        /// <returns></returns>
        void UpdateLotTemplate(LotTemplateDto lotTemplateDto);

        /// <summary>
        ///新增LotTemplate
        /// </summary>
        /// <returns></returns>
        void InsertLotTemplate(LotTemplateDto lotTemplateDto);

        /// <summary>
        ///新增LotTemplate
        /// </summary>
        /// <returns></returns>
        void DeleteLotTemplate(List<Guid> sysId);

        List<SelectItem> GetSelectLotTemplate(string lotCode = null);

    }
}
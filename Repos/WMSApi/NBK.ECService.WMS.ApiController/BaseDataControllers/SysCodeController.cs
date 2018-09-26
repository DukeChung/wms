using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using FortuneLab.ECService.Securities.Filters;
using FortuneLab.Models;
using NBK.ECService.WMS.Application;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Core.WebApi.Filters;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 基础资料系统代码
    /// </summary>
    [RoutePrefix("api/BaseData/SysCode")]
    [AccessLog]
    public class SysCodeController: AbpApiController
    {
        private ISysCodeAppService _sysCodeAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="sysCodeAppService"></param>
        public SysCodeController(ISysCodeAppService sysCodeAppService)
        {
            _sysCodeAppService = sysCodeAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void SysCodeApi() { }

        /// <summary>
        /// 根据ID获取SysCode
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSysCodeDtoById")]
        public SysCodeDto GetSysCodeDtoById(Guid sysId)
        {
            return _sysCodeAppService.GetSysCodeDtoById(sysId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSysCodeDetailDtoById")]
        public SysCodeDetailDto GetSysCodeDetailDtoById(Guid sysId)
        {
            return _sysCodeAppService.GetSysCodeDetailDtoById(sysId);
        }

        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="sysId"></param>
        [HttpPost, Route("DeleteSysCodeDetailByIdList")]
        public void DeleteSysCodeDetailByIdList(List<Guid> sysIdList)
        {
            _sysCodeAppService.DeleteSysCodeDetailByIdList(sysIdList);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="sysCodeQuery"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSysCodeDtoList")]
        public Pages<SysCodeDto> GetSysCodeDtoListByPageInfo(SysCodeQuery sysCodeQuery)
        {
            return _sysCodeAppService.GetSysCodeDtoListByPageInfo(sysCodeQuery);
        }

        /// <summary>
        /// 查询明细列表
        /// </summary>
        /// <param name="sysCodeSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetSysCodeDetailDtoList")]
        public List<SysCodeDetailDto> GetSysCodeDetailDtoList(Guid sysCodeSysId)
        {
            return _sysCodeAppService.GetSysCodeDetailDtoList(sysCodeSysId);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="sysCodeDto"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateSysCode")]
        public void UpdateSysCode(SysCodeDto sysCodeDto)
        {
            _sysCodeAppService.UpdateSysCode(sysCodeDto);
        }

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        [HttpPost, Route("UpdateSysCodeDetail")]
        public void UpdateSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            _sysCodeAppService.UpdateSysCodeDetail(sysCodeDetailDto);
        }

        /// <summary>
        /// 更新SysCodeDetail
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        [HttpPost, Route("InsertSysCodeDetail")]
        public void InsertSysCodeDetail(SysCodeDetailDto sysCodeDetailDto)
        {
            _sysCodeAppService.InsertSysCodeDetail(sysCodeDetailDto); 
        }

        /// <summary>
        /// 获取系统设置明细
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [HttpGet, Route("GetSelectBySysCode")]
        public List<SelectItem> GetSelectBySysCode(string sysCodeType, bool isActive)
        {
            return _sysCodeAppService.GetSysCodeDetailList(sysCodeType, isActive);
        }
    }
}
using System;
using System.Collections.Generic;
using Abp.Application.Services;
using FortuneLab.Models;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface ISysCodeAppService : IApplicationService
    {
        /// <summary>
        /// sysCodeId 获取相关信息
        /// </summary>
        /// <param name="sysCodeId"></param>
        /// <returns></returns>
        SysCodeDto GetSysCodeDtoById(Guid sysCodeId);

        /// <summary>
        /// 根据SysId 获取信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        SysCodeDetailDto GetSysCodeDetailDtoById(Guid sysId);

        /// <summary>
        /// 删除SysCodeDetail
        /// </summary>
        /// <param name="sysId"></param>
        void DeleteSysCodeDetailByIdList(List<Guid> sysId);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="sysCodeQuery"></param>
        /// <returns></returns>
        Pages<SysCodeDto> GetSysCodeDtoListByPageInfo(SysCodeQuery sysCodeQuery);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="sysCodeSysId"></param>
        /// <returns></returns>
        List<SysCodeDetailDto> GetSysCodeDetailDtoList(Guid sysCodeSysId);

        /// <summary>
        /// 更新SysCode
        /// </summary>
        /// <returns></returns>
        void UpdateSysCode(SysCodeDto sysCodeDto);

        /// <summary>
        /// 更新明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        void UpdateSysCodeDetail(SysCodeDetailDto sysCodeDetailDto);

        /// <summary>
        /// 新增明细
        /// </summary>
        /// <param name="sysCodeDetailDto"></param>
        void InsertSysCodeDetail(SysCodeDetailDto sysCodeDetailDto);

        /// <summary>
        /// 获取系统代码明细
        /// </summary>
        /// <param name="sysCodeType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        List<SelectItem> GetSysCodeDetailList(string sysCodeType, bool isActive);

    }
}
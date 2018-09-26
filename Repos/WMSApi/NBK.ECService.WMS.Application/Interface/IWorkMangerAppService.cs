using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IWorkMangerAppService : IApplicationService
    {
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Pages<WorkListDto> GetWorkByPage(WorkQueryDto request);

        /// <summary>
        /// 根据ID获取工单明细
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        WorkDetailDto GetWorkBySysId(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 作废工单
        /// </summary>
        /// <param name="request"></param>
        void CancelWork(CancelWorkDto request);

        /// <summary>
        /// 根据来源单据号修改工单状态
        /// </summary>
        /// <param name="request"></param>
        CommonResponse UpdateWorkByDocOrder(MQWorkDto request);

        /// <summary>
        /// 增加工单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CommonResponse MQAddWork(MQWorkDto request);

        /// <summary>
        /// 更新工单
        /// </summary>
        /// <param name="request"></param>
        void UpdateWorkInfo(WorkUpdateDto request);

        /// <summary>
        /// 新建工单
        /// </summary>
        /// <param name="dtoList"></param>
        void AddWork(List<WorkDetailDto> dtoList);
    }
}

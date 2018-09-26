using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using System;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPreBulkPackRepository : ICrudRepository
    {
        Pages<PreBulkPackDto> GetPreBulkPackByPage(PreBulkPackQuery request);

        PreBulkPackDto GetPreBulkPackBySysId(Guid sysId);

        /// <summary>
        /// 根据出库单更新散货封箱单状态
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户名</param>
        void UpdaPreBulkPack(Guid outboundSysId, int userId, string userName);

        /// <summary>
        /// 作废散货封箱单
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        void CancelPreBulkPack(Guid outboundSysId, int userId, string userName);


        /// <summary>
        /// 修改散货状态
        /// </summary>
        /// <param name="outboundSysId">出库单ID</param>
        /// <param name="userId">修改人ID</param>
        /// <param name="userName">修改人名称</param>
        /// <param name="toStatus">目标状态</param>
        /// <param name="fromStatus">原始装填，可不填</param>
        void UpdatePreBulkPackStatus(Guid outboundSysId, int userId, string userName, int toStatus, int fromStatus = 0);


        /// <summary>
        /// 根据出库单ID回去散货封箱单列表
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<OutboundPreBulkPackDto> GetOutboundPreBulkPackList(Guid outboundSysId);

        /// <summary>
        /// 获取出库单散货箱明细
        /// </summary>
        /// <param name="preBulkPackSysIds"></param>
        /// <returns></returns>
        List<PreBulkPackDetailDto> GetPreBulkPackDetailByPreBulkPackSysIds(List<Guid> preBulkPackSysIds);
    }
}

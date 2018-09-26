using Abp.Application.Services;
using System;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IRedisAppService: IApplicationService
    {
        /// <summary>
        /// 同步菜单
        /// </summary>
        void SynchroMenu();

        /// <summary>
        /// 同步SKU  包含分类 
        /// </summary>
        void SynchroSku();

        /// <summary>
        /// 清除用户登录Redis
        /// </summary>
        void CleanUserLoginRedis();

        /// <summary>
        /// 同步包装 包含单位
        /// </summary>
        void SynchroPack();

        /// <summary>
        /// 同步批次模板
        /// </summary>
        void SynchroLottemplate();

        /// <summary>
        /// 同步仓库
        /// </summary>
        void SynchroWarehouse();

        /// <summary>
        /// 清除复核结果
        /// </summary>
        /// <param name="outboundOrder"></param>
        /// <param name="warehouseSysId"></param>
        void CleanReviewRecords(string outboundOrder, Guid warehouseSysId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void SynchroSetAccessLog(int type);

        /// <summary>
        /// 根据key清除缓存
        /// </summary>
        /// <param name="key"></param>
        void ClearRedisByKey(string key);
    }
}
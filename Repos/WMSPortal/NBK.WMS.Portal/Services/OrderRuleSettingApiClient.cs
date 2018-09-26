using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBK.WMS.Portal.Services
{
    public class OrderRuleSettingApiClient
    {
        private static readonly OrderRuleSettingApiClient instance = new OrderRuleSettingApiClient();

        private OrderRuleSettingApiClient()
        {
        }


        public static OrderRuleSettingApiClient GetInstance()
        {
            return instance;
        }

        #region 工单规则设置

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<WorkRuleDto> GetWorkRuleByWarehouseSysId(Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Get<WorkRuleDto>(PublicConst.WmsApiUrl, "/OrderRuleSetting/GetWorkRuleByWarehouseSysId", query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workRuleDto"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveWorkRule(WorkRuleDto workRuleDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/OrderRuleSetting/SaveWorkRule", query, workRuleDto);
        }
        #endregion

        #region 出库规则设置

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<OutboundRuleDto> GetOutboundRuleByWarehouseSysId(Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Get<OutboundRuleDto>(PublicConst.WmsApiUrl, "/OrderRuleSetting/GetOutboundRuleByWarehouseSysId", query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveOutboundRule(OutboundRuleDto outboundRuleDto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/OrderRuleSetting/SaveOutboundRule", query, outboundRuleDto);
        }
        #endregion

        #region 预包装规则设置
        /// <summary>
        /// 移仓单列表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<PreOrderRuleDto> GetPreOrderRuleByWarehouseSysId(Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Get<PreOrderRuleDto>(PublicConst.WmsApiUrl, "/OrderRuleSetting/GetPreOrderRuleByWarehouseSysId", query);
        }

        /// <summary>
        /// 移仓单列表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ApiResponse<string> SavePreOrderRule(PreOrderRuleDto preOrderRuleDto)
        {
            var query = new CoreQuery();

            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/OrderRuleSetting/SavePreOrderRule", query, preOrderRuleDto);
        }
        #endregion

        #region 加工规则
        /// <summary>
        /// 根据仓库ID获取加工规则
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        public ApiResponse<AssemblyRuleDto> GetAssemblyRuleWarehouseSysId(Guid warehouseSysId)
        {
            var query = new CoreQuery();
            query.ParmsObj = new { warehouseSysId };
            return ApiClient.Get<AssemblyRuleDto>(PublicConst.WmsApiUrl, "/OrderRuleSetting/GetAssemblyRuleWarehouseSysId", query);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ApiResponse<string> SaveAssemblyRule(AssemblyRuleDto dto)
        {
            var query = new CoreQuery();
            return ApiClient.Post<string>(PublicConst.WmsApiUrl, "/OrderRuleSetting/SaveAssemblyRule", query, dto);
        }
        #endregion


    }
}
using Abp.WebApi.Controllers;
using NBK.ECService.WMSLog.Application.Interface;
using NBK.ECService.WMSLog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMSLog.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/ConnectionConfig")]
    public class ConnectionConfigController:AbpApiController
    {
        private IConnectionConfigAppService _iConnectionConfigAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iConnectionConfigAppService"></param>
        public ConnectionConfigController(IConnectionConfigAppService iConnectionConfigAppService)
        {
            _iConnectionConfigAppService = iConnectionConfigAppService;
        }

        /// <summary>
        /// 查询所有有效仓库信息
        /// </summary>
        [HttpGet, Route("GetAllWarehouseInfo")]
        public List<ConnectionStringDto> GetAllWarehouseInfo()
        {
            return _iConnectionConfigAppService.GetAllWarehouseInfo();
        }

        /// <summary>
        /// 获取仓库的连接字符串明文
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetConfig")]
        public ConnectionStringDto GetConfig(string warehouseSysId)
        {
            return _iConnectionConfigAppService.GetConfig(warehouseSysId);
        }


        /// <summary>
        /// 更新数据库连接
        /// </summary>
        /// <param name="warehouseSysId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateWarehouseConnectionString")]
        public bool UpdateWarehouseConnectionString(string warehouseSysId, string connectionString,string connectionStringRead)
        { 
            return _iConnectionConfigAppService.UpdateWarehouseConnectionString(warehouseSysId, connectionString, connectionStringRead);
        }

    }
}

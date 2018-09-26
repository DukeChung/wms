using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.Filters;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMS.ApiController.BaseDataControllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/WareHouse")]
    [AccessLog]
    public class WareHouseController : AbpApiController
    {
        private IWareHouseAppService _wareHouseAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wareHouseAppService"></param>
        public WareHouseController(IWareHouseAppService wareHouseAppService)
        {
            this._wareHouseAppService = wareHouseAppService;
        }

        /// <summary>
        /// WareHouse
        /// </summary>
        [HttpGet]
        public void WareHouseApi() { }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWareHouseByUserId")]
        public List<WareHouseDto> GetWareHouseByUserId(int userId)
        {
            return _wareHouseAppService.GetWareHouseByUserId(userId);
        }

        /// <summary>
        /// 查询用户未匹配的仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetNoAssignedWarehouse")]
        public Pages<UserWarehouseDto> GetNoAssignedWarehouse(UserWarehouseQuery request)
        {
            return _wareHouseAppService.GetNoAssignedWarehouse(request);
        }

        /// <summary>
        /// 查询用户已匹配的仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetAssignedWarehouse")]
        public Pages<UserWarehouseDto> GetAssignedWarehouse(UserWarehouseQuery request)
        {
            return _wareHouseAppService.GetAssignedWarehouse(request);
        }

        /// <summary>
        /// 设置匹配用户仓库
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("SetAssignedWarehouse")]
        public void SetAssignedWarehouse(UserWarehouseDto request)
        {
            _wareHouseAppService.SetAssignedWarehouse(request);
        }

        /// <summary>
        /// 取消匹配用户仓库
        /// </summary>
        /// <param name="request"></param>
        [HttpPost, Route("SetNoAssignedWarehouse")]
        public void SetNoAssignedWarehouse(UserWarehouseDto request)
        {
            _wareHouseAppService.SetNoAssignedWarehouse(request);
        }

        /// <summary>
        /// 根据仓库ID 是否写库标识 获取连接字符串
        /// </summary>
        /// <param name="WarehouseSysId"></param>
        /// <param name="IsWrite"></param>
        /// <returns></returns>
        [HttpPost, Route("GetConnectionStringByWarehouseSysId")]
        public string GetConnectionStringByWarehouseSysId(Guid WarehouseSysId, bool IsWrite)
        {
            return _wareHouseAppService.GetConnectionStringByWarehouseSysId(WarehouseSysId, IsWrite);
        } 

        /// <summary>
        /// 根据otherid获取仓库信息
        /// </summary>
        /// <param name="OtherId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetWarehouseByOtherId")]
        public WareHouseDto GetWarehouseByOtherId(string OtherId)
        {
            return _wareHouseAppService.GetWarehouseByOtherId(OtherId);
        }
    }
}

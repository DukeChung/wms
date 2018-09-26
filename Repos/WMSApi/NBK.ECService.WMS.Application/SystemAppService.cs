using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;
using NBK.ECService.WMS.DTO.System;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.Utility.Redis;

namespace NBK.ECService.WMS.Application
{
    public class SystemAppService : WMSApplicationService, ISystemAppService
    {
        private ICrudRepository _crudRepository = null;
        public SystemAppService(ICrudRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 获取系统菜单
        /// </summary>
        /// <returns></returns>
        public List<MenuDto> GetSystemMenuList()
        { 
            return RedisWMS.CacheResult(()=>
            {
                List<menu> menuList = null;
                menuList = _crudRepository.GetQuery<menu>(p => p.IsActive == true).ToList();
                return menuList.JTransformTo<MenuDto>();
            }, RedisSourceKey.RedisMenuList);
        }

    }
}

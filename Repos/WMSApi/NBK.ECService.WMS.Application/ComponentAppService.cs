using Abp.Application.Services;
using NBK.ECService.WMS.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Repository.Interface;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Core.WebApi.ApplicationService;

namespace NBK.ECService.WMS.Application
{
    public class ComponentAppService : WMSApplicationService, IComponentAppService
    {
        private IComponentRepository _crudRepository = null;

        public ComponentAppService(IComponentRepository crudRepository)
        {
            this._crudRepository = crudRepository;
        }

        /// <summary>
        /// 获取组装件列表
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        public Pages<ComponentListDto> GetComponentListByPaging(ComponentQuery componentQuery)
        {
            return _crudRepository.GetComponentListByPaging(componentQuery);
        }

        /// <summary>
        /// 获取组装件
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        public ComponentDto GetComponentById(ComponentQuery componentQuery)
        {
            var componentDto = _crudRepository.GetComponentById(componentQuery);
            if (componentDto != null)
            {
                componentDto.ComponentDetailDtoList = _crudRepository.GetComponentDetailList(componentQuery);
            }

            return componentDto;
        }
    }
}

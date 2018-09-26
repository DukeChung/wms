using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IComponentAppService : IApplicationService
    {
        /// <summary>
        /// 获取组装件列表
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        Pages<ComponentListDto> GetComponentListByPaging(ComponentQuery componentQuery);

        /// <summary>
        /// 获取组装件
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        ComponentDto GetComponentById(ComponentQuery componentQuery);
    }
}

using System;
using Abp.Domain.Repositories;
using NBK.ECService.WMS.DTO;
using System.Collections.Generic;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IComponentRepository : ICrudRepository
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

        /// <summary>
        /// 获取组装件明细
        /// </summary>
        /// <param name="componentQuery"></param>
        /// <returns></returns>
        List<ComponentDetailDto> GetComponentDetailList(ComponentQuery componentQuery);
    }
}

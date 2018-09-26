using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IContainerAppService : IApplicationService
    {
        Pages<ContainerDto> GetContainerList(ContainerQuery query);

        void DeleteContainer(string sysIdList, Guid warehouseSysId);

        void AddContainer(ContainerDto container);

        ContainerDto GetContainerBySysId(Guid sysId, Guid warehouseSysId);

        void UpdateContainer(ContainerDto container);

        List<ContainerDto> GetContainerListByIsActive(Guid warehouseSysId);
    }
}

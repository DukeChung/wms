using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IFrozenAppService : IApplicationService
    {
        Pages<FrozenRequestSkuDto> GetFrozenRequestSkuByPage(FrozenRequestQuery request);

        void SaveFrozenRequest(FrozenRequestDto request);

        void SaveFrozenRequestForOther(FrozenRequestDto request);

        Pages<FrozenListDto> GetFrozenRequestList(FrozenListQuery request);

        void UnFrozenRequest(FrozenRequestDto request);

        Pages<FrozenRequestSkuDto> GetFrozenDetailByPage(FrozenRequestQuery request);

        void UnFrozenRequestForOther(FrozenRequestDto request);
    }
}

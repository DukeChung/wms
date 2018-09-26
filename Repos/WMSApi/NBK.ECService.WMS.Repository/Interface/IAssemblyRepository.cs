using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IAssemblyRepository : ICrudRepository
    {
        Pages<AssemblyListDto> GetAssemblyList(AssemblyQuery assemblyQuery);

        AssemblyViewDto GetAssemblyViewDtoById(Guid sysId);

        Pages<AssemblySkuDto> GetSkuListForAssembly(AssemblySkuQuery query);

        Pages<AssemblyWeightSkuDto> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request);
    }
}

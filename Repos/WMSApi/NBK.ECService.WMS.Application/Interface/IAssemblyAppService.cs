using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IAssemblyAppService : IApplicationService
    {
        Pages<AssemblyListDto> GetAssemblyList(AssemblyQuery assemblyQuery);

        AssemblyViewDto GetAssemblyViewDtoById(Guid sysId, Guid warehouseSysId);

        void UpdateAssemblyStatus(Guid sysId, AssemblyStatus status, int currentUserId, string currentUserName, Guid warehouseSysId);

        void CancelAssemblyPicking(Guid sysId, int currentUserId, string currentUserName, Guid warehouseSysId);

        void FinishAssemblyOrder(AssemblyFinishDto assemblyFinishDto);
        /// <summary>
        /// 根据条件获取加工单
        /// </summary>
        /// <param name="receiptQuery"></param>
        /// <returns></returns>
        AssemblyViewDto GetAssemblyOrderByOrderId(AssemblyQuery assemblyQuery);

        RFCommResult CheckAssemblyOrderNotOnShelves(string assemblyOrder, Guid warehouseSysId);

        Pages<AssemblySkuDto> GetSkuListForAssembly(AssemblySkuQuery query);

        void AddAssembly(AddAssemblyDto request);

        Pages<AssemblyWeightSkuDto> GetWeighSkuListForAssembly(AssemblyWeightSkuQuery request);

        void SaveAssemblySkuWeight(AssemblyWeightSkuRequest request);
    }
}

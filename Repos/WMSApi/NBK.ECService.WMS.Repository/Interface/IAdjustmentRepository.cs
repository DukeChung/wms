using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IAdjustmentRepository : ICrudRepository
    {
        Pages<AdjustmentListDto> GetAdjustmentListByPage(AdjustmentQuery query);

        AdjustmentViewDto GetAdjustmentBySysId(Guid adjustmentSysId);

        List<AdjustmentDetailViewDto> GetAdjustmentDetails(Guid adjustmentSysId,Guid warehouseSysId);

        Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery);
    }
}

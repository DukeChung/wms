using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IAdjustmentAppService
    {
        Pages<AdjustmentListDto> GetAdjustmentListByPage(AdjustmentQuery query);

        AdjustmentViewDto GetAdjustmentBySysId(Guid sysid,Guid warehouseSysId);

        void AddAdjustment(AdjustmentDto adjustment);

        void UpdateAdjustment(AdjustmentDto adjustment);

        void DeleteAjustmentSkus(List<Guid> request, Guid warehouseSysId);

        Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery);

        void Audit(AdjustmentAuditDto dto);

        void Void(AdjustmentAuditDto dto);
    }
}

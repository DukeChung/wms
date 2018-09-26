using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface ISkuBorrowRepository : ICrudRepository
    {
        Pages<SkuBorrowListDto> GetSkuBorrowListByPage(SkuBorrowQuery query);

        SkuBorrowViewDto GetSkuBorrowBySysId(Guid borrowSysId);

        SkuBorrowViewDto GetSkuBorrowByOrder(string borrowOrder);

        List<SkuBorrowDetailViewDto> GetSkuBorrowDetails(Guid skuborrowSysId);

        Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery);
    }
}

using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NBK.ECService.WMS.Application.Interface
{
    public interface ISkuBorrowAppService: IApplicationService
    {  
        Pages<SkuBorrowListDto> GetSkuBorrowListByPage(SkuBorrowQuery query);

        Pages<SkuInvLotLocLpnDto> GetSkuInventoryList(SkuInvLotLocLpnQuery skuQuery);

        void AddSkuBorrow(SkuBorrowDto skuborrow);

        SkuBorrowViewDto GetSkuBorrowBySysId(Guid SysId,Guid WareHouseSysId);

        SkuBorrowViewDto GetSkuBorrowByOrder(string BorrowOrder, Guid WareHouseSysId);

        void UpdateSkuBorrow(SkuBorrowDto adjustment);

        void DeleteSkuBorrowSkus(List<Guid> request); 

        void Audit(SkuBorrowDto dto);

        void Void(SkuBorrowAuditDto dto);
    }
}

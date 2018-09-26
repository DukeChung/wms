using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IFrozenRepository : ICrudRepository
    {
        Pages<FrozenRequestSkuDto> GetFrozenRequestSkuByPage(FrozenRequestQuery request);

        Pages<FrozenRequestSkuDto> GetFrozenDetailByPage(FrozenRequestQuery request);

        Pages<FrozenListDto> GetFrozenRequestList(FrozenListQuery request);

        List<FrozenSkuDto> FilterUsedSkuList(List<Guid> skuList, Guid warehouseSysId);

        List<FrozenSkuDto> GetUsedLocSkuList(List<FrozenLocSkuDto> locSkuList, Guid warehouseSysId);

        void UpdateFrozenQtyBySku(List<Guid> skuList, Guid warehouseSysId, int updateId, string updateName);

        void UpdateFrozenQtyByLocSku(List<FrozenSkuDto> locSkuList, Guid warehouseSysId, int updateId, string updateName);

        void UpdateUnFrozenQtyByLocSku(List<FrozenSkuDto> loclotSkuList, Guid warehouseSysId, int updateId, string updateName);

        void BatchInsertStockFrozenBySku(FrozenRequestDto request, FrozenSource frozenSource);

        void BatchInsertStockFrozenByLocSku(List<FrozenSkuDto> locSkuList, Guid warehouseSysId, int updateId, string updateName, string memo, FrozenSource frozenSource);

        void BatchUnFrozenSku(List<Guid> skuList, Guid warehouseSysId);

        void BatchUpdateStockUnFrozenByLocSku(List<FrozenSkuDto> locSkuList, Guid warehouseSysId, int updateId, string updateName, string memo);

        void UpdateInvForUnFrozenRequest(List<InvLotLocLpnDto> updateInventoryList, int updateId, string updateName);

        List<InvLotLocLpnDto> GetBatchUpdateInventoryForUnFrozenSkuList(List<Guid> skuList, Guid warehouseSysId, int updateId, string updateName);

        List<InvLotLocLpnDto> GetlotloclpnDtoByFrozenSku(List<Guid> skuSysIds, Guid wareHouseSysId);
    }
}

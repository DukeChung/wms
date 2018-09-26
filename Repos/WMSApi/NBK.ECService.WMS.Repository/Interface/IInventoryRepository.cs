using System;
using System.Collections.Generic;
using Abp.EntityFramework;
using NBK.ECService.WMS.DTO.InvLotLocLpn;
using NBK.ECService.WMS.Model.Models;
using NBK.ECService.WMS.Utility.Enum;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IInventoryRepository : ICrudRepository
    {

        /// <summary>
        /// 根据SkuSysId获取lotloclpn 并且按照相关字段排序 
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="pickRule"></param>
        /// <returns></returns>
        List<InvLotLocLpnDto> GetlotloclpnBySkuSysIdOrderByLotDetail(Guid skuSysId, string pickRule, Guid wareHouseSysId, outbound ob, outboundrule outboundRule);

        List<InvLotLocLpnDto> GetlotloclpnBySkuSysIdOrderByLotDetail(List<Guid> skuSysId, Guid wareHouseSysId, outbound ob, outboundrule outboundRule);

        List<InvLotLocLpnDto> GetlotloclpnDto(Guid skuSysId, string lotCode, string locCode, string lpnCode, Guid wareHouseSysId);

        InvLotLocLpnDto GetlotloclpnDto(Guid skuSysId, Guid warehouseSysId, string lotCode, string locCode, string lpnCode);

        Guid Getinvlotloclpn(Guid skuSysId, Guid warehouseSysId, string lotCode, string locCode, string lpnCode);

        List<Guid> Getinvlotloclpn(Guid skuSysId, Guid warehouseSysId, string locCode, string lpnCode);

        Guid Getinvlot(Guid skuSysId, Guid warehouseSysId, string lotCode);
        List<invlot> Getinvlotlist(Guid skuSysId, Guid warehouseSysId);

        Guid Getinvskuloc(Guid skuSysId, Guid warehouseSysId, string locCode);

        /// <summary>
        /// 库存转移根据批次属性查询库存
        /// </summary>
        /// <param name="skuSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <param name="stockTransferDto"></param>
        /// <returns></returns>
        List<InvLotLocLpnDto> GetlotloclpnDtoList(Guid skuSysId, Guid warehouseSysId, StockTransferDto stockTransferDto);

        List<InvLotLocLpnDto> GetlotloclpnDtoByZone(Guid zoneSysId, Guid wareHouseSysId);

        List<InvLotLocLpnDto> GetlotloclpnDtoByZoneLoc(Guid zoneSysId, string loc, Guid wareHouseSysId);
        List<InvLotLocLpnDto> GetlotloclpnDto(List<Guid> skuSysId, Guid warehouseSysId);
    }
}
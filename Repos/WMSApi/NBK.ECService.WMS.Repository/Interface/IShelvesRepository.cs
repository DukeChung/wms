using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IShelvesRepository
    {
        /// <summary>
        /// 获取待上架收货单
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        Pages<RFWaitingShelvesListDto> GetWaitingShelvesList(RFShelvesQuery shelvesQuery);

        /// <summary>
        /// 获取某个单据的待上架商品
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        List<RFWaitingShelvesSkuListDto> GetWaitingShelvesSkuList(RFShelvesQuery shelvesQuery);

        /// <summary>
        /// 获取收货明细推荐货位
        /// </summary>
        /// <param name="shelvesQuery"></param>
        /// <returns></returns>
        string GetAdviceToLoc(RFShelvesQuery shelvesQuery);

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="inventoryQuery"></param>
        /// <returns></returns>
        List<RFInventoryListDto> GetInventoryList(RFInventoryQuery inventoryQuery);

        /// <summary>
        /// 获取待上架加工单列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        Pages<RFAssemblyWaitingShelvesListDto> GetAssemblyWaitingShelvesList(RFAssemblyShelvesQuery assemblyShelvesQuery);

        /// <summary>
        /// 获取加工单待上架商品列表
        /// </summary>
        /// <param name="assemblyShelvesQuery"></param>
        /// <returns></returns>
        List<RFAssemblyWaitingShelvesSkuListDto> GetAssemblyWaitingShelvesSkuList(RFAssemblyShelvesQuery assemblyShelvesQuery);

        /// <summary>
        /// 获取商品货位
        /// </summary>
        /// <param name="skuSysIds"></param>
        /// <returns></returns>
        List<InvSkuLocDto> GetSkuLocBySkuSysIds(List<Guid> skuSysIds, Guid wareHouseSysId, string lotattr01);
    }
}

using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.DTO.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Application.Interface
{
    /// <summary>
    /// 上架接口
    /// </summary>
    public interface IShelvesAppService : IApplicationService
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
        /// 检查商品是否存在于收货明细中
        /// </summary>
        /// <returns></returns>
        RFCommResult CheckReceiptDetailSku(ScanShelvesDto scanShelvesDto);

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
        /// 扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        RFCommResult ScanShelves(ScanShelvesDto scanShelvesDto);

        /// <summary>
        /// 自动上架
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        RFCommResult AutoShelves(ScanShelvesDto scanShelvesDto);

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
        /// 加工单成品上架校验
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        RFCommResult CheckAssemblyWaitShelvesSku(RFAssemblyScanShelvesDto scanShelvesDto);

        /// <summary>
        /// 加工单成品扫描上架
        /// </summary>
        /// <param name="scanShelvesDto"></param>
        /// <returns></returns>
        RFCommResult AssemblyScanShelves(RFAssemblyScanShelvesDto scanShelvesDto);

        /// <summary>
        /// 取消上架
        /// </summary>
        /// <param name="cancelShelvesDto"></param>
        /// <returns></returns>
        CommonResponse CancelShelves(CancelShelvesDto cancelShelvesDto);
    }
}

using Abp.Application.Services;
using NBK.ECService.WMS.DTO;
using System;
using System.Collections.Generic;

namespace NBK.ECService.WMS.Application.Interface
{
    public interface IPrintAppService : IApplicationService
    {
        PrintPickDetailByOrderDto GetPrintPickDetailByOrderDto(string pickDetailOrder, Guid warehouseSysId);

        PrintPickDetailByBatchDto GetPrintPickDetailByBatchDto(string pickDetailOrder, Guid warehouseSysId);

        PrintPickDetailByOrderDto GetPrintRecommendPickDetail(Guid outboundSysId, Guid warehouseSysId);

        PrintReceiptDto GetPrintReceiptDto(string receiptOrder, Guid warehouseSysId);

        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintPurchaseViewDto GetPrintPurchaseViewDto(Guid sysId, Guid warehouseSysId);

        PrintVanningDetailDto GetPrintVanningDetailDto(Guid vanningDetailSysId, string actionType, Guid warehouseSysId);

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintVanningDetailStickDto GetPrintVanningDetailStick(Guid sysId, string actionType, Guid warehouseSysId);

        /// <summary>
        /// 获取箱贴数据ToB
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintVanningDetailStickDto GetPrintVanningDetailStickToB(Guid sysId, string actionType, Guid warehouseSysId);

        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintOutboundDto GetPrintOutboundDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintOutboundPrePackDiffDto GetPrintOutboundPrePackDiffDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 获取打印出库单对应的交接单列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        List<PrintTMSPackNumberListDto> GetPrintTMSPackNumberList(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 获取打印散货箱差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintOutboundPrePackDiffDto GetPrintOutboundPreBulkPackDiffDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblySysId"></param>
        /// <returns></returns>
        PrintAssemblyRCMDPickDetailDto GetPrintAssemblyRCMDPickDetail(Guid assemblySysId, Guid warehouseSysId);

        /// <summary>
        /// 打印收货自动上架单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        PrintAutoShelvesDto GetPrintReceiptAutoShelves(Guid receiptSysId, Guid warehouseSysId);

        /// <summary>
        /// 打印盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintStockTakeDto GetPrintStockTakeDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 打印盘点单汇总报告
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintStockTakeReportDto GetPrintStockTakeReportDto(List<Guid> sysIds, Guid warehouseSysId);

        /// <summary>
        /// 打印质检单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintQualityControlDto GetPrintQualityControlDto(Guid sysId, Guid warehouseSysId);

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        PrintPrebulkPackByBatchDto GetPrintPrebulkPackByBatchDto(List<Guid> sysIds, Guid warehouseSysId);

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <summary>
        PrintPickingMaterialDto GetPrintPickingMaterialDetailList(PrintPickingMaterialQuery request);
    }
}
using System;
using System.Collections.Generic;
using NBK.ECService.WMS.DTO;
using NBK.ECService.WMS.Model.Models;

namespace NBK.ECService.WMS.Repository.Interface
{
    public interface IPrintRepository : ICrudRepository
    {
        List<PrintPickDetailDto> GetPrintPickDetailByOrderDto(string pickDetailOrder);

        PrintReceiptDto GetPrintReceiptDto(string receiptOrder);

        List<PrintReceiptDetailDto> GetPrintReceiptDetailDto(string receiptOrder);

        List<PrintReceiptDetailDto> GetPrintReceiptDetailDtoByPurchaseDetail(string receiptOrder);
        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintPurchaseViewDto GetPrintPurchaseViewDtoBySysId(Guid sysId);

        /// <summary>
        /// 打印采购单明细
        /// </summary>
        /// <param name="purchaseSysId"></param>
        /// <returns></returns>
        List<PrintPurchaseDetailViewDto> GetPrintPurchaseDetailViewBySysId(Guid sysId);

        /// <summary>
        /// 装箱单
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        PrintVanningDetailDto GetPrintVanningDetailDto(Guid vanningDetailSysId);

        /// <summary>
        /// 获取装箱单合计
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<PrintVanningDetailSkuDto> GetPrintVanningSkuList(Guid outboundSysId);

        /// <summary>
        /// 箱贴 每m 3wsz箱中 所有商品信息集合
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        List<PrintVanningDetailSkuDto> GetPrintVanningDetailSkuDtoList(Guid outboundSysId);

        /// <summary>
        /// 箱贴 每m 3wsz箱中 所有商品信息集合
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <returns></returns>
        List<PrintVanningDetailSkuDto> GetPrintVanningDetailSkuDtoListNew(Guid outboundSysId);

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintVanningDetailStickDto GetPrintVanningDetailStick(Guid sysId);

        /// <summary>
        /// 获取箱贴数据ToB
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintVanningDetailStickDto GetPrintVanningDetailStickToB(Guid sysId);

        /// <summary>
        /// 批量打印拣货单
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <returns></returns>
        List<PrintPickDetailDto> GetPrintPickDetailByBatchDtoList(string pickDetailOrder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <returns></returns>
        List<PrintPickDetailDto> GetPrintRecommendPickDetail(Guid outboundSysId, Guid wareHouseSysId);

        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintOutboundDto GetPrintOutboundDto(Guid sysId);

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        PrintOutboundPrePackDiffDto GetPrintOutboundPrePackDiffDto(Guid sysId);

        List<PrintTMSPackNumberListDto> GetPrintTMSPackNumberList(Guid sysId);
        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblyDetailSysId"></param>
        /// <returns></returns>
        List<PrintAssemblyPickDetailDto> GetPrintAssemblyRCMDPickDetail(Guid assemblyDetailSysId, Guid wareHouseSysId);

        /// <summary>
        /// 重载：生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblyDetailSysId"></param>
        /// <param name="wareHouseSysId"></param>
        /// <param name="assemblyrule">加工单规则信息</param>
        /// <param name="channel">渠道</param>
        /// <returns></returns>
        List<PrintAssemblyPickDetailDto> GetPrintAssemblyRCMDPickDetail(Guid assemblyDetailSysId, Guid wareHouseSysId, assemblyrule assemblyrule, string channel);

        /// <summary>
        /// 打印上架单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <returns></returns>
        List<PrintReceiptDetailDto> GetPrintReceiptAutoShelvesDetail(Guid receiptSysId);

        /// <summary>
        /// 打印盘点单明细
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <returns></returns>
        List<PrintStockTakeDetailDto> GetPrintStockTakeDetails(Guid stockTakeSysId);

        /// <summary>
        /// 打印盘点汇总报告明细
        /// </summary>
        /// <param name="stockTakeSysId"></param>
        /// <returns></returns>
        List<PrintStockTakeReportDetailDto> GetPrintStockTakeReportDetails(List<Guid> sysIds);

        /// <summary>
        /// 打印质检单明细
        /// </summary>
        /// <param name="qcSysId"></param>
        /// <returns></returns>
        List<PrintQualityControlDetailDto> GetPrintQualityControlDetails(Guid qcSysId);

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <returns></returns>
        PrintPrebulkPackByBatchDto GetPrintPrebulkPackByBatchDto(List<Guid> sysIds);

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <summary>
        List<PrintPickingMaterialDetailDto> GetPrintPickingMaterialDetailList(PrintPickingMaterialQuery request);
    }
}
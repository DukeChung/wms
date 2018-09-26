using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;
using System;
using NBK.ECService.WMS.Core.WebApi.Filters;
using System.Collections.Generic;

namespace NBK.ECService.WMS.ApiController.PrintControllers
{
    [RoutePrefix("api/Print")]
    [AccessLog]
    public class PrintController : AbpApiController
    {
        private IPrintAppService _printAppService;

        /// <summary>
        /// 系统构造
        /// </summary>
        /// <param name="printAppService"></param>
        public PrintController(IPrintAppService printAppService)
        {
            _printAppService = printAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void PrintAPI()
        {
        }

        /// <summary>
        /// 按订单打印
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintPickDetailByOrderDto")]
        public PrintPickDetailByOrderDto GetPrintPickDetailByOrderDto(string pickDetailOrder, Guid warehouseSysId)
        {
            return _printAppService.GetPrintPickDetailByOrderDto(pickDetailOrder, warehouseSysId);
        }

        /// <summary>
        /// 按贱货单号打印
        /// </summary>
        /// <param name="pickDetailOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintPickDetailByBatchDto")]
        public PrintPickDetailByBatchDto GetPrintPickDetailByBatchDto(string pickDetailOrder, Guid warehouseSysId)
        {
            return _printAppService.GetPrintPickDetailByBatchDto(pickDetailOrder, warehouseSysId);
        }

        /// <summary>
        /// 获取推荐货位 打印出库单列表
        /// </summary>
        /// <param name="outboundSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintRecommendPickDetail")]
        public PrintPickDetailByOrderDto GetPrintRecommendPickDetail(Guid outboundSysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintRecommendPickDetail(outboundSysId, warehouseSysId);
        }

        /// <summary>
        /// 收货单打印
        /// </summary>
        /// <param name="receiptOrder"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintReceiptDto")]
        public PrintReceiptDto GetPrintReceiptDto(string receiptOrder, Guid warehouseSysId)
        {
            return _printAppService.GetPrintReceiptDto(receiptOrder, warehouseSysId);
        }

        /// <summary>
        /// 打印采购单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintPurchaseViewDto")]
        public PrintPurchaseViewDto GetPrintPurchaseViewDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintPurchaseViewDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 打印箱贴
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <param name="actionType"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintVanningDetailDto")]
        public PrintVanningDetailDto GetPrintVanningDetailDto(Guid vanningDetailSysId, string actionType, Guid warehouseSysId)
        {
            return _printAppService.GetPrintVanningDetailDto(vanningDetailSysId, actionType, warehouseSysId);
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <param name="actionType"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintVanningDetailStick")]
        public PrintVanningDetailStickDto GetPrintVanningDetailStick(Guid vanningDetailSysId, string actionType, Guid warehouseSysId)
        {
            return _printAppService.GetPrintVanningDetailStick(vanningDetailSysId, actionType, warehouseSysId);
        }

        /// <summary>
        /// 获取箱贴数据
        /// </summary>
        /// <param name="vanningDetailSysId"></param>
        /// <param name="actionType"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintVanningDetailStickToB")]
        public PrintVanningDetailStickDto GetPrintVanningDetailStickToB(Guid vanningDetailSysId, string actionType, Guid warehouseSysId)
        {
            return _printAppService.GetPrintVanningDetailStickToB(vanningDetailSysId, actionType, warehouseSysId);
        }

        /// <summary>
        /// 获取打印出库单数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintOutboundDto")]
        public PrintOutboundDto GetPrintOutboundDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintOutboundDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 获取打印预包装差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintOutboundPrePackDiffDto")]
        public PrintOutboundPrePackDiffDto GetPrintOutboundPrePackDiffDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintOutboundPrePackDiffDto(sysId, warehouseSysId);
        }


        /// <summary>
        /// 获取打印出库单对应的交接单列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintTMSPackNumberList")]
        public List<PrintTMSPackNumberListDto> GetPrintTMSPackNumberList(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintTMSPackNumberList(sysId, warehouseSysId);
        }



        /// <summary>
        /// 获取打印散货箱差异数据
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintOutboundPreBulkPackDiffDto")]
        public PrintOutboundPrePackDiffDto GetPrintOutboundPreBulkPackDiffDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintOutboundPreBulkPackDiffDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 生产加工单推荐拣货货位
        /// </summary>
        /// <param name="assemblySysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintAssemblyRCMDPickDetail")]
        public PrintAssemblyRCMDPickDetailDto GetPrintAssemblyRCMDPickDetail(Guid assemblySysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintAssemblyRCMDPickDetail(assemblySysId, warehouseSysId);
        }

        /// <summary>
        /// 打印收货自动上架单
        /// </summary>
        /// <param name="receiptSysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintReceiptAutoShelves")]
        public PrintAutoShelvesDto GetPrintReceiptAutoShelves(Guid receiptSysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintReceiptAutoShelves(receiptSysId, warehouseSysId);
        }

        /// <summary>
        /// 打印盘点单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintStockTakeDto")]
        public PrintStockTakeDto GetPrintStockTakeDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintStockTakeDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 打印盘点单汇总报告
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrintStockTakeReportDto")]
        public PrintStockTakeReportDto GetPrintStockTakeReportDto(List<Guid> sysIds, Guid warehouseSysId)
        {
            return _printAppService.GetPrintStockTakeReportDto(sysIds, warehouseSysId);
        }

        /// <summary>
        /// 打印质检单
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrintQualityControlDto")]
        public PrintQualityControlDto GetPrintQualityControlDto(Guid sysId, Guid warehouseSysId)
        {
            return _printAppService.GetPrintQualityControlDto(sysId, warehouseSysId);
        }

        /// <summary>
        /// 批量打印散货封箱单
        /// </summary>
        /// <param name="sysIds"></param>
        /// <param name="warehouseSysId"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrintPrebulkPackByBatchDto")]
        public PrintPrebulkPackByBatchDto GetPrintPrebulkPackByBatchDto(List<Guid> sysIds, Guid warehouseSysId)
        {
            return _printAppService.GetPrintPrebulkPackByBatchDto(sysIds, warehouseSysId);
        }

        /// <summary>
        /// 获取打印领料数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPrintPickingMaterialDetailList")]
        public PrintPickingMaterialDto GetPrintPickingMaterialDetailList(PrintPickingMaterialQuery request)
        {
            return _printAppService.GetPrintPickingMaterialDetailList(request);
        }



    }
}
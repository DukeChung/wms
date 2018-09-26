using System;
using System.Collections.Generic;
using System.Web.Http;
using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Chart;

namespace NBK.ECService.WMSReport.ApiController
{
    [RoutePrefix("api/Chart")]
    public class ChartController: AbpApiController
    {
        private IChartAppService _chartAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chartAppService"></param>
        public ChartController(IChartAppService chartAppService)
        {
            _chartAppService = chartAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void ChartAPI() { }


        /// <summary>
        /// 获取入库出库订单占比
        /// </summary>
        [HttpPost, Route("GetPurchaseAndOutboundChart")]
        public PurchaseAndOutboundChartDto GetPurchaseAndOutboundChart(Guid wareHouseSysId)
        {
            return _chartAppService.GetPurchaseAndOutboundChart(wareHouseSysId);
        }

        /// <summary>
        /// 获取临期产品
        /// </summary>
        [HttpPost, Route("GetAdventSkuChartTop5")]
        public List<AdventSkuChartDto> GetAdventSkuChartTop5(Guid wareHouseSysId)
        {
            return _chartAppService.GetAdventSkuChartTop5(wareHouseSysId);
        }

        /// <summary>
        /// 过去十天收发货商品数量
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetSkuReceiptOutboundChartAfter10")]
        public List<SkuReceiptOutboundReportDto> GetSkuReceiptOutboundChartAfter10(Guid wareHouseSysId)
        {
            return _chartAppService.GetSkuReceiptOutboundChartAfter10(wareHouseSysId);
        }

        /// <summary>
        /// 获取过去十天订单与退货数量
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundAndReturnCharDataOfLastTenDays")]
        public List<OutboundAndReturnCharDto> GetOutboundAndReturnCharDataOfLastTenDays(Guid wareHouseSysId)
        {
            return _chartAppService.GetOutboundAndReturnCharDataOfLastTenDays(wareHouseSysId);
        }

        /// <summary>
        /// 首页预包装看板
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPrePackBoardTop12")]
        public List<PrePackBoardDto> GetPrePackBoardTop12(Guid wareHouseSysId)
        {
            return _chartAppService.GetPrePackBoardTop12(wareHouseSysId);
        }

        /// <summary>
        /// 首页超过三天未收货的入库单6条
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetExceedThreeDaysPurchase")]
        public List<ExceedThreeDaysPurchase> GetExceedThreeDaysPurchase(Guid wareHouseSysId)
        {
            return _chartAppService.GetExceedThreeDaysPurchase(wareHouseSysId);
        }


        /// <summary>
        /// 首页获取当天订单数
        /// </summary>
        /// <param name="wareHouseSysId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetToDayOrderStatusTotal")]
        public OutboundTotalChartDto GetToDayOrderStatusTotal(Guid wareHouseSysId)
        {
            return _chartAppService.GetToDayOrderStatusTotal(wareHouseSysId);
        }



    }
}
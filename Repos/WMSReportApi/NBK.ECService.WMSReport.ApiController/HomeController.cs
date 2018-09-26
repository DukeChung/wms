using Abp.WebApi.Controllers;
using NBK.ECService.WMSReport.Application.Interface;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.DTO.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBK.ECService.WMSReport.ApiController
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/Home")]
    public class HomeController : AbpApiController
    {
        private IHomeAppService _homeAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="homeAppService"></param>
        public HomeController(IHomeAppService homeAppService)
        {
            _homeAppService = homeAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void HomeAPI() { }

        /// <summary>
        /// 获取采购入库/B2B出库/B2C出库数据
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetSparkLineSummaryDto")]
        public SparkLineSummaryDto GetSparkLineSummaryDto(bool flag)
        {
            return _homeAppService.GetSparkLineSummaryDto(flag);
        }

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetPurchaseTypePieDto")]
        public ReturnPurchaseTypePieDto GetPurchaseTypePieDto(bool flag)
        {
            return _homeAppService.GetPurchaseTypePieDto(flag);
        }

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundTypePieDto")]
        public ReturnOutboundTypePieDto GetOutboundTypePieDto(bool flag)
        {
            return _homeAppService.GetOutboundTypePieDto(flag);
        }

        /// <summary>
        /// 全局一年之内收发存
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetStockInOutData")]
        public List<StockInOutListDto> GetStockInOutData(bool flag)
        {
            return _homeAppService.GetStockInOutData(flag);
        }

        /// <summary>
        /// 获取所有仓库总收货分布
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetWareHouseReceiptQtyList")]
        public List<WareHouseQtyDto> GetWareHouseReceiptQtyList(bool flag)
        {
            return _homeAppService.GetWareHouseReceiptQtyList(flag);
        }

        /// <summary>
        /// 获取所有仓库总出库分布
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetWareHouseOutboundQtyList")]
        public List<WareHouseQtyDto> GetWareHouseOutboundQtyList(bool flag)
        {
            return _homeAppService.GetWareHouseOutboundQtyList(flag);
        }

        /// <summary>
        /// 获取所有仓库剩余库存
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetWareHouseQtyList")]
        public List<WareHouseQtyDto> GetWareHouseQtyList(bool flag)
        {
            return _homeAppService.GetWareHouseQtyList(flag);
        }


        /// <summary>
        /// 仓库出库，库存库龄占比
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetStockAgeGroup")]
        public StockAgeGroupDto GetStockAgeGroup(bool flag)
        {
            return _homeAppService.GetStockAgeGroup(flag);
        }

        /// <summary>
        /// 获取畅销商品Top10
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuSellingTop10")]
        public List<SkuSaleQtyDto> GetSkuSellingTop10(bool flag)
        {
            return _homeAppService.GetSkuSellingTop10(flag);
        }

        /// <summary>
        /// 获取滞销商品Top10
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetSkuUnsalableTop10")]
        public List<SkuSaleQtyDto> GetSkuUnsalableTop10(bool flag)
        {
            return _homeAppService.GetSkuUnsalableTop10(flag);
        }

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetServiceStationOutboundTopTen")]
        public List<ServiceStationOutboundDto> GetServiceStationOutboundTopTen(bool flag)
        {
            return _homeAppService.GetServiceStationOutboundTopTen(flag);
        }

        /// <summary>
        /// 获取渠道库存
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetChannelPieData")]
        public List<ChannelQtyDto> GetChannelPieData(bool flag)
        {
            return _homeAppService.GetChannelPieData(flag);
        }


        /// <summary>
        /// 获取最新10个退货入库收货完成的单子
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetReturnPurchase")]
        public List<ReturnPurchaseDto> GetReturnPurchase(bool flag)
        {
            return _homeAppService.GetReturnPurchase(flag);
        }

        /// <summary>
        /// 获取所有需要更新经纬度的出库单总条数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetOutboundNoLngLatCount")]
        public int GetOutboundNoLngLatCount()
        {
            return _homeAppService.GetOutboundNoLngLatCount();
        }

        /// <summary>
        /// 批量处理没有经纬度的出库单
        /// </summary>
        /// <param name="pageCount">页面大小</param>
        /// <returns></returns>
        [HttpGet, Route("UpdataOutboundLngLat")]
        public bool UpdataOutboundLngLat(int pageCount)
        {
            return _homeAppService.UpdataOutboundLngLat(pageCount);
        }

        /// <summary>
        /// 获取仓库业务分部
        /// </summary>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [HttpGet, Route("GetHistoryServiceStationData")]
        public List<OutboundMapData> GetHistoryServiceStationData(int page)
        {
            return _homeAppService.GetHistoryServiceStationData(page);
        }

        /// <summary>
        /// 获取仓库城市出库关系
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetWarehouseStationRelation")]
        public List<WarehouseStationRelationDto> GetWarehouseStationRelation()
        {
            return _homeAppService.GetWarehouseStationRelation();
        }

        /// <summary>
        ///  仓库日历订单统计 每天出入库，和预计出入库的数量
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet, Route("GetDailyEventDataCountInfo")]
        public EventDataCountReportDto GetDailyEventDataCountInfo(bool flag, string startDate, string endDate)
        {
            return _homeAppService.GetDailyEventDataCountInfo(flag, startDate, endDate);
        }

        /// <summary>
        /// 获取某一天的所有仓库出入库数据
        /// </summary>
        /// <param name="date">日期字符串</param>
        /// <returns></returns>
        [HttpGet, Route("GetCalendarDataByDate")]
        public List<CalendarDataByDateDto> GetCalendarDataByDate(string date)
        {
            return _homeAppService.GetCalendarDataByDate(date);
        }

        /// <summary>
        /// 仓库作业时间分布图
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkDistributionData")]
        public List<WorkDistributionListData> GetWorkDistributionData(bool flag)
        {
            return _homeAppService.GetWorkDistributionData(flag);
        }

        /// <summary>
        /// 根据仓库获业务类型获取作业时间分布
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <param name="sysId">仓库ID</param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkDistributionByWarehouse")]
        public List<WorkDistributionDataByWarehouseDto> GetWorkDistributionByWarehouse(bool flag, Guid sysId)
        {
            return _homeAppService.GetWorkDistributionByWarehouse(flag, sysId);
        }

        /// <summary>
        /// 仓库作业类型占比
        /// </summary>
        /// <param name="flag">是否更新缓存：portal默认false，Job默认true</param>
        /// <returns></returns>
        [HttpGet, Route("GetWorkDistributionPieData")]
        public List<WorkDistributionPieDto> GetWorkDistributionPieData(bool flag)
        {
            return _homeAppService.GetWorkDistributionPieData(flag);
        }
    }
}

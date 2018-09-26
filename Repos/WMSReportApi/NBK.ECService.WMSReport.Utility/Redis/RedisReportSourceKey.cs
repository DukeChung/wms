using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Redis
{
    /// <summary>
    /// 报表缓存Key
    /// </summary>
    public static class RedisReportSourceKey
    {
        /// <summary>
        /// 入库，出库各类型环比
        /// </summary>
        public static string SparkLineSummaryDtoKey = "Report_SparkLineSummaryDtoKey";

        /// <summary>
        /// 顶部入库单据类型占比
        /// </summary>
        public static string PurchaseTypePieDto = "Report_PurchaseTypePieDto";

        /// <summary>
        /// 顶部出库单据类型占比
        /// </summary>
        public static string OutboundTypePieDto = "Report_OutboundTypePieDto";

        /// <summary>
        /// 全局一年之内收发存
        /// </summary>
        public static string StockInOutData = "Report_StockInOutData";

        /// <summary>
        /// 获取所有仓库总收货分布
        /// </summary>
        public static string WareHouseReceiptQtyList = "Report_WareHouseReceiptQtyList";

        /// <summary>
        /// 业务接口访问统计
        /// </summary>
        public static string AccessBizMappingList = "Report_AccessBizMappingList";

        /// <summary>
        /// 获取所有仓库总出库分布
        /// </summary>
        public static string WareHouseOutboundQtyList = "Report_WareHouseOutboundQtyList";

        /// <summary>
        /// 获取所有仓库总库存
        /// </summary>
        public static string WareHouseQtyList = "Report_WareHouseQtyList";

        /// <summary>
        /// 仓库出库，库存库龄占比
        /// </summary>
        public static string StockAgeGroup = "Report_StockAgeGroup";

        /// <summary>
        /// 获取畅销商品Top10
        /// </summary>
        public static string SkuSellingTop10 = "Report_SkuSellingTop10";

        /// <summary>
        /// 获取滞销商品Top10
        /// </summary>
        public static string SkuUnsalableTop10 = "Report_SkuUnsalableTop10";

        /// <summary>
        /// 获取服务站发货Top10
        /// </summary>
        public static string ServiceStationOutboundTopTen = "Report_ServiceStationOutboundTopTen";

        /// <summary>
        /// 获取渠道库存
        /// </summary>
        public static string ChannelPieData = "Report_ChannelPieData";

        /// <summary>
        /// 获取最新10个退货入库收货完成的单子
        /// </summary>
        public static string ReturnPurchase = "Report_ReturnPurchase";

        /// <summary>
        /// 菜单缓存
        /// </summary>
        public static string RedisReportMenuList = "ReportMenuList";

        /// <summary>
        /// 用户缓存
        /// </summary>
        public static string RedisReportLoginUserList = "ReportLoginUserList";

        /// <summary>
        /// 仓库日历订单缓存
        /// </summary>
        public static string DailyEventDataCountInfo = "DailyEventDataCountInfo";

        /// <summary>
        /// 8个环形库存分布图
        /// </summary>
        public static string FertilizerInvPieList = "FertilizerInvPieList|{0}";

        /// <summary>
        /// 化肥库存蜘蛛网图
        /// </summary>
        public static string FertilizerInvRadarList = "FertilizerInvRadarList";

        /// <summary>
        /// 化肥出入库蜘蛛网图
        /// </summary>
        public static string FertilizerRORadarList = "FertilizerRORadarList";

        /// <summary>
        /// 所有仓库作业时间分布图
        /// </summary>
        public static string WorkDistributionData = "WorkDistributionData";

        /// <summary>
        /// 仓库作业占比缓存Key
        /// </summary>
        public static string WorkDistributionPie = "WorkDistributionPie";

        /// <summary>
        /// 具体仓库作业类型时间统计
        /// </summary>
        public static string WorkDistributionByWarehouse = "WareHouseQtyData|{0}";
    }
}

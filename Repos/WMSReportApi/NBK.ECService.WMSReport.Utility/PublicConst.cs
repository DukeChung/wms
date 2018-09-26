using System.Configuration;

namespace NBK.ECService.WMSReport.Utility
{
    public class PublicConst
    {
        public static string WmsReportApiUrl = ConfigurationManager.AppSettings["WMSREPORTAPIURL"];
        public static string WmsLogApiUrl = ConfigurationManager.AppSettings["WMSLOGAPIURL"];
        public static string WmsPortalUrl = ConfigurationManager.AppSettings["WMSPortalUrl"];

        public static string RedisWMSHostName = ConfigurationManager.AppSettings["RedisWMSHostName"];
        public static string WmsApiUrl = ConfigurationManager.AppSettings["WMSAPIURL"];

        public static string nbk_wms_LogContext = ConfigurationManager.ConnectionStrings["nbk_wms_LogContext"] == null ? string.Empty
            : ConfigurationManager.ConnectionStrings["nbk_wms_LogContext"].ConnectionString;



        public static string nbk_wms_GlobalContext = ConfigurationManager.ConnectionStrings["nbk_wms_GlobalContext"] == null ? string.Empty
            : ConfigurationManager.ConnectionStrings["nbk_wms_GlobalContext"].ConnectionString;

        public const string NotInbound = "未入库";
        public const string NotLimit = "未限制";
        public const string StartDateFormat = "yyyy-MM-dd 00:00:00";
        public const string EndDateFormat = "yyyy-MM-dd 23:59:00";
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateFormat1 = "yyyy/MM/dd";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 损益级别
        /// </summary>
        public const string AdjustmentLevel = "AdjustmentLevel";

        /// <summary>
        /// 临期控制天数
        /// </summary>
        public const string SysCodeTypeShelfLifeWarning = "ShelfLifeWarning";

        /// <summary>
        /// 上传导出excel 到文件服务器
        /// </summary>
        public const string ReportFile = "ReportFile";

        #region 状态
        #region 入库状态
        public const string ReceiptInit = "初始";
        public const string ReceiptNew = "新增";
        public const string ReceiptPrint = "已经打印";
        public const string ReceiptReceiving = "收货中";
        public const string ReceiptReceived = "收货完成";
        public const string ReceiptCancel = "取消";
        #endregion
        #region 入库明细状态
        #region 入库状态
        public const string ReceiptDetailInit = "初始";
        public const string ReceiptDetailNew = "新增";
        public const string ReceiptDetailReceiving = "收货中";
        public const string ReceiptDetailReceived = "收货完成";
        public const string ReceiptDetailCancel = "取消";
        #endregion
        #endregion
        #region 采购状态
        public const string PurchaseNew = "待入库";
        public const string PurchaseInReceipt = "收货中";
        public const string PurchasePartReceipt = "部分入库";
        public const string PurchaseFinish = "已入库";
        public const string PurchaseStopReceipt = "终止入库";
        public const string PurchaseVoid = "作废";
        public const string PurchaseClose = "关闭";
        #endregion
        #region 拣货状态
        public const string PickDetailNew = "新建";
        public const string PickDetailFinish = "拣货完成";
        public const string PickDetailCancel = "取消拣货";
        #endregion
        #region 出库
        public const string OutboundStatusNew = "新建";
        public const string OutboundStatusPartAllocation = "部分分配";
        public const string OutboundStatusAllocation = "分配完成";
        public const string OutboundStatusPartPick = "部分拣货";
        public const string OutboundStatusPicking = "拣货完成";
        public const string OutboundStatusDelivery = "出库完成";
        public const string OutboundStatusCancel = "作废";

        #endregion

        #region 移仓单
        public const string TransferInventoryNew = "新建";
        public const string TransferInventoryDelivery = "出库完成";
        public const string TransferInventoryReceiptFinish = "收货完成";
        #endregion

        #region 预包装
        public const string PrePackNew = "新建";
        public const string PrePackFinish = "完成";
        public const string PrePackCancel = "作废";
        #endregion

        #endregion

        #region  类型
        #region 出库
        public const string OutboundTypeNormal = "正常";
        public const string OutboundTypeB2C = "B2C";
        public const string OutboundTypeB2B = "B2B";
        public const string OutboundTypeReturn = "退货出库";
        public const string OutboundTypeFIFO = "快进快出";
        public const string OutboundTypeTransferInventory = "移仓出库";
        public const string OutboundTypeMaterial = "原材料出库";
        #endregion
        #region 采购
        public const string PurchaseTypePurchase = "采购入库";
        public const string PurchaseTypeReturn = "采购退货";
        public const string PurchaseTypeFiFo = "快进快出";
        public const string PurchaseTypeMaterial = "原材料入库";
        public const string PurchaseTypeTransferInventory = "移仓入库";
        #endregion

        #endregion

        #region 拣货 
        /// <summary>
        /// 后生产先出库
        /// </summary>
        public const string PickRuleLO = "LastProductionOutbound";

        /// <summary>
        /// 先生产先出库
        /// </summary>
        public const string PickRuleFO = "FirstProductionOutbound";

        /// <summary>
        /// 按单拣货
        /// </summary>
        public const string PickTypeByOrder = "ByOrder";
        /// <summary>
        /// 批量拣货
        /// </summary>
        public const string PickTypeByBatch = "ByOrder";


        #endregion

        #region 装箱
        public const string VanningStatusNew = "新建";
        public const string VanningStatusVanning = "装箱中";
        public const string VanningStatusFinish = "装箱完成";
        public const string VanningStatusCancel = "取消";
        public const string VanningTypeOrder = "订单装箱";


        #endregion

        #region 损益

        public const string AdjustmentStatusNew = "新建";

        public const string AdjustmentStatusAudit = "已审核";

        public const string AdjustmentStatusVoid = "已作废";

        public const string AdjustmentTypeProfiAndLoss = "损益";

        /// <summary>
        /// 损益级别默认值，包装破损，由系统代码管理配置
        /// </summary>
        public const string AdjustmentLevelCodeDefault = "1";
        #endregion


        #region  百度地图
        public static string GeocoderURL = ConfigurationManager.AppSettings["GeocoderURL"];
        public static string GeocoderAK = ConfigurationManager.AppSettings["GeocoderAK"];
        #endregion

        #region Excel导出相关

        public const int EachSheetDataRowsCount = 50000;

        public const int EachQuestDataRowsCount = 20000;

        public const int EachExportDataMaxCount = 100000;

        #endregion
    }
}
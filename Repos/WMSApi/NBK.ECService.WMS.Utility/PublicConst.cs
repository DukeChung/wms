//  

using System.Configuration;
using NBK.ECService.WMS.Utility.RabbitMQ;
using System;

namespace NBK.ECService.WMS.Utility
{
    public static class PublicConst
    {
        public static string WMSReportUrl = ConfigurationManager.AppSettings["WMSReportUrl"];
        public static string WmsApiUrl = ConfigurationManager.AppSettings["WMSAPIURL"];
        public static string WmsLogApiUrl = ConfigurationManager.AppSettings["WMSLogAPIURL"];
        public static string GZNBBaseUrl = ConfigurationManager.AppSettings["GZNBBaseUrl"];
        public static string ERPBaseUrl = ConfigurationManager.AppSettings["ERPBaseUrl"];
        public static string B2CBaseUrl = ConfigurationManager.AppSettings["B2CBaseUrl"];
        public static string TMSBaseUrl = ConfigurationManager.AppSettings["TMSBaseUrl"];
        public static string SingalRMessageWMSHostName = ConfigurationManager.AppSettings["SingalRMessageWMSHostName"];

        //基础数据同步
        public static string WmsBizApiUrl = ConfigurationManager.AppSettings["WMSBizAPIURL"];
        public static bool SyncMultiWHSwitch = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncMultiWHSwitch"]);

        public static bool IsAsyncECCBussinessByMQ
        {
            get
            {
                var config = ConfigurationManager.AppSettings["IsAsyncECCBussinessByMQ"];
                if (string.IsNullOrEmpty(config))
                {
                    return true;
                }
                return config.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
            }
        }

        #region 缓存队列
        /// <summary>
        /// 请求服务地址
        /// </summary>
        public static string RedisWMSHostName = ConfigurationManager.AppSettings["RedisWMSHostName"];
        //public static string RedisWMSPort = ConfigurationManager.AppSettings["RedisWMSPort"];
        //public static string RedisWMSPassword = ConfigurationManager.AppSettings["RedisWMSPassword"];
        #endregion

        #region 消息队列

        /// <summary>
        /// 请求服务地址
        /// </summary>
        public static string RabbitWMSHostName = ConfigurationManager.AppSettings["RabbitWMSHostName"];


        /// <summary>
        /// 请求服务用户名
        /// </summary>
        public static string RabbitWMSUserName = ConfigurationManager.AppSettings["RabbitWMSUserName"];

        /// <summary>
        /// 请求服务密码
        /// </summary>
        public static string RabbitWMSPassword = ConfigurationManager.AppSettings["RabbitWMSPassword"];

        /// <summary>
        /// 请求服务端口
        /// </summary>
        public static string RabbitWMSPort = ConfigurationManager.AppSettings["RabbitWMSPort"];

        public static string Exchange = "WMS_MQ_Business";

        #endregion

        #region  数据来源

        public static string ThirdPartySourceERP = "ERP";
        public static string ThirdPartySourceGZNB = "GZNB";

        /// <summary>
        /// 删除数据
        /// </summary>
        public static string ThirdPartyActionDelete = "Delete";
        /// <summary>
        /// 新增数据
        /// </summary>
        public static string ThirdPartyActionInsert = "Insert";
        /// <summary>
        /// 更新数据
        /// </summary>
        public static string ThirdPartyActionUpdate = "Update";



        #endregion

        #region  公共
        /// <summary>
        /// 昆明邮编
        /// </summary>
        public const string PostCode = "650000";
        public const string ReceiptSource = "WMS";
        public const string SortingDirectionAsc = " ASC";
        public const string SortingDirectionDesc = " Desc";
        public const string Yes = "Y";
        public const string IsActiveTrue = "是";
        public const string IsActiveFalse = "否";
        public const string NotInbound = "未入库";
        public const string NotLimit = "未限制";
        public const string StartDateFormat = "yyyy-MM-dd 00:00:00";
        public const string EndDateFormat = "yyyy-MM-dd 23:59:00";
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateFormat1 = "yyyy/MM/dd";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string WareHouseSysId = ConfigurationManager.AppSettings["WareHouseSysId"];
        public const string VanningActionType = "Finish";
        public static string FIFOLoc = ConfigurationManager.AppSettings["FIFOLoc"];
        public const string ReturnVendorSysId = "92558e81-24c1-11e7-9dfe-005056bd5942";
        /// <summary>
        /// 领料分拣货位
        /// </summary>
        public const string PickingSkuLoc = "PickingSkuLoc";

        /// <summary>
        /// 生成的单号数量
        /// </summary>
        public const int SupOrderNumberCount = 2000;

        /// <summary>
        /// 少于多少数量的时候开始补充单号
        /// </summary>
        public const int RemainOrderNumberCount = 300;
        #endregion

        #region SysCodeType

        /// <summary>
        /// 拣货规则
        /// </summary>
        public const string SysCodeTypePickRule = "PickRule";

        /// <summary>
        /// 临期控制天数
        /// </summary>
        public const string SysCodeTypeShelfLifeWarning = "ShelfLifeWarning";

        /// <summary>
        /// 收发货邮件提醒
        /// </summary>
        public const string SysCodeTypeReceiptOutboundMail = "ReceiptOutboundMail";

        /// <summary>
        /// 每次访问API时，记录accesslog时生成的sysid，用于关联记录 bussinesslog
        /// </summary>
        public const string AccessLogSysId = "AccessLogSysId";

        /// <summary>
        /// 损益级别
        /// </summary>
        public const string AdjustmentLevel = "AdjustmentLevel";

        /// <summary>
        /// 货位用途
        /// </summary>
        public const string SysCodeLocUsage = "LocUsage";

        #endregion

        #region 单号生成规则

        /// <summary>
        /// 采购单号
        /// </summary>
        public const string GenNextNumberPurchase = "Purchase";
        /// <summary>
        /// 入库单号
        /// </summary>
        public const string GenNextNumberReceipt = "Receipt";
        /// <summary>
        /// 商品SN
        /// </summary>
        public const string GenNextNumberReceiptSn = "ReceiptSN";
        /// <summary>
        /// 拣货单号
        /// </summary>
        public const string GenNextNumberPickDetail = "PickDetail";
        /// <summary>
        /// 批次号
        /// </summary>
        public const string GenNextNumberLot = "Lot";
        /// <summary>
        /// 批量批次号
        /// </summary>
        public const string GenNextNumberBatchLot = "BatchLot";
        /// <summary>
        /// 商品
        /// </summary>
        public const string GenNextNumberSku = "Sku";
        /// <summary>
        /// 出库单号
        /// </summary>
        public const string GenNextNumberOutbound = "Outbound";
        /// <summary>
        /// 装箱单
        /// </summary>
        public const string GenNextNumberVanning = "Vanning";

        /// <summary>
        /// 损益单
        /// </summary>
        public const string GenNextNumberAdjustment = "Adjustment";
        /// <summary>
        /// 盘点单
        /// </summary>
        public const string GenNextNumberStockTake = "StockTake";

        /// <summary>
        /// 库存批次转移单
        /// </summary>
        public const string GenNextNumberStockTransfer = "StockTransfer";
        /// <summary>
        /// 交接单
        /// </summary>
        public const string GenNextNumberHandoverGroup = "HandoverGroup";
        /// <summary>
        /// 库存移动单
        /// </summary>
        public const string GenNextNumberStockMovement = "StockMovement";
        /// <summary>
        /// 组装单
        /// </summary>
        public const string GenNextNumberAssembly = "Assembly";

        /// <summary>
        /// 移仓单
        /// </summary>
        public const string GenNextNumberTransferInventory = "TransferInventory";

        /// <summary>
        /// 预包装单号
        /// </summary>
        public const string GenNextNumberPrePack = "PrePack";

        /// <summary>
        /// 
        /// </summary>
        public const string GenNextNumberPreBulkPack = "PreBulkPack";

        /// <summary>
        /// 质检单号
        /// </summary>
        public const string GenNextNumberQC = "QualityControl";

        /// <summary>
        /// 商品外借单
        /// </summary>
        public const string GenNextNumberSkuBorrow = "SkuBorrow";

        /// <summary>
        /// 工单号
        /// </summary>
        public const string GenNextNumberWork = "WorkManger";

        /// <summary>
        /// 领料分拣单
        /// </summary>
        public const string GenNextNumberPicking = "Picking";

        #endregion

        #region 单号前缀
        /// <summary>
        /// 采购单号
        /// </summary>
        public const string AlphaPrefixPurchase = "PO";

        /// <summary>
        /// 入库单号
        /// </summary>
        public const string AlphaPrefixReceipt = "GR";

        /// <summary>
        /// 商品SN
        /// </summary>
        public const string AlphaPrefixReceiptSn = "";

        /// <summary>
        /// 拣货单号
        /// </summary>
        public const string AlphaPrefixPickDetail = "OP";

        /// <summary>
        /// 批次号
        /// </summary>
        public const string AlphaPrefixLot = "GZ";

        /// <summary>
        /// 批量批次号
        /// </summary>
        public const string AlphaPrefixBatchLot = "BL";

        /// <summary>
        /// 商品
        /// </summary>
        public const string AlphaPrefixSku = "58";

        /// <summary>
        /// 出库单号
        /// </summary>
        public const string AlphaPrefixOutbound = "OB";

        /// <summary>
        /// 装箱单
        /// </summary>
        public const string AlphaPrefixVanning = "PN";

        /// <summary>
        /// 损益单
        /// </summary>
        public const string AlphaPrefixAdjustment = "PL";

        /// <summary>
        /// 盘点单
        /// </summary>
        public const string AlphaPrefixStockTake = "SC";

        /// <summary>
        /// 库存批次转移单
        /// </summary>
        public const string AlphaPrefixStockTransfer = "ST";

        /// <summary>
        /// 交接单
        /// </summary>
        public const string AlphaPrefixHandoverGroup = "PS";

        /// <summary>
        /// 库存移动单
        /// </summary>
        public const string AlphaPrefixStockMovement = "SM";

        /// <summary>
        /// 组装单
        /// </summary>
        public const string AlphaPrefixAssembly = "BZ";

        /// <summary>
        /// 移仓单
        /// </summary>
        public const string AlphaPrefixTransferInventory = "TO";

        /// <summary>
        /// 预包装单号
        /// </summary>
        public const string AlphaPrefixPrePack = "PP";

        /// <summary>
        /// 
        /// </summary>
        public const string AlphaPrefixPreBulkPack = "PB";

        /// <summary>
        /// 质检单号
        /// </summary>
        public const string AlphaPrefixQC = "QC";

        /// <summary>
        /// 商品外借单
        /// </summary>
        public const string AlphaPrefixSkuBorrow = "BO";

        /// <summary>
        /// 工单号
        /// </summary>
        public const string AlphaPrefixWorkManger = "WO";

        /// <summary>
        /// 领料分拣单号
        /// </summary>
        public const string AlphaPrefixPicking = "PM";
        #endregion

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

        #region 调整SourceType
        /// <summary>
        /// 调整状态 盘点
        /// </summary>
        public const string AJSourceTypeStockTake = "StockTake";

        /// <summary>
        /// 质检
        /// </summary>
        public const string AJSourceTypeQC = "QualityControl";
        #endregion

        #region 默认拣货 上架货位

        /// <summary>
        /// 上架默认货位
        /// </summary>
        public const string InShelfLoc = "Stage";
        /// <summary>
        /// 拣货默认到货位
        /// </summary>
        public const string PickingLoc = "Stage";
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
        /// 先入库先出库
        /// </summary>
        public const string PickRuleFR = "FirstReceiptFirstOutbound";

        /// <summary>
        /// 按单拣货
        /// </summary>
        public const string PickTypeByOrder = "ByOrder";
        /// <summary>
        /// 批量拣货
        /// </summary>
        public const string PickTypeByBatch = "ByBatch";


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

        #region 打印底部商城信息
        public static string PrintMallName = ConfigurationManager.AppSettings["PrintMallName"];
        public static string PrintMallHttpUrl = ConfigurationManager.AppSettings["PrintMallHttpUrl"];
        public static string PrintMallPhone = ConfigurationManager.AppSettings["PrintMallPhone"];
        #endregion

        #region 打印机地址设置
        //A4纸打印地址
        public static string PrintSettingA4 = ConfigurationManager.AppSettings["PrintSettingA4"];

        //10*18纸打印地址
        public static string PrintSetting1018 = ConfigurationManager.AppSettings["PrintSetting1018"];

        //UPC打印机地址
        public static string PrintSettingUPC = ConfigurationManager.AppSettings["PrintSettingUPC"];

        //针试打印机地址
        public static string PrintSettingZS = ConfigurationManager.AppSettings["PrintSettingZS"];

        //箱号打印机地址
        public static string PrintSettingCase = ConfigurationManager.AppSettings["PrintSettingCase"];
        #endregion

        #region 邮件
        public const string NewPurchaseSubject = "新的采购单通知";

        public const string NewOutboundSubject = "新的出库单通知";

        public const string NewPurchaseMailBody =
@"您好，
有新的采购单请您关注。
采购单号：{0}
预计到货日期：{1}
";

        public const string NewOutboundMailBody =
@"您好，
有新的出库单请您关注。
出库单号：{0}
出库日期：{1}
";
        #endregion

        #region 日志

        public const string InterfaceUserId = "99999";
        public const string InterfaceUserName = "wms";
        public const string InsertOrUpdateSku = "创建/更新商品";
        public const string InsertOrUpdatePackSku = "创建/更新商品包装";
        public const string InsertOrUpdateSkuClass = "创建/更新商品分类";
        public const string InsertOrUpdateWareHouse = "创建/更新仓库信息";
        public const string InsertPurchase = "创建采购单";
        public const string InsertOutbound = "创建出库单";
        public const string WriteERPPOStatus = "回写ERP采购单状态";
        public const string WriteOMSPOStatus = "回写OMS采购单状态";
        public const string WriteGZNBPOStatus = "回写农商采购单状态";
        public const string WriteERPSOStatus = "回写ERP订单状态";
        public const string WriteOMSSOStatus = "回写OMS订单状态";
        public const string WriteGZNBSOStatus = "回写农商订单状态";
        public const string VoidOutbound = "关闭出库单";
        public const string InsertAssembly = "创建加工单";
        public const string WriteBackeECCAssembly = "回写ECC加工单完成状态";
        public const string WriteBackeECCAssemblyStatus = "回写ECC加工单领料";
        public const string InsertStockTransfer = "库存转移";
        public const string CreatePurchaseOrderNumber = "退货入库生成入库单号";

        public const string AuditAdjustment = "损益单审核";
        public const string ClosePurchase = "关闭采购单";

        public const string CreateTransInventory = "创建移仓单";
        public const string WriteBackTransInventoryOutbound = "移仓单发货回写ECC";
        public const string WriteBackTransInventoryInbound = "移仓单入库回写ECC";

        public const string InsertAdjustment = "ECC创建损益单";

        public const string InsertSkuBorrow = "ECC创建商品外借单";
        public const string AuditSkuBorrow = "商品外借单审核";
        public const string LoadingSequence = "TMS装车顺序";

        public const string ZTOOrderSubmitLog = "创建中通订单";
        public const string InsertTMSOrder = "插入TMS运单号";

        public const string PushSkuBorrow = "商品外借推送";

        #region 出库
        public const string LogSaveDelivery = "保存发货";
        public const string LogQuicklyDeliver = "快速发货";
        public const string LogAllocationDeliver = "分配发货";
        //拣货
        public const string LogGeneratePickDetail = "生成拣货单";
        public const string LogCancelPickDetail = "取消拣货单";



        #endregion

        #region 增值
        //生产加工单
        public const string LogCancelReceive = "撤销领料";
        public const string LogFinishAssembly = "完成加工单";

        //质检
        public const string LogFinishQC = "质检完成";
        //TMS推送箱号
        public const string TMSStorageCase = "TMS推送箱号";
        public const string TMSUpdateStatus = "TMS推送出库单状态变更";
        public const string PushBoxCount = "TMS推送出库单总箱数";

        //ECC推送商品外借 
        public const string ECCSkuBorrow = "ECC推送商品外借";
        public const string ECCSkuBorrowReturn = "ECC推送商品外借归还";

        //ECC推送库存冻结
        public const string ECCLockOrder = "ECC推送库存冻结";
        public const string ECCLockOrderRelease = "ECC推送库存解冻";

        //ECC调用退货申请
        public const string InsertReturnPurchase = "插入退货入库单";
        public const string UpdateOutboundByReturnPurchase = "B2C退货";

        #endregion

        #endregion

        #region  百度地图
        public static string GeocoderURL = ConfigurationManager.AppSettings["GeocoderURL"];
        public static string GeocoderAK = ConfigurationManager.AppSettings["GeocoderAK"];
        #endregion

        #region ftp上传
        /// <summary>
        /// ftp地址
        /// </summary>
        private static string ftpAddress = ConfigurationManager.AppSettings["FtpUrl"].ToString();
        /// <summary>
        /// ftp展示地址
        /// </summary>
        public static string httpAddress = ConfigurationManager.AppSettings["FtpShowUrl"].ToString();
        /// <summary>
        /// ftp登陸名
        /// </summary>
        private static string userName = ConfigurationManager.AppSettings["FtpUserName"].ToString();
        /// <summary>
        /// ftp登陸密碼
        /// </summary>
        private static string password = ConfigurationManager.AppSettings["FtpPassword"].ToString();

        /// <summary>
        /// 损益图片目录 
        /// </summary>
        public const string Adjustment = "Adjustment";

        /// <summary>
        /// 商品外借图片目录
        /// </summary>
        public const string SkuBorrow = "SkuBorrow";

        #endregion

        #region 图片文件
        public const string FileAdjustmentDetail = "AdjustmentDetail";

        public const string FileSkuBorrowDetail = "SkuBorrowDetail";
        #endregion

        #region ZTO接口 

        public static string ZTOApi = ConfigurationManager.AppSettings["ZTOApi"];
        public static string ZTOOrderSubmit = ConfigurationManager.AppSettings["ZTOOrderSubmit"];
        public static string ZTOOrderMarke = ConfigurationManager.AppSettings["ZTOOrderMarke"];
        public static string ZTOUserName = ConfigurationManager.AppSettings["ZTOUserName"];
        public static string ZTOPassword = ConfigurationManager.AppSettings["ZTOPassword"];
        public static string ZTOSenderName = "国资商城";
        public static string ZTOSenderCity = "云南省,昆明市,呈贡区";
        public static string ZTOSenderAddress = "国资商场泛亚物流中心";
        public static string ZTOSenderPhone = "400-188-0871";
        public static string ZTOSenderCityTitle = "昆明市 呈贡区";


        //ZTO接口新

        public static string ZTOMarke = ConfigurationManager.AppSettings["ZTOMarke"];
        public static string ZTOMarkeAPI = ConfigurationManager.AppSettings["ZTOMarkeAPI"];
        public static string ZTOMarkeCompanyID = ConfigurationManager.AppSettings["ZTOMarkeCompanyID"];
        public static string ZTOMarkeKey = ConfigurationManager.AppSettings["ZTOMarkeKey"];

        public static string ZTOSubmit = ConfigurationManager.AppSettings["ZTOSubmit"];
        public static string ZTOSubmitAPI = ConfigurationManager.AppSettings["ZTOSubmitAPI"];
        public static string ZTOSubmitCompanyID = ConfigurationManager.AppSettings["ZTOSubmitCompanyID"];
        public static string ZTOSubmitKey = ConfigurationManager.AppSettings["ZTOSubmitKey"]; 


        #endregion

        public const string AssemblyLotAttr01 = "仓库加工";

        #region Excel导出相关

        public const int EachSheetDataRowsCount = 50000;

        public const int EachQuestDataRowsCount = 20000;

        public const int EachExportDataMaxCount = 100000;

        #endregion
    }
}
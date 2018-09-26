using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class FinanceInvoicingReportDto
    {
        public Guid WareHouseSysId { get; set; }

        public Guid SkuSysId { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        public string SkuOtherId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// 期初数量
        /// </summary>
        public int InitialQty { get; set; }
        public decimal DisplayInitialQty { get; set; }

        /// <summary>
        /// 期初数量
        /// </summary>
        public decimal InitialPrice { get; set; }

        /// <summary>
        /// 本期收货数量
        /// </summary>
        public int CurrentPeriodReceiptQty { get; set; }
        public decimal DisplayCurrentPeriodReceiptQty { get; set; }
        /// <summary>
        /// 本期收货金额
        /// </summary>
        public decimal CurrentPeriodReceiptPrice { get; set; }

        /// <summary>
        /// 本期出库数量
        /// </summary>
        public int CurrentPeriodOutboundQty { get; set; }
        public decimal DisplayCurrentPeriodOutboundQty { get; set; }



        /// <summary>
        /// 本期加工数量
        /// </summary>
        public int AssemblyQty { get; set; }
        public decimal DisplayAssemblyQty { get; set; }

        /// <summary>
        /// 本期出库金额
        /// </summary>
        public decimal CurrentPeriodOutboundPrice { get; set; }

        public int EndingInventoryQty { get; set; }
        public decimal DisplayEndingInventoryQty { get; set; }

        public decimal EndingInventoryPrice { get; set; }


        /// <summary>
        /// 本期损益数量
        /// </summary>
        public int AdjustmentQty { get; set; }
        public decimal DisplayAdjustmentQty { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 单位转化
        /// </summary>
        public Nullable<bool> InLabelUnit01 { get; set; }
        public Nullable<int> FieldValue01 { get; set; }
        public Nullable<int> FieldValue02 { get; set; }
    }
}
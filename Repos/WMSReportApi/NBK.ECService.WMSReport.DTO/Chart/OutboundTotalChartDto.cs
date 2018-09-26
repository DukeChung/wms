using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.DTO.Chart
{
    public class OutboundTotalChartDto
    {
        /// <summary>
        /// 当天出库单合计
        /// </summary>
        public int OutboundTotal { get; set; }


        /// <summary>
        /// 当天采购单合计
        /// </summary>
        public int PurchaseTotal { get; set; }

        public int B2COnlyTotal { get; set; }
        public int B2COnlyNewTotal { get; set; }
        public int B2COnlyPickTotal { get; set; }
        public int B2COnlyDeliverTotal { get; set; }
        public int B2COnlyFinishTotal { get; set; }
        public int B2CMultiTotal { get; set; }
        public int B2CMultiNewTotal { get; set; }
        public int B2CMultiPickTotal { get; set; }
        public int B2CMultiDeliverTotal { get; set; }
        public int B2CMultiFinishTotal { get; set; }
        public int B2BTotal { get; set; }
        public int B2BNewTotal { get; set; }
        public int B2BPickTotal { get; set; }
        public int B2BDeliverTotal { get; set; }
        public int B2BFinishTotal { get; set; }
    }

    public class OutboundChartDto
    {
        public System.Guid WareHouseSysId { get; set; }
        public string OutboundOrder { get; set; }
        public DateTime? OutboundDate { get; set; }
        public int? OutboundType { get; set; }
      
        public int? Status { get; set; }

        public int? OutboundSkuCount { get; set; }

    }
}
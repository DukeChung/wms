using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class BalanceReportQuery: BaseQuery
    {
        /// <summary>
        /// 快递公司
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 出库时间从
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 出库时间到
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string ConsigneeProvince { get; set; }

        /// <summary>
        /// 市
        /// </summary>

        public string ConsigneeCity { get; set; }

        /// <summary>
        /// 县
        /// </summary>
        public string ConsigneeArea { get; set; }

        /// <summary>
        /// 物流运单号
        /// </summary>
        public string CarrierNumber { get; set; }
    }
}

using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class BalanceReportDto
    {
        /// <summary>
        /// 快递公司
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? OutboundDate { get; set; }

        /// <summary>
        /// 出库日期显示
        /// </summary>
        public string OutboundDateText
        {
            get
            {
                if (OutboundDate == null)
                {
                    return string.Empty;
                }
                return Convert.ToDateTime(OutboundDate).ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 出库单号
        /// </summary>
        public string OutboundOrder { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 发货仓库
        /// </summary>
        public string WarehouseName { get; set; } 

        /// <summary>
        /// 收货省份
        /// </summary>
        public string ConsigneeProvince { get; set; }

        /// <summary>
        /// 收货市
        /// </summary>
        public string ConsigneeCity { get; set; }

        /// <summary>
        /// 收货区
        /// </summary>
        public string ConsigneeArea { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 物流运单号
        /// </summary>
        public string CarrierNumber { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }
    }
}

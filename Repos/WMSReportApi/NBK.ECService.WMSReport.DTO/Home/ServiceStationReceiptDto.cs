using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Home
{
    public class ServiceStationReceiptDto
    {
        public Guid SysId { get; set; }

        public DateTime UpdateDate { get; set; }

        public string ServiceStationName { get; set; }
        public decimal ReceiptPeriod { get; set; }

        public string DisplayReceiptPeriod
        {
            get {
                if (ReceiptPeriod > 0)
                {
                    return (ReceiptPeriod / 24.0m).ToString("f1");
                }
                return string.Empty;
            }
        }

        public DateTime LastReceiptDate { get; set; }
        public decimal LastReceiptPeriodSkuQty { get; set; }
        public decimal LastReceiptPeriodQty { get; set; }
        public Nullable<System.DateTime> NextReceiptPeriodDate { get; set; }

        public string DisplayNextReceiptPeriodDate { get { return NextReceiptPeriodDate.HasValue ? NextReceiptPeriodDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; }  }

        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public string OutboundOrder { get; set; }
        public Nullable<int> Status { get; set; }
        public decimal TotalQty { get; set; }
    }

    public class ServiceStationReceiptQuery : BaseQuery
    {

    }
}

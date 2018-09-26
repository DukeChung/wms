using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundSkuGlobalDto: BaseDto
    {
        public string ServiceStationCode { get; set; }

        public string ConsigneeCity { get; set; }

        public string ConsigneeArea { get; set; }

        public string OutboundOrder { get; set; } 

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateDisplay
        {
            get
            {
                return ActualShipDate.HasValue ? ActualShipDate.Value.ToString(PublicConst.DateFormat) : string.Empty;
            }
        }

        public string WarehouseName { get; set; }

        public string ServiceStationName { get; set; }

        //public Guid SysId { get; set; }

        public int SkuType { get; set; }

        public int SkuCount { get; set; }

        public int ReturnQty { get; set; }
    }
}

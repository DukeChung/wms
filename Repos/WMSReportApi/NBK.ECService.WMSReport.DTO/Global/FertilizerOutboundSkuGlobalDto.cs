using NBK.ECService.WMSReport.DTO.Base;
using NBK.ECService.WMSReport.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class FertilizerOutboundSkuGlobalDto : BaseDto
    {

        public string ServiceStationCode { get; set; }

        public string ServiceStationName { get; set; }

        public string OutboundOrder { get; set; }

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateDisplay
        {
            get
            {
                if (ActualShipDate.HasValue)
                {
                    return Convert.ToDateTime(ActualShipDate).ToString(PublicConst.DateTimeFormat);
                }
                return string.Empty;
            }
        }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public int ShippedQty { get; set; }

        public int ReturnQty { get; set; }

        public string WarehouseName { get; set; }
    }
}

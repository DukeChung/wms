using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class FertilizerOutboundSkuGlobalQuery: BaseQuery
    {

        public Guid SearchWarehouseSysId { get; set; }

        public string ServiceStationCode { get; set; }

        public string ServiceStationName { get; set; }

        public string OutboundOrder { get; set; }          

        public DateTime? ActualShipDateFrom { get; set; }

        public DateTime? ActualShipDateTo { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }
    }
}

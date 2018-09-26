using NBK.ECService.WMSReport.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundBoxReportQuery : BaseQuery
    {
        public string OutboundOrder { get; set; }

        public int? Status { get; set; }

        public DateTime? CreateDateStart { get; set; }

        public DateTime? CreateDateEnd { get; set; }

        public DateTime? ActualShipDateStart { get; set; }

        public DateTime? ActualShipDateEnd { get; set; }
    }
}

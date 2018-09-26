using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;

namespace NBK.ECService.WMS.DTO
{
    public class OutboundBoxReportDto
    {
        public string WarehouseName { get; set; }

        public Guid OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }

        public int Status { get; set; }

        public string StatusText
        {
            get { return ((OutboundStatus)Status).ToDescription(); }
        }

        public int WholeCaseQty { get; set; }

        public int ScatteredCaseQty { get; set; }

        public int TotalCaseQty { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateText { get { return ActualShipDate.HasValue ? ActualShipDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }
    }
}

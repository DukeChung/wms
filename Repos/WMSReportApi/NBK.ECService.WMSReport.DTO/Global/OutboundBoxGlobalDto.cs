using NBK.ECService.WMSReport.Utility;
using NBK.ECService.WMSReport.Utility.Enum;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class OutboundBoxGlobalDto
    {
        public string WarehouseName { get; set; }

        public Guid OutboundSysId { get; set; }

        public string OutboundOrder { get; set; }

        public int WholeCaseQty { get; set; }

        public int ScatteredCaseQty { get; set; }

        public int TotalCaseQty { get { return WholeCaseQty + ScatteredCaseQty; } }

        public int Status { get; set; }

        public string StatusText
        {
            get { return ((OutboundStatus)Status).ToDescription(); }
        }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get { return CreateDate.ToString(PublicConst.DateTimeFormat); } }

        public DateTime? ActualShipDate { get; set; }

        public string ActualShipDateText { get { return ActualShipDate.HasValue ? ActualShipDate.Value.ToString(PublicConst.DateTimeFormat) : string.Empty; } }

    }
}

using System;

namespace NBK.ECService.WMS.DTO
{
    public class PickDetailSumDto
    {
        public Guid? OutboundSysId { get; set; }

        public Guid? OutboundDetailSysId { get; set; }

        public string OutboundOrder { get; set; }

        public Guid? SkuSysId { get; set; }

        public string SkuName { get; set; }

        public string UPC { get; set; }

        public string SkuDescr { get; set; }

        public int? Qty { get; set; }

        public int ScanQty { get; set; }

        public int? UnScanQty { get; set; }
        public string UPC01 { get; set; }

        public string UPC02 { get; set; }

        public string UPC03 { get; set; }

        public string UPC04 { get; set; }

        public string UPC05 { get; set; }

        public int? FieldValue01 { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int? FieldValue04 { get; set; }

        public int? FieldValue05 { get; set; }
    }
}
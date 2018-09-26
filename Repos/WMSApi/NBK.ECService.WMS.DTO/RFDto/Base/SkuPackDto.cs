using System;

namespace NBK.ECService.WMS.DTO
{
    public class SkuPackDto
    {
        public Guid? SkuSysId { get; set; }

        public string UPC { get; set; }

        public string SkuName { get; set; }

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

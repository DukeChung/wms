using System;
using System.Collections.Generic;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class OutboundBoxListDto
    {
        public int CaseCount { get; set; }

        public int Qty { get; set; }

        public List<OutboundBoxDto> OutboundBoxDtoList { get; set; }
    }

    public class OutboundBoxDto
    {
        public Guid? SkuSysId { get; set; }

        public int Qty { get; set; }

        public string SkuName { get; set; }

        public int? FieldValue02 { get; set; }

        public int? FieldValue03 { get; set; }

        public int CaseQty { get; set; }

        public Guid? BoxSysId { get; set; }

        public string BoxName { get; set; }

        public string BoxType { get; set; }

        public int BoxSkuQty { get; set; }

        public int BoxSkuCount { get; set; }

        public string CreateBy { get; set; }

        public string CreateTime { get; set; }

        public Guid OutboundSysId { get; set; }
    }
}
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class InvSkuReportDto
    {
        public Guid? SysId { get; set;}
        public string SkuOtherId { get; set; }
        public string SkuName { get; set; }
        public string UPC { get; set; }
        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }
        public string WareHouseName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.DTO.Other
{
    public class ReceiptDto
    {
        public Guid SysId { get; set; }
        public string ReceiptOrder { get; set; }

        //public string DisplayExternalOrder { get; set; }
        public string ExternalOrder { get; set; }
        public int ReceiptType { get; set; }
        public Guid? WarehouseSysId { get; set; }
        public DateTime? ExpectedReceiptDate { get; set; }
        public DateTime? ReceipDate { get; set; }
        public int? Status { get; set; }
        public string Descr { get; set; }
        public string ReturnDescr { get; set; }
        public long CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int? TotalExpectedQty { get; set; }
        public int? TotalReceivedQty { get; set; }
        public int? TotalRejectedQty { get; set; }

        public decimal? DisplayTotalExpectedQty { get; set; }
        public decimal? DisplayTotalReceivedQty { get; set; }
        public decimal? DisplayTotalRejectedQty { get; set; }
    }
}

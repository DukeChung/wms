using NBK.ECService.WMSReport.DTO.Base;
using System;

namespace NBK.ECService.WMSReport.DTO
{
    public class SkuBorrowReportQuery:BaseQuery
    {
        public string SkuBorrowOrder { get; set; }

        public int? Status { get; set; }

        public DateTime? BorrowStartTime { get; set; }

        public DateTime? BorrowEndTime { get; set; }

        public string CreateUserName { get; set; }

        public string UPC { get; set; }

        public string SkuCode { get; set; }

        public string SkuName { get; set; }

        public string BorrowName { get; set; }

        public string LendingDepartment { get; set; }

        public string OtherId { get; set; }

        public string Channel { get; set; }
    }
}

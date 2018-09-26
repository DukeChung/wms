using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ReceiptdetailAutoShelvesDto
    {
        public Guid? SysId { get; set; }
        public Guid ReceiptSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public int Status { get; set; }
        public int? ExpectedQty { get; set; }
        public Nullable<int> ReceivedQty { get; set; }
        public Nullable<int> RejectedQty { get; set; }
        public Nullable<decimal> Price { get; set; }

        public string Remark { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Nullable<Guid> UOMSysId { get; set; }
        public Nullable<Guid> PackSysId { get; set; }
        public string ToLoc { get; set; }
        public string ToLot { get; set; }
        public string ToLpn { get; set; }
        public string LotAttr01 { get; set; }
        public string LotAttr02 { get; set; }
        public string LotAttr04 { get; set; }
        public string LotAttr03 { get; set; }
        public string LotAttr05 { get; set; }
        public string LotAttr06 { get; set; }
        public string LotAttr07 { get; set; }
        public string LotAttr08 { get; set; }
        public string LotAttr09 { get; set; }
        public string ExternalLot { get; set; }
        public Nullable<DateTime> ProduceDate { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public Nullable<DateTime> ReceivedDate { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }

        public int ShelvesStatus { get; set; }

        public int ShelvesQty { get; set; }

        public Guid PickDetailSysId { get; set; }
    }
}

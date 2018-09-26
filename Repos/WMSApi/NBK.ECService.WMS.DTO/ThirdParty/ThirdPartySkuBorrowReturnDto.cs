using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartySkuBorrowReturnDto
    { 
        public int LendOrderId { get; set; }
        public string EditUserName { get; set; }
        public long? EditUserId { get; set; }
        public DateTime? EditTime { get; set; }
        public List<ThirdPartySkuBorrowReturnDetail> Detail { get; set; }
        public int InStockWay { get; set; } 
        public string Memo { get; set; }
    }

    public class ThirdPartySkuBorrowReturnDetail
    {
        public int ProductCode { get; set; } 
        public int DamageLevel { get; set; }
        public string DamageReason { get; set; }
        public int Quantity { get; set; }
        public int RejectQuantity { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartySkuBorrowLendDto
    {
        public int LendOrderId { get; set; }
        public string EditUserName { get; set; }
        public long? EditUserId { get; set; }
        public DateTime? EditTime { get; set; }
    } 
}

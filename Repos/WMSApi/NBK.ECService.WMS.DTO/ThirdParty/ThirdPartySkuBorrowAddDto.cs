using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartySkuBorrowAddDto
    { 
        public int LendOrderId { get; set; }
        public int WareHouseId { get; set; }  
        public DateTime? BorrowStartTime { get; set; }
        public DateTime? BorrowEndTime { get; set; } 
        public string Remark { get; set; }
        public string BorrowName { get; set; }
        public string LendingDepartment { get; set; } 
        public string Channel { get; set; }  
        public long CreateBy { get; set; } 
        public string CreateUserName { get; set; }  
        public List<ThirdPartySkuBorrowAddDetailDto> SkuBorrowDetailList { get; set; } 
    }

    public class ThirdPartySkuBorrowAddDetailDto
    { 
        public string SkuId { get; set; }
        public int Qty { get; set; } 
        public DateTime? BorrowStartTime { get; set; }
        public DateTime? BorrowEndTime { get; set; }    
        public string Remark { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuBorrowDto
    {
        public Guid? SysId { get; set; }  
        public Guid? WareHouseSysId { get; set; }  
        public string BorrowOrder { get; set; }  
        public int Status { get; set; }
        public DateTime? BorrowStartTime { get; set; } 
        public DateTime? BorrowEndTime { get; set; } 
        public int IsDamage { get; set; } 
        public string Remark { get; set; } 
        public string BorrowName { get; set; } 
        public string LendingDepartment { get; set; } 
        public int? OtherId { get; set; }
        public string Channel { get; set; } 
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; } 

        public List<SkuBorrowDetailDto> SkuBorrowDetailList { get; set; }

        public int AuditingBy { get; set; }
        public string AuditingName { get; set; } 
    }

    public class SkuBorrowDetailDto
    {
        public Guid SysId { get; set; }
        public Guid SkuBorrowSysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public string Loc { get; set; } = "";

        public string Lot { get; set; } = "";

        public string Lpn { get; set; } = "";

        public string UPC { get; set; }

        public string UOMCode { get; set; }

        public int Qty { get; set; }

        public decimal DisplayQty { get; set; }

        public DateTime? BorrowStartTime { get; set; }

        public DateTime? BorrowEndTime { get; set; }

        public int Status { get; set; }

        public int IsDamage { get; set; }

        public string DamageReason { get; set; }

        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Remark { get; set; }
        public List<PictureDto> PictureDtoList { get; set; }

        public int ReturnQty { get; set; }

        public decimal DisplayReturnQty { get; set; }
    }
}

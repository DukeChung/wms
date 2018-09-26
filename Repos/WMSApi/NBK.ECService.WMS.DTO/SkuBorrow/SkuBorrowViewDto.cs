using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuBorrowViewDto
    {
        public Guid SysId { get; set; }

        public string SkuBorrowOrder { get; set; }

        public Guid WareHouseSysId { get; set; }

        public string WareHouseName { get; set; }

        public DateTime? BorrowStartTime { get; set; }

        public string BorrowStartTimeDisplay
        {
            get
            {
                if (BorrowStartTime != null)
                {
                    return BorrowStartTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                return string.Empty;
            }
        }

        public DateTime? BorrowEndTime { get; set; }

        public string BorrowEndTimeDisplay
        {
            get
            {
                if (BorrowEndTime != null)
                {
                    return BorrowEndTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                return string.Empty;
            }
        } 
       
        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                return ((SkuBorrowStatus)Status).ToDescription();
            }
        } 

        public DateTime CreateDate { get; set; }

        public string CreateUserName { get; set; }

        public string CreateInfoDisplay
        {
            get
            {
                return $"{CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - {CreateUserName}";
            }
        }

        public string BorrowName { get; set; }

        public string LendingDepartment { get; set; }

        public string OtherId { get; set; }

        public string Channel { get; set; }

        public string Remark { get; set; }

        public List<SkuBorrowDetailViewDto> SkuBorrowDetailList { get; set; } = new List<SkuBorrowDetailViewDto>();
    }

    public class SkuBorrowDetailViewDto
    {
        public Guid SysId { get; set; }
        public Guid SkuSysId { get; set; }
        public string SkuCode { get; set; }
        public string SkuName { get; set; }

        public string SkuDescr { get; set; }

        public decimal DisplayQty { get; set; }

        public string UPC { get; set; }

        public string UOMCode { get; set; }

        public int Qty { get; set; } 

        public string Loc { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public DateTime? BorrowStartTime { get; set; }

        public string BorrowStartTimeDisplay
        {
            get
            {
                if (BorrowStartTime != null)
                {
                    return BorrowStartTime.ToDateTime().ToString("yyyy-MM-dd");
                }
                return string.Empty;                
            }
        }

        public DateTime? BorrowEndTime { get; set; }

        public string BorrowEndTimeDisplay
        {
            get
            {
                if (BorrowEndTime != null)
                {
                    return BorrowEndTime.ToDateTime().ToString("yyyy-MM-dd");
                }
                return string.Empty;
            }
        }

        public int? IsDamage { get; set; }

        public string IsDamageDisplay
        {
            get
            {
                if (IsDamage != null)
                {
                    return IsDamage == 1 ? "是" : "否";
                }
                return string.Empty;
            }
        }

        public string DamageReason { get; set; }

        public string Remark { get; set; }
        public List<PictureDto> PictureDtoList { get; set; }

        public int ReturnQty { get; set; }
        public decimal DisplayReturnQty { get; set; }

    }
}

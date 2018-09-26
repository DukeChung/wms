using NBK.ECService.WMS.Utility;
using NBK.ECService.WMS.Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class SkuBorrowListDto
    {

        public Guid SysId { get; set; }

        public string BorrowOrder { get; set; }

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

        
        public string BorrowName { get; set; }

        public string LendingDepartment { get; set; }

        public DateTime? BorrowStartTime { get; set; }

        public string BorrowStartTimeDisplay
        {
            get
            {
                if (BorrowStartTime == null)
                {
                    return string.Empty;
                }
                return Convert.ToDateTime(BorrowStartTime).ToString("yyyy-MM-dd");
            }
        }
        public DateTime? BorrowEndTime { get; set; }

        public string BorrowEndTimeDisplay
        {
            get
            {
                if (BorrowEndTime == null)
                {
                    return string.Empty;
                }
                return Convert.ToDateTime(BorrowEndTime).ToString("yyyy-MM-dd");
            }
        }

        public string CreateInfoDisplay
        {
            get
            {
                return $"{CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - {CreateUserName}";
            }
        }

        public string Remark { get; set; }

        public int? OtherId { get; set; }

        public string Channel { get; set; }
    }
}

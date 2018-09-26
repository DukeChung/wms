using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class ZoneDto : BaseDto
    {
        public Guid SysId { get; set; }

        public string ZoneCode { get; set; }

        public string DefaultPickToLoc { get; set; }

        public string InLoc { get; set; }

        public string OutLoc { get; set; }

        public string Descr { get; set; }

        public long CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

        public string IsActiveText { get { return IsActive ? "是" : "否"; } }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }
    }
}

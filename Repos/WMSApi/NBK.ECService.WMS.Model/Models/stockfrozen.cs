using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class stockfrozen : SysIdEntity
    {
        public Guid? SkuSysId { get; set; }

        public Guid ZoneSysId { get; set; }

        public string Loc { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public Guid WarehouseSysId { get; set; }

        public string Lot { get; set; }

        public string Lpn { get; set; }

        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }

        public string Memo { get; set; }

        public int FrozenSource { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
   public partial class workrule : SysIdEntity
    {
        public workrule()
        {
        }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> PickWork { get; set; }
        public Nullable<bool> ShelvesWork { get; set; }
        public Nullable<bool> ReceiptWork { get; set; }
        public System.Guid WarehouseSysId { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }

    }
}

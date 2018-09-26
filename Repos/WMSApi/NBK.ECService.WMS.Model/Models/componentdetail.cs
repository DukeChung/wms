using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class componentdetail : SysIdEntity
    {
        public System.Guid ComponentSysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public int Qty { get; set; }
        public int LossQty { get; set; }
        public Nullable<bool> IsMain { get; set; }
        public int Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}

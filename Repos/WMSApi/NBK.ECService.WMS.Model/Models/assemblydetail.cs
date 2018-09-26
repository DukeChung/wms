using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class assemblydetail : SysIdEntity
    {
        public System.Guid AssemblySysId { get; set; }
        public System.Guid SkuSysId { get; set; }
        public decimal UnitQty { get; set; }
        public int Qty { get; set; }
        public decimal LossQty { get; set; }
        public int AllocatedQty { get; set; }
        public int PickedQty { get; set; }
        public int Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
        public string Grade { get; set; }
    }
}

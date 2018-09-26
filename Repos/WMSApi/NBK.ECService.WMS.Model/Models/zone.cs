using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class zone : SysIdEntity
    {
        public zone()
        {
            this.locations = new List<location>();
        }

        public string ZoneCode { get; set; }
        public string DefaultPickToLoc { get; set; }
        public string InLoc { get; set; }
        public string OutLoc { get; set; }
        public string Descr { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public Guid WarehouseSysId { get; set; }
        public virtual ICollection<location> locations { get; set; }
    }
}

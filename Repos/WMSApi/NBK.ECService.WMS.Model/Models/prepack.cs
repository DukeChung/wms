using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class prepack : SysIdEntity
    {
        public prepack()
        {
            this.prepackdetails = new List<prepackdetail>();
        }

        public System.Guid WareHouseSysId { get; set; }
        public string PrePackOrder { get; set; }
        public string StorageLoc { get; set; }
        public Nullable<int> Status { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string OutboundOrder { get; set; }
        public Nullable<System.Guid> OutboundSysId { get; set; }
        public string Source { get; set; }
        public string CreateUserName { get; set; }
        public string UpdateUserName { get; set; }

        public string BatchNumber { get; set; }

        public string ServiceStationName { get; set; }


        public virtual ICollection<prepackdetail> prepackdetails { get; set; }
    }
}

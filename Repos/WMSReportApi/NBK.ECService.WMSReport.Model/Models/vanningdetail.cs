using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class vanningdetail : SysIdEntity
    {
        public vanningdetail()
        {
            this.vanningpickdetails = new List<vanningpickdetail>();
        }
         
        public Nullable<System.Guid> VanningSysId { get; set; }
        public System.Guid ContainerSysId { get; set; }
        public string ContainerNumber { get; set; }
        public Nullable<System.Guid> CarrierSysId { get; set; }
        public string CarrierNumber { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string HandoverGroupOrder { get; set; }
        public DateTime HandoverCreateDate { get; set; }
        public int HandoverCreateBy { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        [JsonIgnore]
        public virtual container container { get; set; }
        [JsonIgnore]
        public virtual vanning vanning { get; set; }
        public virtual ICollection<vanningpickdetail> vanningpickdetails { get; set; }
    }
}

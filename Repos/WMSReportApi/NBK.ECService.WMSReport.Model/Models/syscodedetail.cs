using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Newtonsoft.Json;

namespace NBK.ECService.WMSReport.Model.Models
{
    public partial class syscodedetail: SysIdEntity
    {
        public System.Guid SysCodeSysId { get; set; }
        public string SeqNo { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }

        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        [JsonIgnore]
        public virtual syscode syscode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class syscode : SysIdEntity
    {
        public syscode()
        {
            this.syscodedetails = new List<syscodedetail>();
        }

        public string SysCodeType { get; set; }
        public string Descr { get; set; }
        public virtual ICollection<syscodedetail> syscodedetails { get; set; }
    }
}

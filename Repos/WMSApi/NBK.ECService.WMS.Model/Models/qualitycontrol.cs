using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class qualitycontrol : SysIdEntity
    {
        public Guid WareHouseSysId { get; set; }
        public int Status { get; set; }
        public string QCOrder { get; set; }
        public long CreateBy { get; set; }
        public string CreateUserName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public string UpdateUserName { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int QCType { get; set; }
        public string ExternOrderId { get; set; }
        public string DocOrder { get; set; }
        public string QCUserName { get; set; }
        public Nullable<System.DateTime> QCDate { get; set; }
        public string Descr { get; set; }
    }
}

using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public class assemblyskuweight : SysIdEntity
    {
        public Guid SkuSysId { get; set; }
        public Guid AssemblySysId { get; set; }
        public Guid WarehouseSysId { get; set; }
        public decimal Weight { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
    
    }
}

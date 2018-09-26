using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class workuser : SysIdEntity
    {
        public string WorkUserCode { get; set; }
        public string WorkUserName { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> WorkType { get; set; }
        public int WorkStatus { get; set; }
        public Nullable<decimal> Proficiency { get; set; }
        public Nullable<Guid> TS { get; set; }
        public bool IsAssigned { get; set; }
        public Guid WarehouseSysId { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; }
        public long UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUserName { get; set; }
    }
}

using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Model.Models
{
    public partial class warehouse : SysIdEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Contacts { get; set; }
        public string Telephone { get; set; }
        public string URL { get; set; }
        public long CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string OtherId { get; set; }
        public string UpdateUserName { get; set; }
        public string CreateUserName { get; set; }
        public string WareHouseArea { get; set; }
        public string WareHouseProperty { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionStringRead { get; set; }
    }
}

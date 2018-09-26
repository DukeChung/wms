using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDelete { get; set; }
        public bool Active { get; set; }

        public string SystemName { get; set; }
        public List<SystemFunction> PermissionRecords { get; set; }
    }
}

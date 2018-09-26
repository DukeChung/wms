using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.DTO.Authorize
{
    public class ApplicationUser
    {
        public ApplicationUser()
        {

        }

        public ApplicationUser(dynamic user)
        {
            this.Id = user.SysNO.ToString();
            this.UserId = user.SysNO;
            this.UserName = user.LoginName;
            this.DepartmentID = user.DepartmentID;
            this.DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.LoginName : user.DisplayName;
        }

        public string Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string SessionKey { get; set; }

        public int DepartmentID { get; set; }
        
        public Guid WarehouseSysId { get; set; }

        public string WarehouseName { get; set; }
    }
}

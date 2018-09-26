using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WMS.Global.Portal.App_Start;
using WMS.Global.Portal.AuthServiceReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using NBK.ECService.WMSReport.DTO;
using NBK.ECService.WMSReport.DTO.Base;
using NBK.AuthServiceUtil;

namespace WMS.Global.Portal.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }

        public ApplicationUser(SystemUser user)
        {
            this.Id = user.SysNO.ToString();
            this.UserId = user.SysNO;
            this.UserName = user.LoginName;
            this.DepartmentID = user.DepartmentID;
            this.DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.LoginName : user.DisplayName;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, SystemDataConst.ApplicationID + DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("LogonUser", JsonConvert.SerializeObject(this)));
            return userIdentity;
        }

        // public string Id { get; set; }
        public int UserId { get; set; }
        // public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string SessionKey { get; set; }

        public int DepartmentID { get; set; }

        //登录用户的所有仓库
        public List<WareHouseDto> WareHouseList { get; set; }

        public Guid WarehouseSysId { get; set; }

        public string WarehouseName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
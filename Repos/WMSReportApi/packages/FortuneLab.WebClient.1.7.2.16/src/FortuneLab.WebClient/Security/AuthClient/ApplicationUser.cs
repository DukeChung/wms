using FortuneLab.WebClient.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Security.AuthClient
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IUser
    {
        public ApplicationUser()
        {

        }

        public ApplicationUser(User user)
        {
            this.LogonUser = user;
            this.Id = user.SysId.ToString();
            this.UserId = user.SysId;
            this.UserName = user.LoginName;
            this.DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.LoginName : user.DisplayName;
        }

        public ApplicationUser(SessionObject sessionObject)
            : this(sessionObject.LogonUser)
        {
            this.SessionKey = sessionObject.SessionKey;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(AuthConstants.SessionKey, this.SessionKey));
            userIdentity.AddClaim(new Claim(AuthConstants.LogonUser, JsonConvert.SerializeObject(this)));
            return userIdentity;
        }

        public string Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string SessionKey { get; set; }
        private User LogonUser { get; set; }
    }
}

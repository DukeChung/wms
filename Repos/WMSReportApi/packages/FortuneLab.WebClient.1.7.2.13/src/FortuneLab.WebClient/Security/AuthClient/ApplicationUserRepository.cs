using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebClient.Security.AuthClient
{
    public class ApplicationUserRepository : IUserStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        private Microsoft.Owin.IOwinContext context;

        //private string _sessionKey = null;
        //public string SessionKey
        //{
        //    get
        //    {
        //        if (_sessionKey == null)
        //        {
        //            _sessionKey = context.Get<string>(AuthConstants.SessionKey);
        //        }
        //        return _sessionKey;
        //    }
        //}

        public ApplicationUserRepository(Microsoft.Owin.IOwinContext context)
        {
            // TODO: Complete member initialization
            this.context = context;
        }



        #region IUserStore
        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            //var accountResponse = AccountApiClient.GetAccount(new AccountQuery() { LoginId = userId, SessionKey = SessionKey });
            //return Task.FromResult(new ApplicationUser(accountResponse.ResponseResult));
            return null;
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return null;
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            //return Task.FromResult( AccountApiClient.UpdateAccount(new ArticlePageIdQuery(), ) );
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region IUserLoginStore
        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            return null;
            //throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            IList<UserLoginInfo> result = (IList<UserLoginInfo>)new List<UserLoginInfo>();
            //foreach (IdentityUserLogin identityUserLogin in (IEnumerable<IdentityUserLogin>)user.Logins)
            //    result.Add(new UserLoginInfo(identityUserLogin.LoginProvider, identityUserLogin.ProviderKey));

            return Task.FromResult<IList<UserLoginInfo>>(result);
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserEmailStore
        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserPasswordStore
        //public Task<string> GetPasswordHashAsync(ApplicationUser user)
        //{
        //    return Task.FromResult(user.Password);
        //}

        //public Task<bool> HasPasswordAsync(ApplicationUser user)
        //{
        //    return Task.FromResult(!string.IsNullOrWhiteSpace(user.Password));
        //}

        //public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        //{
        //    return Task.FromResult(user.Password = passwordHash);
        //}
        #endregion
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using NBK.AuthServiceUtil;
using WMS.Global.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WMS.Global.Portal.App_Start
{

    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            //var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            var manager = new ApplicationUserManager(new ApplicationUserRepository(context));

            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                //AllowOnlyAlphanumericUserNames = false,
                //RequireUniqueEmail = true
            };

            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // 配置用户锁定默认值
            //manager.UserLockoutEnabledByDefault = true;
            //manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并将其插入到此处。
            //manager.RegisterTwoFactorProvider("电话代码", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "你的安全代码是 {0}"
            //});
            //manager.RegisterTwoFactorProvider("电子邮件代码", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "安全代码",
            //    BodyFormat = "你的安全代码是 {0}"
            //});
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    // 配置要在此应用程序中使用的应用程序登录管理器。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        /// <summary>
        /// 重写验证登录密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        //public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        //{
        //    var loginUser = AuthorizeManager.Login(userName, password);



        //    return SignInStatus.Success;
        //}
    }

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
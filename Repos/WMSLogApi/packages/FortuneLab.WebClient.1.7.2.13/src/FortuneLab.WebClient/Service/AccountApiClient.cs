using FortuneLab.WebApiClient;
using FortuneLab.WebApiClient.Query;
using FortuneLab.WebClient.Models;
using FortuneLab.WebClient.Security;
using System;
using System.Configuration;

namespace FortuneLab.WebClient.Service.API
{
    internal class ApiClientConst
    {
        public static string BenzAPIURL = ConfigurationManager.AppSettings["BenzAPIURL"];
    }

    /// <summary>
    /// 帐号相关接口(登录, 注册账户，更新账户
    /// </summary>
    public class AccountApiClient
    {
        public static string ApiUrl = ApiClientConst.BenzAPIURL;

        public static ApiResponse<SessionObject> Login(LoginQuery query)
        {
            return ApiClient.NExecute<SessionObject>(ApiUrl, "accounts/account/login", query, MethodType.Get);
        }

        public static ApiResponse<Guid> Register(User model)
        {
            return ApiClient.NExecute<Guid>(ApiUrl, "accounts/account/register", null, MethodType.Post, model);
        }

        public static ApiResponse<SessionObject> AdminLogin(CoreQuery query)
        {
            var loginInfo = ApiClient.NExecute<SessionObject>(ApiUrl, "accounts/account/adminLogin", query, MethodType.Get);
            return loginInfo;
        }

        public static ApiResponse<SessionObject> SessionValidate(SessionQuery query)
        {
            var obj = ApiClient.NExecute<SessionObject>(ApiUrl, "accounts/account/validateSession", query, MethodType.Get);
            return obj;
        }

        public static ApiResponse<User> GetAccount(string loginId, Guid? accoungId = null)
        {
            var query = new CoreQuery() { ParmsObj = new { loginId, accoungId } };
            return ApiClient.NExecute<User>(ApiUrl, accoungId.HasValue ? "accounts/account/get" : "accounts/account/getByLoginId", query);
        }

        public static ApiResponse<string> ConfirmEmail(string email, string verifyToken)
        {
            var query = new CoreQuery() { ParmsObj = new { email, verifyToken } };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/account/email/confirm", query, MethodType.Get);
        }

        /// <summary>
        /// 重新发送邮箱确认邮件
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId">如果存在, 会用email更新用户的邮件地址</param>
        /// <returns></returns>
        public static ApiResponse<string> ResendConfirmEmail(string email, Guid? userId = null)
        {
            var query = new CoreQuery() { ParmsObj = new { email, userId } };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/account/resendConfirmEmail", query, MethodType.Get);
        }

        /// <summary>
        /// 用户自己修改自己的密码
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static ApiResponse<string> ChangePassword(LoginQuery query, Guid userId, string oldPassword, string newPassword)
        {
            query.ParmsObj = new { userId, oldHashedPassword = MD5CryptoProvider.GetMD5Hash(oldPassword), newHashedPassword = MD5CryptoProvider.GetMD5Hash(newPassword) };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/password/changePassword", query, MethodType.Post);
        }

        public static ApiResponse<string> AdminResetPassword(LoginQuery query, Guid userId, string newPassword)
        {
            query.ParmsObj = new { userId, newHashedPassword = MD5CryptoProvider.GetMD5Hash(newPassword) };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/admin/password/reset", query, MethodType.Post);
        }
        /// <summary>
        /// 更新密码ResetToken
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static ApiResponse<string> UpdateResetPasswordToken(string email)
        {
            var query = new CoreQuery() { ParmsObj = new { email = email } };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/password/updateResetPasswordToken", query, MethodType.Post);
        }

        /// <summary>
        /// 通过Email发送的Token来恢复密码
        /// </summary>
        /// <param name="email"></param>
        /// <param name="resetToken"></param>
        /// <param name="newHashedPassword"></param>
        /// <returns></returns>
        public static ApiResponse<string> ResetPasswordByToken(string email, string resetToken, string newHashedPassword)
        {
            var query = new CoreQuery() { ParmsObj = new { email, resetToken, newHashedPassword } };
            return ApiClient.NExecute<string>(ApiUrl, "accounts/password/resetPassword/byEmailToken", query, MethodType.Post);
        }
    }
}

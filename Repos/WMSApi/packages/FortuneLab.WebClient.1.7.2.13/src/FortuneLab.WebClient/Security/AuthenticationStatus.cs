using System.Web;

namespace FortuneLab.WebClient.Security
{
    public enum AuthenticationResult
    {
        Success,
        NoTicket,
        CorruptedTicket,
        ExpiredTicket,
        IncorrectLogin,
        IncorrectPassword,
        InactiveUser,
        DenyAccess,
        TemporaryLockedOut,
        ExceptionDuringAuthentication,
        InvalidUserReference
    }

    public class AuthenticationStatus
    {
        private const string AuthStatusContextKey = "AuthenticationStatus";

        public AuthenticationStatus(AuthenticationResult result)
        {
            Result = result;
        }

        public AuthenticationResult Result { get; set; }

        public static void Set(AuthenticationStatus status)
        {
            HttpContext.Current.Items[AuthStatusContextKey] = status;
        }

        public static AuthenticationStatus Get()
        {
            return HttpContext.Current.Items[AuthStatusContextKey] as AuthenticationStatus;
        }
    }
}
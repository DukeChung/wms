
using FortuneLab.WebClient.Models;

namespace FortuneLab.WebClient.Security
{
    public class CachedAuthenticatedUser
    {
        /// <summary>
        /// Created to support reset password functionality.
        /// </summary>
        /// <param name="v5UserData"></param>
        /// <param name="password"></param>
        public CachedAuthenticatedUser(User userData, string password)
        {
            PasswordHash = (password ?? string.Empty).GetHashCode();
            UserData = userData;
        }

        public int PasswordHash { get; set; }
        public User UserData { get; set; }
    }
}

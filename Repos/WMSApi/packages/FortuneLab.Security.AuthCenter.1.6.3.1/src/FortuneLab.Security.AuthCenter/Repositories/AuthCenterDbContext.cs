using FortuneLab.Repositories;

namespace FortuneLab.Security.AuthCenter.Repositories
{
    public class AuthCenterDbConnProvider :  IDbConnProvider
    {
        public AuthCenterDbConnProvider()
        {
            this.NameOrConnectionString = "AuthCenter";
        }

        public string NameOrConnectionString { get; private set; }
    }
}

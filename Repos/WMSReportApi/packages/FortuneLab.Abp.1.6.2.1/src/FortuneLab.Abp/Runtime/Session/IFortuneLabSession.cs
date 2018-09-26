using Abp.Runtime.Session;
using Abp.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.Runtime.Session
{
    public interface IFortuneLabSession : IAbpSession
    {
        FortuneLabUserPrincipal<int> Principal { get; }
        FortuneLabUserIdentity<int> Identity { get; }
        string UserAgent { get; }
        string UserIpAddress { get; }
        ApiToken ApiToken { get; }
    }
}

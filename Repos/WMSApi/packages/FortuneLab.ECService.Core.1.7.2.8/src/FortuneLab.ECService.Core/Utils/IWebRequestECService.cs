using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Utils
{
    [Obsolete("请使用FortuneLabWebApiClient替代", false)]
    public interface IWebRequestECService : ITransientDependency
    {
        Task<TResult> PostAsync<TResult>(string baseUrl, string url, object input = null, int timeout = 30000)
          where TResult : class, new();

        Task PostAsync(string baseUrl, string url, object input = null, int timeout = 30000);

        TResult Post<TResult>(string baseUrl, string url, object input = null, int timeout = 30000);

    }
}

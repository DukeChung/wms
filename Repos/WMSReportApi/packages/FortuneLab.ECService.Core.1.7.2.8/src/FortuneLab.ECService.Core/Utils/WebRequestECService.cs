using System.Threading.Tasks;
using FortuneLab.ECService.Utils;
using System;

namespace Abp.Utils
{
    [Obsolete("请使用FortuneLabWebApiClient替代", false)]
    public class WebRequestECService : IWebRequestECService
    {
        /// <summary>
        /// 又返回的更新
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="input"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string baseUrl, string url, object input = null, int timeout = 30000) where TResult : class, new()
        {
            string ecserviceUrl = baseUrl + url;
            AbpWebApiClient client = new AbpWebApiClient();
            return await client.PostAsync<TResult>(ecserviceUrl, input, timeout);
        }

        /// <summary>
        /// 没有返回值 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="input"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task PostAsync(string baseUrl, string url, object input = null, int timeout = 30000)
        {
            string ecserviceUrl = baseUrl + url;
            AbpWebApiClient client = new AbpWebApiClient();
            await client.PostAsync(ecserviceUrl, input, timeout);
        }

        /// <summary>
        /// 同步调用
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="baseUrl"></param>
        /// <param name="url"></param>
        /// <param name="input"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public TResult Post<TResult>(string baseUrl, string url, object input = null, int timeout = 30000)
        {
            string ecserviceUrl = baseUrl + url;
            AbpWebApiClient client = new AbpWebApiClient();
            return client.Post<TResult>(ecserviceUrl, input, timeout);
        }
    }
}

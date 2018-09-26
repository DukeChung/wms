using FortuneLab.WebApiClient.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient
{
    /// <summary>
    /// 专供API端调用第三方API(指非本系统的API)
    /// </summary>
    public class FortuneLabWebApiClient
    {
        /// <summary>
        /// 调用西安各系统API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUrl">API URL中url/前面的部分</param>
        /// <param name="methodName">URL中api/后面的部分</param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static ApiResponse<T> Get<T>(string apiUrl, string methodName, SessionQuery query, bool useEndpointPreffix = true)
        {
            return ApiClient.NExecute<T>(apiUrl, methodName, query, MethodType.Get, useEndpointPreffix: useEndpointPreffix);
        }

        /// <summary>
        /// 调用西安各系统API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUrl">API URL中url/前面的部分</param>
        /// <param name="methodName">URL中api/后面的部分</param>
        /// <param name="query"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static ApiResponse<T> Post<T>(string apiUrl, string methodName, SessionQuery query, object postData = null, bool useEndpointPreffix = true)
        {
            return ApiClient.NExecute<T>(apiUrl, methodName, query, MethodType.Post, postData, useEndpointPreffix: useEndpointPreffix);
        }

        /// <summary>
        /// 调用西安各系统API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiUrl">API URL中url/前面的部分</param>
        /// <param name="methodName">URL中api/后面的部分</param>
        /// <param name="query"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static ApiResponse<T> Put<T>(string apiUrl, string methodName, SessionQuery query, object postData = null, bool useEndpointPreffix = true)
        {
            return ApiClient.NExecute<T>(apiUrl, methodName, query, MethodType.Put, postData, useEndpointPreffix: useEndpointPreffix);
        }


        /// <summary>
        /// 仅调用上海提供的API
        /// 这里会统一处理成我们API返回的ApiResponse<T>对象, 通过 IsSucces返回对象判断请求是否成功/>
        /// </summary>
        /// <typeparam name="T">返回的Dto类型</typeparam>
        /// <param name="apiUrl">API URL中url/前面的部分</param>
        /// <param name="methodName">URL中api/后面的部分</param>
        /// <param name="query">PlatformQuery对象，这个对象必须传入, 后面会处理OperationUserId</param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static ApiResponse<T> CallPlatform<T>(string apiUrl, string methodName, PlatformQuery query, object postData = null, bool isHaveEnvelope = true, MethodType methodType = MethodType.Post)
        {
            if (query == null)
                throw new Exception("PlatformQuery参数不能为null");
            if (isHaveEnvelope)
            {
                var rsp = ApiClient.NExecute<AjaxResponse<T>>(apiUrl, methodName, query, methodType, postData);
                if (rsp.Success && rsp.ResponseResult.Success)
                {
                    return new ApiResponse<T>()
                    {
                        Content = rsp.Content,
                        Exception = rsp.Exception,
                        StatusCode = rsp.StatusCode,
                        ResponseResult = rsp.Success ? rsp.ResponseResult.Result : default(T),
                        Success = rsp.Success && rsp.ResponseResult.Success
                    };
                }
                else
                {
                    var platformErrorRsp = JsonConvert.DeserializeObject<AjaxResponse<ErrorInfo>>(rsp.Content);

                    var resultRsp = new ApiResponse<T>()
                    {
                        Content = rsp.Content,
                        Exception = rsp.Exception,
                        StatusCode = rsp.StatusCode,
                        ResponseResult = rsp.Success ? rsp.ResponseResult.Result : default(T),
                        Success = rsp.Success && rsp.ResponseResult.Success
                    };

                    resultRsp.SetApiMessage(new PlatformApiMessage()
                    {
                        ErrorCode = platformErrorRsp.Error.Code.ToString(),
                        ErrorMessage = platformErrorRsp.Error.Message,
                        Details = platformErrorRsp.Error.Details,
                        ValidationErrors = platformErrorRsp.Error.ValidationErrors
                    });
                    return resultRsp;
                }
            }
            else
            {
                return ApiClient.NExecute<T>(apiUrl, methodName, query, methodType, postData);
            }
        }
    }
}

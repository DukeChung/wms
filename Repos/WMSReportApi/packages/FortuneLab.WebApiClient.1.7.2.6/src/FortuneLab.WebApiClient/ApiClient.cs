using FortuneLab.WebApiClient.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using FortuneLab.WebApiClient.Utils;
using NLog;

namespace FortuneLab.WebApiClient
{
    public static class ApiClient
    {
        private const string EndpointPreffix = "api/";
        private static readonly ILogger Logger;

        static ApiClient()
        {
            Logger = LogManager.GetLogger("FortuneLab.WebApiClient.ApiClient");
        }

        public static ApiResponse<T> Get<T>(string apiUrl, string methodName, CoreQuery query = null, bool useEndpointPreffix = true, Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            return NExecute<T>(apiUrl, methodName, query, useEndpointPreffix: useEndpointPreffix, customerHeader: customerHeader);
        }

        public static ApiResponse<T> Post<T>(string apiUrl, string methodName, CoreQuery query = null, object postData = null, bool useEndpointPreffix = true,
            Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            return NExecute<T>(apiUrl, methodName, query, MethodType.Post, postData, useEndpointPreffix: useEndpointPreffix, customerHeader: customerHeader);
        }

        public static ApiResponse Get(string apiUrl, string methodName, CoreQuery query = null, bool useEndpointPreffix = true, Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            return NExecute(apiUrl, methodName, query, useEndpointPreffix: useEndpointPreffix, customerHeader: customerHeader);
        }

        public static ApiResponse Post(string apiUrl, string methodName, CoreQuery query = null, object postData = null, bool useEndpointPreffix = true,
            Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            return NExecute(apiUrl, methodName, query, MethodType.Post, postData, useEndpointPreffix: useEndpointPreffix, customerHeader: customerHeader);
        }

        public static ApiResponse NExecute(string apiUrl, string methodName, CoreQuery query = null, MethodType method = MethodType.Get, object postData = null, bool useEndpointPreffix = true,
    Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            var rsp = NExecute<string>(apiUrl, methodName, query, method, postData, useEndpointPreffix, customerHeader);
            return rsp as ApiResponse;
        }

        public static ApiResponse<T> NExecute<T>(string apiUrl, string methodName, CoreQuery query = null, MethodType method = MethodType.Get, object postData = null, bool useEndpointPreffix = true,
            Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            var result = new ApiResponse<T>();
            HttpClient client = CreateHttpClient(apiUrl);
            HttpResponseMessage response = null;
            string url = string.Empty;
            var stopWatch = Stopwatch.StartNew();

            try
            {
                result.Content = ExecuteRequestInternal(useEndpointPreffix, methodName, query, method, postData, client, ref response, ref url,
                    responseCallback: res => res.Content.ReadAsStringAsync(),
                    customerHeader: customerHeader);
                result.StatusCode = response.StatusCode;

                //#region Get Remote Duration
                //long remoteDuration = 0;
                //IEnumerable<string> headerValues = null;
                //response.Headers.TryGetValues("Duration", out headerValues);

                //if (headerValues != null && headerValues.Any())
                //    long.TryParse(headerValues.Single(), out remoteDuration);
                //#endregion
                if (!response.IsSuccessStatusCode)
                    return result;

                if (!JsonHelper.IsValidJson(result.Content))
                {
                    throw new Exception("返回内容不是有效的Json格式");
                }
                result.ResponseResult = JsonConvert.DeserializeObject<T>(result.Content);
                result.Success = true;
            }
            catch (Exception ex)
            {
                //如Http请求发生异常,直接记录异常信息
                result.Exception = ex;
                result.Success = false;
                Logger.Error(ex, $"Error Message:{ex.FullMessage()}");
            }
            finally
            {
                stopWatch.Stop();
            }
            return result;
        }

        public static ApiResponse<T> UploadFiles<T>(string apiUrl, string methodName, CoreQuery query = null, MethodType method = MethodType.Get, List<FileUpload> uploadFiles = null,
            bool useEndpointPreffix = true, Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            var result = new ApiResponse<T>();
            HttpClient client = CreateHttpClient(apiUrl);
            HttpResponseMessage response = null;
            string url = string.Empty;
            try
            {
                var sendContent = new MultipartFormDataContent($"{new string('-', 10)}{DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture)}");

                if (uploadFiles != null)
                {
                    foreach (var item in uploadFiles)
                    {
                        StreamContent streamContent = new StreamContent(item.Stream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
                        var originalFileName = $"\"{item.OriginalFileName}{(item.OriginalFileName.IndexOf(".", StringComparison.OrdinalIgnoreCase) < 0 ? ".jpg" : string.Empty)}\"";
                        sendContent.Add(streamContent, item.NewFileName ?? originalFileName, originalFileName);
                    }
                }

                result.Content = ExecuteRequestInternal(useEndpointPreffix, methodName, query, method, sendContent, client, ref response, ref url,
                    requestCallback: (a, b, c, d) => c.PostAsync(d, b as HttpContent).Result,
                    responseCallback: rsp => rsp.Content.ReadAsStringAsync(),
                    customerHeader: customerHeader);

                result.StatusCode = response.StatusCode;
                if (!response.IsSuccessStatusCode)
                    return result;

                result.ResponseResult = JsonConvert.DeserializeObject<T>(result.Content);
                result.Success = true;
            }
            catch (Exception ex)
            {
                //如Http请求发生异常,直接记录异常信息
                result.Exception = ex;
                result.Success = false;
            }
            return result;
        }

        [Obsolete("已过期，请使用DownloadFileV2替换", true)]
        public static ApiResponse<Stream> DownloadFile(string apiUrl, string methodName, CoreQuery query = null, MethodType method = MethodType.Get, object postData = null, bool useEndpointPreffix = true)
        {
            var result = DownloadFileV2(apiUrl, methodName, query, method, postData, useEndpointPreffix);
            return new ApiResponse<Stream>(result) { ResponseResult = result.ResponseResult.Stream };
        }

        public static ApiResponse<FileDownload> DownloadFileV2(string apiUrl, string methodName, CoreQuery query = null, MethodType method = MethodType.Get, object postData = null,
            bool useEndpointPreffix = true, Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            var result = new ApiResponse<FileDownload>();

            HttpClient client = CreateHttpClient(apiUrl);
            HttpResponseMessage response = null;
            string url = string.Empty;
            try
            {
                result.ResponseResult = new FileDownload()
                {
                    Stream = ExecuteRequestInternal(useEndpointPreffix, methodName, query, method, postData, client, ref response, ref url, responseCallback: res => res.Content.ReadAsStreamAsync(),
                    customerHeader: customerHeader),

                    ResponseHeaders = response.Headers,
                    ContentHeader = response.Content.Headers
                };

                result.StatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                    result.Success = true;
            }
            catch (Exception ex)
            {
                //如Http请求发生异常,直接记录异常信息
                result.Exception = ex;
                result.Success = false;
            }
            return result;
        }


        #region 基本HTTP请求

        /// <summary>
        /// 基本HTTP请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="useEndpointPreffix"></param>
        /// <param name="methodName"></param>
        /// <param name="query"></param>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        /// <param name="client"></param>
        /// <param name="response"></param>
        /// <param name="url"></param>
        /// <param name="requestCallback"></param>
        /// <param name="responseCallback"></param>
        /// <param name="customerHeader"></param>
        /// <returns></returns>
        private static T ExecuteRequestInternal<T>(bool useEndpointPreffix,
            string methodName,
            CoreQuery query,
            MethodType method,
            object postData,
            HttpClient client,
            ref HttpResponseMessage response,
            ref string url,
            Func<MethodType, object, HttpClient, string, HttpResponseMessage> requestCallback = null,
            Func<HttpResponseMessage, Task<T>> responseCallback = null,
            Dictionary<string, IEnumerable<string>> customerHeader = null)
        {
            var stopWatch = Stopwatch.StartNew();

            url = $"{(useEndpointPreffix ? EndpointPreffix : string.Empty)}" +
                  $"{(string.IsNullOrEmpty(EndpointPreffix) || EndpointPreffix.EndsWith("/") || methodName.StartsWith("/") ? string.Empty : "/")}" +
                  $"{methodName}?{query}";

            url = AttachToken(url);

            //如果Query==null, 默认传false
            client.DefaultRequestHeaders.Add("IgnoreEnvelope", (query?.IgnoreEnvelope ?? false).ToString());
            if (query is SessionQuery)
                client.DefaultRequestHeaders.Add("SessionKey", (query as SessionQuery).SessionKey);

            if (customerHeader != null)
            {
                foreach (var keyValuePair in customerHeader)
                {

                    switch (keyValuePair.Key)
                    {
                        case nameof(client.DefaultRequestHeaders.Authorization):
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", string.Join(",", keyValuePair.Value));
                            break;
                        default:
                            client.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                            break;
                    }
                }
            }

            string requestData;
            if (postData is MultipartContent)
            {
                requestData = "MultipartContent";
            }
            else
            {
                requestData = (JsonConvert.SerializeObject(postData) ?? string.Empty).GetLimitString();
            }

            var requestHeader = JsonConvert.SerializeObject(client.DefaultRequestHeaders);
            T responseContent = default(T);
            try
            {
                using (client)
                {
                    if (requestCallback != null)
                    {
                        response = requestCallback(method, postData, client, url);
                    }
                    else
                    {
                        switch (method)
                        {
                            case MethodType.Get:
                                response = client.GetAsync(url).Result;
                                break;
                            case MethodType.Post:
                                response = client.PostAsJsonAsync(url, postData).Result;
                                break;
                            case MethodType.Delete:
                                response = client.DeleteAsync(url).Result;
                                break;
                            case MethodType.Put:
                                response = client.PutAsJsonAsync(url, postData).Result;
                                break;
                        }
                    }
                }

                if (responseCallback == null)
                    throw new ApiClientLogicException("Response CallBack不能为空");

                if (response == null)
                    throw new ApiClientLogicException("HttpClient response 不能为空");

                responseContent = responseCallback(response).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiClientLogicException($"远程API地址{url}调用失败, 返回状态:{response.StatusCode}, 详细错误参考ResponseContent");
                }

                LogResponseInfo(method.ToString(), $"{client.BaseAddress}{url}", requestHeader, requestData, response, responseContent, stopWatch.ElapsedMilliseconds);
            }
            catch (ApiClientLogicException ex)
            {
                LogResponseInfo(method.ToString(), $"{client.BaseAddress}{url}", requestHeader, requestData, response, responseContent, stopWatch.ElapsedMilliseconds, ex);
            }
            catch (Exception ex)
            {
                LogResponseInfo(method.ToString(), $"{client.BaseAddress}{url}", requestHeader, requestData, response, responseContent, stopWatch.ElapsedMilliseconds, ex);
                throw;
            }
            return responseContent;
        }

        internal class ApiClientLogicException : Exception
        {
            public ApiClientLogicException(string message) : base(message)
            {

            }
        }

        #endregion

        #region 日志处理

        private static void LogResponseInfo(string method, string url, string requestHeader, string requestBody, HttpResponseMessage response, object responseContent, long duration, Exception exception = null)
        {
            var theEvent = new LogEventInfo
            {
                Level = exception == null && response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Error,
                LoggerName = "FortuneLab.Monitor.WebApiClient.Request",
                Message = exception?.Message,
                Exception = exception
            };

            theEvent.Properties["RequestPathInfo"] = url;
            theEvent.Properties["RequestMethod"] = method;
            theEvent.Properties["RequestHeader"] = requestHeader;
            theEvent.Properties["RequestBody"] = requestBody;
            if (response != null)
            {
                theEvent.Properties["ResponseHeader"] = JsonConvert.SerializeObject(response.Headers);
                theEvent.Properties["ResponseContentHeader"] = JsonConvert.SerializeObject(response.Content.Headers);
            }
            if (responseContent is Stream)
            {
                theEvent.Properties["ResponseContent"] = "Stream Content";
            }
            else
            {
                theEvent.Properties["ResponseContent"] = JsonConvert.SerializeObject(responseContent).GetLimitString();
            }
            theEvent.Properties["ExecuteDuration"] = duration;
            theEvent.Properties["RequestTraceId"] = HttpContext.Current?.Items["RequestTraceId"]?.ToString();
            theEvent.Properties["RequestCaller"] = "ApiClient";
            theEvent.Properties["RequestFlag"] = "ApiClientFlag";
            LogManager.GetLogger(theEvent.LoggerName).Log(theEvent);
        }
        #endregion

        #region Http请求及URL辅助方法
        private static string AttachToken(string url)
        {
            var apiToken = WebConfigurationManager.AppSettings["APITOKEN"];
            if (!string.IsNullOrEmpty(apiToken))
            {
                url = string.Concat(url, "&APITOKEN=", HttpUtility.UrlEncode(apiToken));
            }
            return url;
        }

        private static HttpClient CreateHttpClient(string apiUrl)
        {
            if (!apiUrl.EndsWith("/"))
                apiUrl += "/";
            var client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (HttpContext.Current != null)
            {
                client.DefaultRequestHeaders.Add("UserAgent", HttpContext.Current.Request.UserAgent);
                client.DefaultRequestHeaders.Add("UserAddr", GetClientIp(HttpContext.Current));

                if (HttpContext.Current.Items.Contains("RequestTraceId"))
                {
                    client.DefaultRequestHeaders.Add("RequestTraceId", HttpContext.Current.Items["RequestTraceId"].ToString());
                }
            }
            return client;
        }

        private static string GetClientIp(HttpContext filterContext)
        {
            var myRequest = filterContext.Request;
            var ip = myRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ip) ? ip.Split(',').SingleOrDefault() : myRequest.ServerVariables["REMOTE_ADDR"];
        }
        #endregion
    }
}

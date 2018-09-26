using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FortuneLab.WebApiClient.Utils;

namespace FortuneLab.WebApiClient
{
    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse()
        {

        }

        public ApiResponse(ApiResponse apiReponse)
            : base(apiReponse)
        {

        }

        public T ResponseResult { get; set; }
    }

    public class ApiResponse : IApiResponse
    {
        public ApiResponse()
        { }

        public ApiResponse(ApiResponse apiResponse)
        {
            this.Success = apiResponse.Success;
            this.Exception = apiResponse.Exception;
            this.Content = apiResponse.Content;
            this.StatusCode = apiResponse.StatusCode;
        }

        [JsonIgnore]
        private ApiMessage _apiMessage;

        [JsonIgnore]
        private string _content;

        [Obsolete("请使用ApiMessage.Message替换, 目前此属性不再有值", true)]
        public string Message { get; set; }

        [JsonIgnore]
        public bool Success { get; set; }

        [JsonIgnore]
        public Exception Exception { get; set; }

        [JsonIgnore]
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
            }
        }

        [JsonIgnore]
        public ApiMessage ApiMessage
        {
            get
            {
                if (Success)
                    return null;

                if (_apiMessage == null && !string.IsNullOrEmpty(Content))
                {
                    if (JsonHelper.IsValidJson(Content))
                    {
                        try
                        {
                            _apiMessage = ApiMessage.ToMessage(Content);
                        }
                        catch (Exception ex)
                        {
                            _apiMessage = new ApiMessage()
                            {
                                ErrorCode = "ContentFormatException",
                                ErrorMessage = ex.FullMessage()
                            };
                        }
                    }
                    else
                    {
                        _apiMessage = new ApiMessage()
                        {
                            ErrorCode = "InvalidFormatForResponseContent",
                            ErrorMessage = Exception.FullMessage()
                        };
                    }
                }

                if (_apiMessage == null || string.IsNullOrWhiteSpace(_apiMessage.ErrorMessage))
                {
                    return _apiMessage = new ApiMessage()
                    {
                        ErrorCode = "UnknowError",
                        ErrorMessage = Exception == null ? StatusCode.ToString() : Exception.FullMessage()
                    };
                }
                return _apiMessage;
            }
            private set { _apiMessage = value; }
        }

        public void SetApiMessage(ApiMessage apiMessage)
        {
            this._apiMessage = apiMessage;
        }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public virtual bool IsError
        {
            get { return !Success; }
        }
    }

    public interface IApiResponse
    {
        ApiMessage ApiMessage { get; }

        string Message { get; }
    }
}

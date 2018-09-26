using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuneLab.WebApiClient
{
    public class ApiMessage
    {
        public string ErrorCode { get; set; }

        [Obsolete("请使用ErrorMessage替换", true)]
        public string Message => ErrorMessage;

        public string ErrorMessage { get; set; }

        public static ApiMessage ToMessage(string content)
        {
            return JsonConvert.DeserializeObject<ApiMessage>(content);
        }

        public override string ToString()
        {
            return $"{ErrorMessage}";
        }
    }

    public class PlatformApiMessage : ApiMessage
    {
        //
        // 摘要:
        //     Error details.
        public string Details { get; set; }
        //
        // 摘要:
        //     Validation errors if exists.
        public ValidationErrorInfo[] ValidationErrors { get; set; }
    }
}

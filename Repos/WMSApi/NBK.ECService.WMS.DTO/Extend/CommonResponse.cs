using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class CommonResponse
    {
        public CommonResponse()
        {
            IsSuccess = true;
        }

        public CommonResponse(bool? isSuccess = false, string errorCode = "", string errorMessage = "")
        {
            IsSuccess = isSuccess.Value;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; set; }

        public bool IsAsyn { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }
    }
}

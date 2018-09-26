using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO
{
    public class TMSCommonResponse
    {
        public TMSCommonResponse()
        {
        }

        public TMSCommonResponse(string messageCode = "", string message = "")
        {
            MessageCode = messageCode;
            Message = message;
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string MessageCode { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
    }
}
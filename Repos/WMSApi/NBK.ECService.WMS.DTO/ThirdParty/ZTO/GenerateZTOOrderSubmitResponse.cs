using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class GenerateZTOOrderSubmitResponse
    {
        public GenerateZTOOrderSubmitResponseData data { get; set; }

        public string messgage { get; set; }

        public bool result { get; set; }
    }

    public class GenerateZTOOrderSubmitResponseData
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string billCode { get; set; }

        public string message { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string orderId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class GenerateZTOOrderMarkeResponse
    {
        public string message { get; set; }

        public GenerateZTOOrderMarkeResponseResult result { get; set; }

        public bool status { get; set; }

        public string statusCode { get; set; }
    }

    public class GenerateZTOOrderMarkeResponseResult
    {
        /// <summary>
        /// 集包地
        /// </summary>
        public string bagAddr { get; set; }

        /// <summary>
        /// 大头笔
        /// </summary>
        public string mark { get; set; }
    }
}

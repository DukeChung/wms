using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class GenerateZTOOrderMarkeRequest
    {
        /// <summary>
        /// 唯一标示
        /// </summary>
        public string unionCode { get; set; }

        /// <summary>
        /// 发件省
        /// </summary>
        public string send_province { get; set; }

        /// <summary>
        /// 发件市
        /// </summary>
        public string send_city { get; set; }

        /// <summary>
        /// 发件区（县/镇）
        /// </summary>
        public string send_district { get; set; }

        /// <summary>
        /// 发件详细地址
        /// </summary>
        public string send_address { get; set; }

        /// <summary>
        /// 收件省
        /// </summary>
        public string receive_province { get; set; }

        /// <summary>
        /// 收件市
        /// </summary>
        public string receive_city { get; set; }

        /// <summary>
        /// 收件区（县/镇）
        /// </summary>
        public string receive_district { get; set; }

        /// <summary>
        /// 收件详细地址
        /// </summary>
        public string receive_address { get; set; }
    }
}

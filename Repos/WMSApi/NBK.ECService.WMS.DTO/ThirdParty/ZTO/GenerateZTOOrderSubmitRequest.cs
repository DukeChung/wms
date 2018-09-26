using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class GenerateZTOOrderSubmitRequest
    {
        /// <summary>
        ///商家ID(正式环境由中通合作网点提供，测试环境使用test)
        /// </summary>
        public string partner { get; set; }

        /// <summary>
        ///订单号由合作商平台产生，具有平台唯一性。(测试的id一律使用xfs101100111011)
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 订单类型id具体可查看 订单类型接口 默认为1 如果不传或者传空自动为1
        /// </summary>
        public string typeid { get; set; } 

        /// <summary>
        /// 发件人信息
        /// </summary>
        public GenerateZTOOrderSubmitRequestSender sender { get; set; }

        /// <summary>
        /// 收件人信息
        /// </summary>
        public GenerateZTOOrderSubmitRequestReceiver receiver { get; set; }

        /// <summary>
        /// 订单类型：0标准；1代收；2到付
        /// </summary>
        public string order_type { get; set; }

    }

    public class GenerateZTOOrderSubmitRequestSender
    {
        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 发件人手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 发件人市 eg:上海市,上海市,青浦区
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 发件人路名门牌等地址信息 eg:华新镇华志路123号
        /// </summary>
        public string address { get; set; }

    }

    public class GenerateZTOOrderSubmitRequestReceiver
    {
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 收件人手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 收件人所在城市，逐级指定，用英文半角逗号分隔 eg:四川省,成都市,武侯区
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 收件人路名门牌等地址信息 eg:育德路497号
        /// </summary>
        public string address { get; set; }
    }
}

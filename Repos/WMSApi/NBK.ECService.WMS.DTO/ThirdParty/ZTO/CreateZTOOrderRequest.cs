using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty.ZTO
{
    public class CreateZTOOrderRequest
    {
        /// <summary>
        /// 订单号，由合作商平台产生，具有平台唯一性。
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 订单类型id具体可查看 订单类型接口
        /// </summary>
        public string typeid { get; set; }

        public sender sender { get; set; }

        public receiver receiver { get; set; }

    }

    public class sender
    {
        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 发件公司名
        /// </summary>
        public string company { get; set; }

        /// <summary>
        /// 发件人手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 发件人电话号码
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 发件人所在城市，必须逐级指定，用英文半角逗号分隔，目前至少需要指定到区县级，如能往下精确更好，如“上海市,上海市,青浦区,华新镇,华志路,123号”
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 发件人路名门牌等地址信息
        /// </summary>
        public string address { get; set; } 

    }

    public class receiver
    {
        /// <summary>
        ///收件人姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 收件公司名
        /// </summary>
        public string company { get; set; }

        /// <summary>
        /// 收件人手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 收件人电话号码
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 收件人所在城市，逐级指定，用英文半角逗号分隔
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 收件人路名门牌等地址信息
        /// </summary>
        public string address { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSReport.Utility.Redis
{
    /// <summary>
    /// WMS业务系统缓存Key
    /// </summary>
    public static class RedisSourceKey
    {
        public static string ReceiptOutboundChart = "ReceiptOutboundChart{0}";

        public static string OutboundAndReturnCharData = "OutboundAndReturnCharData{0}";

        public static string RedisMenuList = "MenuList";

        public static string RedisWareHouseList = "WareHouseList";

        public static string RedisSkuList = "SkuList";

        public static string RedisPack = "PackList";

        public static string RedisUOMList = "UOMList";

        public static string RedisOutboundReview = "OutboundReview|{0}|{1}";

        public static string RedisOutboundReviewScanning = "OutboundReviewScanning|{0}|{1}";

        public static string RedisLoginUserList = "LoginUserList";
    }
}

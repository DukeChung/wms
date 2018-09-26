using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Utility
{
    /// <summary>
    /// 缓存名称
    /// </summary>
    public static class RedisSourceKey
    {

        public static string ReceiptOutboundChart = "ReceiptOutboundChart{0}";

        public static string OutboundAndReturnCharData = "OutboundAndReturnCharData{0}";

        public static string RedisMenuList = "MenuList";

        public static string RedisSkuList = "SkuList";

        public static string RedisPack = "PackList";

        public static string RedisUOMList = "UOMList";

        public static string RedisWareHouseList = "WareHouseList";

        public static string RedisOutboundReview = "OutboundReview|{0}|{1}";

        public static string RedisOutboundReviewScanning = "OutboundReviewScanning|{0}|{1}";

        public static string RedisLoginUserList = "LoginUserList";

        public static string RedisGenOrderList = "GenOrderList";
    }
}

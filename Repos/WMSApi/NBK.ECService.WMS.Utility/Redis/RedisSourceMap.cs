using System;

namespace NBK.ECService.WMS.Utility.Redis
{
    public static class RedisSourceMap
    {
        /// <summary>
        /// 检查是否启用Redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool CheckRedis<T>()
        {
            var result = false;
            Type type = typeof(T);
            switch (type.Name)
            {
                case "menu":
                    result = true;
                    break;
                case "sku1":
                    result = true;
                    break;
                case "uom1":
                    result = true;
                    break;
                case "pack1":
                    result = true;
                    break;
                case "skuclass1":
                    result = true;
                    break;
                case "lottemplate1":
                    result = true;
                    break;
                case "warehouse1":
                    result = true;
                    break;
                case "syscode1":
                    result = true;
                    break;
                case "syscodedetail1":
                    result = true;
                    break;
            }
            return result;
        }
    }

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

        public static string RedisOutboundReview = "OutboundReview|{0}|{1}";

        public static string RedisOutboundReviewScanning = "OutboundReviewScanning|{0}|{1}";

        public static string RedisOutboundReviewDiff = "OutboundReviewDiff|{0}|{1}";

        public static string RedisLoginUserList = "LoginUserList";

        public static string RedisGenOrderList = "GenOrderList";

        public static string RedisWareHouseList = "WareHouseList";

        public static string RedisRFPicking = "RFPicking|{0}|{1}";

        public static string RedisSetAccessLog = "SetAccessLog";
    }
}
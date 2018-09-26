using BachLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMSBussinessApi.Utility
{
    public class PublicConst
    {
        public static string RedisWMSHostName = Config.AppSetting.Get("Redis:RedisWMSHostName");

        /// <summary>
        /// 请求服务地址
        /// </summary>
        public static string RabbitWMSHostName = Config.AppSetting.Get("RabbitMQ:RabbitWMSHostName");

        /// <summary>
        /// 请求服务用户名
        /// </summary>
        public static string RabbitWMSUserName = Config.AppSetting.Get("RabbitMQ:RabbitWMSUserName");

        /// <summary>
        /// 请求服务密码
        /// </summary>
        public static string RabbitWMSPassword = Config.AppSetting.Get("RabbitMQ:RabbitWMSPassword");

        public static string Exchange = "WMS_MQ_Business";

        public const string InterfaceUserId = "99999";
        public const string InterfaceUserName = "wms";


        public const string SyncCreateSku = "多仓同步创建商品";
        public const string SyncUpdateSku = "多仓同步更新商品";

        public const string SyncCreatePack = "多仓同步创建商品包装";
        public const string SyncUpdatePack = "多仓同步更新商品包装";
        public const string SyncDeletePack = "多仓同步删除商品包装";

        public const string SyncCreateSyscode = "多仓同步创建系统代码";
        public const string SyncCreateSyscodeDetail = "多仓同步创建系统代码明细";
    }
}

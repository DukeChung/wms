using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Utility
{
    public class PublicConst
    {
        public const string BussinessType_Inbound = "入库";

        public const string BussinessType_Outbound = "出库";

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 请求服务地址
        /// </summary>
        public static string RedisWMSHostName = ConfigurationManager.AppSettings["RedisWMSHostName"];

        #region 检测库存数据库地址
        /// <summary>
        /// 数据库链接地址1
        /// </summary>
        public static string ConnectionString1 = ConfigurationManager.AppSettings["ConnectionString1"];
        /// <summary>
        /// 数据库链接地址2
        /// </summary>
        public static string ConnectionString2 = ConfigurationManager.AppSettings["ConnectionString2"];
        /// <summary>
        /// 数据库链接地址3
        /// </summary>
        public static string ConnectionString3 = ConfigurationManager.AppSettings["ConnectionString3"];

        #endregion 


        /// <summary>
        /// 表示用户为WMS系统
        /// </summary>
        public const int WMSUserID = 66666;
        public const string WMSUserName = "WMS系统";

        /// <summary>
        /// 系统消息类型
        /// </summary>
        public const string MessageType_SystemNotice = "SystemNotice";

        #region 友盟消息推送
        /// <summary>
        /// 友盟Appkey
        /// </summary>
        public static string UmengAppKey = ConfigurationManager.AppSettings["UmengAppKey"];
        /// <summary>
        /// 友盟推送Url
        /// </summary>
        public static string UmengPushUrl = ConfigurationManager.AppSettings["UmengPushUrl"];
        public static string UmengMessageSecret = ConfigurationManager.AppSettings["UmengMessageSecret"];
        public static string UmengAppMasterSecret = ConfigurationManager.AppSettings["UmengAppMasterSecret"];


        /// <summary>
        /// 根据传入webconfig名称获取url
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetWebConfigUrl(string configName)
        {
            var result = ConfigurationManager.AppSettings[configName];
            return result;
        }
        #endregion

        //基础数据同步
        public static string WmsBizApiUrl = ConfigurationManager.AppSettings["WMSBizAPIURL"];
        public static bool SyncMultiWHSwitch = Convert.ToBoolean(ConfigurationManager.AppSettings["SyncMultiWHSwitch"]);
    }
}

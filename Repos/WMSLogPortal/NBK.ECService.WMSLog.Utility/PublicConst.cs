using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMSLog.Utility
{
    public static class PublicConst
    {

        public static string WmsLogApiUrl = ConfigurationManager.AppSettings["WMSLOGAPIURL"];

        public static string WmsApiUrl = ConfigurationManager.AppSettings["WMSAPIURL"];

        /// <summary>
        /// 统计日志记录范围前多少天
        /// </summary>
        public static int ReportDays = int.Parse(ConfigurationManager.AppSettings["ReportDays"]);

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    }
}

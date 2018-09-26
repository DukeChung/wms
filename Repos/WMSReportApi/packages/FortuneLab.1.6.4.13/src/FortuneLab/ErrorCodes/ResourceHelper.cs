using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace FortuneLab.ErrorCodes
{
    /// <summary>
    /// 从XML配置获取错误代码提示信息
    /// Author: Ken Wang
    /// </summary>
    public class ResourceHelper
    {
        private static ErrorCode ErrorMsg;
        private static string ResourcePath = ConfigurationManager.AppSettings["ResourcePath"];
        static ResourceHelper()
        {
            ErrorMsg = GetErrorMessage();
        }

        private static ErrorCode GetErrorMessage()
        {
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/"), ResourcePath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            using (StringReader sr = new StringReader(xmlDoc.OuterXml))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(ErrorCode));
                ErrorCode errorInfo = xmldes.Deserialize(sr) as ErrorCode;
                return errorInfo;
            }
        }

        public static ErrorMessage GetMessage(string errorKey)
        {
            var errorInfo = ErrorMsg.ErrorCodeList.FirstOrDefault(p => p.Key == errorKey);
            if (errorInfo == null)
            {
                return new ErrorMessage() { Code = "001", Msg = string.Format("ErrorKey: {0} 未找到, 资源文件: {1}", errorKey, ResourcePath) };
            }
            return errorInfo;
        }
    }
}
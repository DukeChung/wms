using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Abp.Utils
{
    public static partial class StringHelper
    {
        /// <summary>
        /// 从Http请求中获取指定参数的值，顺序依次是Form、QueryString、Cookies
        /// </summary>
        /// <param name="paramName">参数值</param>
        /// <returns>返回参数值，或者空字符串</returns>
        public static string GetValueFromHttpRequest(string paramName)
        {
            var result = "";
            if (HttpContext.Current == null) return result;

            var Request = HttpContext.Current.Request;
            if (Request.Form.AllKeys.Contains(paramName, StringComparer.OrdinalIgnoreCase))
            {
                result = Request.Form[paramName];
            }
            else if (Request.QueryString.AllKeys.Contains(paramName, StringComparer.OrdinalIgnoreCase))
            {
                result = Request.QueryString[paramName];
            }
            else if (Request.Cookies.AllKeys.Contains("__scoped_" + paramName, StringComparer.OrdinalIgnoreCase))
            {
                result = Request.Cookies[paramName].Value;
            }
            return result;
        }
    }
}

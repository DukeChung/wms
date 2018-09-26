using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Common
{
    /// <summary>
    /// Web API文件上传时的文件流处理类
    /// 这里的文件名是原文件名+随机的时间字符串，如果需要特别定制，可以直接重写这个类，通过override GetLocalFile来实现。
    /// </summary>
    public class NamedMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public NamedMultipartFormDataStreamProvider(string fileName)
            : base(fileName)
        {
        }
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string fileName = headers.ContentDisposition.Name ?? headers.ContentDisposition.FileName; //base.GetLocalFileName(headers);
            fileName = fileName.Trim(new char[] { '"' });
            if (fileName.IndexOf('.') < 0)
            {
                var originalFileName = headers.ContentDisposition.FileName.Trim(new char[] { '"' });
                fileName += new FileInfo(originalFileName).Extension;
            }
            if (!string.IsNullOrEmpty(headers.ContentDisposition.FileName))
            {
                fileName = headers.ContentDisposition.FileName;
            }
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Trim('"');
            }
            if (fileName.StartsWith(" "))
            {
                fileName = fileName.Replace(" ", "_");
            }
            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
            {
                fileName = Path.GetFileName(fileName);
            }
            fileName = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.Ordinal)) + "_" + Utilities.CurrentTimeMillis() + fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal));
            return fileName;
        }

        public class Utilities
        {
            public static long CurrentTimeMillis()
            {
                return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            public static string EncryptionMD5(string toCryString)
            {
                MD5CryptoServiceProvider hashmd5;
                hashmd5 = new MD5CryptoServiceProvider();
                return BitConverter.ToString(hashmd5.ComputeHash(Encoding.UTF8.GetBytes(toCryString))).Replace("-", "").ToLower();
            }
        }
    }
}

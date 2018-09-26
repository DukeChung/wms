using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Common
{
    public class ETagAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Filter = new ETagFilter(filterContext.HttpContext.Response);
            }
            catch (System.Exception)
            {
                // Do Nothing
            };
        }
    }

    public class ETagFilter : MemoryStream
    {
        private HttpResponseBase o = null;
        private Stream filter = null;

        public ETagFilter(HttpResponseBase response)
        {
            o = response;
            filter = response.Filter;
        }

        private string GetToken(Stream stream)
        {
            byte[] checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            filter.Write(data, 0, count);
            o.AddHeader("ETag", GetToken(new MemoryStream(data)));
        }
    }
}
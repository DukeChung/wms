using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Optimization.HttpCache
{
    /// <summary>
    /// 仅根据客户机上次访问的时间戳进行简单缓存处理
    /// </summary>
    public class LazyCacheFilter : ActionFilterAttribute
    {
        #region Properties
        public long Seconds = 30;
        public TimeSpan MaxAge = new TimeSpan(0, 0, 0, 30);
        #endregion

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;
            string sinceTag = request.Headers["If-Modified-Since"];
            if (sinceTag != null && TimeSpan.FromTicks(DateTime.Now.Ticks - DateTime.Parse(sinceTag).Ticks).TotalSeconds < Seconds)
            {
                response.StatusCode = 304;
                response.StatusDescription = "Not Modified";
            }
            else
                SetClientCaching(filterContext.HttpContext, DateTime.Now);
        }

        #region private Helper Methods

        protected virtual void SetClientCaching(HttpContextBase context, DateTime time)
        {
            var cache = context.Response.Cache;
            cache.SetETag(time.Ticks.ToString());

            cache.SetLastModified(time);
            cache.SetMaxAge(MaxAge);
            cache.SetSlidingExpiration(true);
            cache.SetCacheability(HttpCacheability.Public);
        }
        #endregion
    }
}

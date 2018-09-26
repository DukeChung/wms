using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Abp.Web.WebApi
{
    public class BaseActionFilterAttribute : ActionFilterAttribute, IBaseAttribute
    {
        public const string SessionKeyName = "SessionKey";
        public const string TokenName = "Token";
        public const string LogonUserName = "LogonUser";

        public int Order { get; set; }

        public BaseActionFilterAttribute()
        {
            Order = 0;
        }
        public BaseActionFilterAttribute(int order)
        {
            Order = order;
        }
    }

    public interface IBaseAttribute : IFilter
    {
        int Order { get; set; }
    }
}

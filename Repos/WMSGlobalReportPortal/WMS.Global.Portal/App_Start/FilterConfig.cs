using WMS.Global.Portal.Controllers;
using System.Web;
using System.Web.Mvc;

namespace WMS.Global.Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SetUserInfoAttribute());
        }
    }
}

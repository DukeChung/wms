using System.Web;
using System.Web.Mvc;

namespace NBK.ECService.WMSReport.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Common
{
    public class MenuNavNameAttribute : ActionFilterAttribute
    {
        public MenuNavNameAttribute(string firstNavName = null, string secondNavName = null, string thridNavName = null)
        {
            FirstNavName = firstNavName;
            SecondNavName = secondNavName;
            ThirdNavName = thridNavName;
        }
        public string FirstNavName { get; set; }
        public string SecondNavName { get; set; }
        public string ThirdNavName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.FirstNavName = FirstNavName;
            filterContext.Controller.ViewBag.SecondNavName = SecondNavName;
            filterContext.Controller.ViewBag.ThirdNavName = ThirdNavName;
        }
    }

    public class MainNavMenuAttribute : ActionFilterAttribute
    {
        public string Main1stNavName { get; set; }
        public string Main2ndNavName { get; set; }
        public string Main3rdNavName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.Main1stNavName = Main1stNavName;
            filterContext.Controller.ViewBag.Main2ndNavName = Main2ndNavName;
            filterContext.Controller.ViewBag.Main3rdNavName = Main3rdNavName;
        }
    }
}

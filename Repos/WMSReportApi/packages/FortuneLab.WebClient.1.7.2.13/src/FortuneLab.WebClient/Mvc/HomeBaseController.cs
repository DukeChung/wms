using System.Configuration;
using System.Web.Mvc;

namespace FortuneLab.WebClient.Mvc
{
    public class HomeBaseController : BaseController
    {
        /// <summary>
        /// 所有项目的默认入口地址
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            if (Request.IsLocal)
            {
                return View();
            }

            return Redirect(string.Format("http://wf.{0}", ConfigurationManager.AppSettings["domain"]));
        }
    }
}

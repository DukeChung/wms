﻿#region Using

using System.Web.Mvc;

#endregion

namespace WMS.Report.Portal.Controllers
{
    public class EmailTemplatesController : Controller
    {
        // GET: /emailtemplates/basic
        public ActionResult Basic()
        {
            return View();
        }

        // GET: /emailtemplates/sidebar
        public ActionResult SideBar()
        {
            return View();
        }

        // GET: /emailtemplates/hero
        public ActionResult Hero()
        {
            return View();
        }

        // GET: /emailtemplates/sidebarhero
        public ActionResult SideBarHero()
        {
            return View();
        }

        // GET: /emailtemplates/newsletter
        public ActionResult NewsLetter()
        {
            return View();
        }
    }
}
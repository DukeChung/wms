﻿#region Using

using System.Web.Mvc;

#endregion

namespace WMSGlobalReportPortal.Controllers
{

    public class HomeController : Controller
    {
        // GET: home/index
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderDetail()
        {

            return View();
        }


        public ActionResult Dome()
        {
            return View();
        }


        public ActionResult Social()
        {
            return View();
        }

        // GET: home/inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/widgets
        public ActionResult Widgets()
        {
            return View();
        }

        // GET: home/chat
        public ActionResult Chat()
        {
            return View();
        }
    }
}
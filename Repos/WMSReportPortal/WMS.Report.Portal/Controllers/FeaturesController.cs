﻿#region Using

using System.Web.Mvc;

#endregion

namespace WMS.Report.Portal.Controllers
{
    //[Authorize]
    public class FeaturesController : Controller
    {
        // GET: home/calendar
        public ActionResult Calendar()
        {
            return View();
        }

        // GET: home/google-map
        public ActionResult GoogleMap()
        {
            return View();
        }
    }
}
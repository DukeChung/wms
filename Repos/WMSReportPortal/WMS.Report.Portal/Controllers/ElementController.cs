﻿#region Using

using System.Web.Mvc;

#endregion

namespace WMS.Report.Portal.Controllers
{
    //[Authorize]
    public class ElementController : Controller
    {
        // GET: /elements/general/
        public ActionResult General()
        {
            return View();
        }

        // GET: /elements/buttons/
        public ActionResult Buttons()
        {
            return View();
        }
    }
}
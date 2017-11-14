using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Upload.Dashboard.Areas.Upload.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Upload/Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
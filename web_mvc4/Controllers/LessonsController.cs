using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web_mvc4.Controllers
{
    public class LessonsController : Controller
    {
        //
        // GET: /Lessons/

        public ActionResult Index()
        {
            return View();
        }

    }
}

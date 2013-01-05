using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using admin.Code;
using Newtonsoft.Json;

namespace admin.Controllers
{
    public class RegexBuilderController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult LoadNamespace(string id)
		{
			var types = NamespaceWorker.GetFullNamespaceInfo(id);
			return Content(JsonConvert.SerializeObject(types));
		}
    }
}

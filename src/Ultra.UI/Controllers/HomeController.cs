using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		public HomeController()
		{
		}

		[Route("")]
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			var runner = new JmxRunner();
			runner.Run("C:\\utils\\apache-jmeter-2.9\\RealWorld.jmx", new JmxSettings {
				Domain = "uat.shopyourway.com", Duration = 1800, RampUp = 150
			});

			return View();
		}
	}
}

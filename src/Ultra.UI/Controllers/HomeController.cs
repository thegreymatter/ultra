using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Controllers.Plumbing;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		private readonly IJmxRunner _jmxRunner;

		public HomeController(IJmxRunner jmxRunner)
		{
			_jmxRunner = jmxRunner;
		}

		[Route("")]
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			_jmxRunner.Run("C:\\utils\\apache-jmeter-2.9\\RealWorld.jmx", new JmxSettings
			{
				Domain = "uat.shopyourway.com",
				Duration = 60,
				RampUp = 150
			});

			return Content("Gilly The King");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		private readonly IStorage<TestEntity> _testStorage;

		public HomeController(IStorage<TestEntity> testStorage)
		{
			_testStorage = testStorage;
		}

		[Route("")]
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			return View();
		}
	}
}

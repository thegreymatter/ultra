using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.Builders;
using Ultra.Config.Routes;
using Ultra.Controllers.Plumbing;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Dal.Repositories;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILoadRunRepository _loadRunRepository;

		public HomeController(ILoadRunRepository loadRunRepository)
		{
			_loadRunRepository = loadRunRepository;
		}

		[Route("")]
		public ActionResult Index()
		{
			var loadRuns = _loadRunRepository.GetMostRecentLoadRuns(10);

			return View(loadRuns);
		}
	}
}

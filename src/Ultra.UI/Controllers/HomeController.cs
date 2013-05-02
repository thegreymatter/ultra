using System;
using System.Web.Mvc;
using MongoDB.Bson;
using Ultra.Config.Routes;
using Ultra.Dal.Repositories;

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

		[Route("analysis/{runId}")]
		public ActionResult Analysis(string runId)
		{
			ObjectId loadRunId;
			if (!ObjectId.TryParse(runId, out loadRunId))
				throw new Exception("Could not parse the given runId");

			var loadRun = _loadRunRepository.GetLoadRun(loadRunId);
			// TODO: open the output file, and serialize

			return View();
		}

		[Route("run")]
		public ActionResult Run()
		{
			return View();
		}
	}
}

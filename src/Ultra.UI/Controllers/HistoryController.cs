using System.Web.Mvc;
using MongoDB.Bson;
using Ultra.Config.Routes;
using Ultra.Dal.Repositories;

namespace Ultra.Controllers
{
	public class HistoryController : Controller
	{
		private readonly ILoadRunRepository _loadRunRepository;

		public HistoryController(ILoadRunRepository loadRunRepository)
		{
			_loadRunRepository = loadRunRepository;
		}

		[Route("")]
		public ActionResult RunsHistory()
		{
			var loadRuns = _loadRunRepository.GetMostRecentLoadRuns(10);

			return View(loadRuns);
		}

		[Route("-/delete-run")]
		public ActionResult DeleteRun(string loadRunId)
		{
			// TODO: remove the files as well
			var runId = ObjectId.Parse(loadRunId);
			_loadRunRepository.DeleteLoadRun(runId);

			return Json("OK");
		}
	}
}

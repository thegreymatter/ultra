using System;
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
		public ActionResult RunsHistory(int skip = 0)
		{
			skip = Math.Max(0, skip);
			var loadRuns = _loadRunRepository.GetMostRecentLoadRuns(10,skip);

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

		[Route("-/save-label")]
		public ActionResult SaveLabel(string loadRunId,string newLabel)
		{
			// TODO: remove the files as well
			var runId = ObjectId.Parse(loadRunId);
			var run = _loadRunRepository.GetLoadRun(runId);
			run.Label = newLabel;
			_loadRunRepository.SaveOrUpdate(run);

			return Json("OK");
		}
	}
}

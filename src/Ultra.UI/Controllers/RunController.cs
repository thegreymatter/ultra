using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;
using Ultra.Models;

namespace Ultra.Controllers
{
	public class RunController : Controller
	{
		private readonly ILoadRunRepository _loadRunRepository;

		public RunController(ILoadRunRepository loadRunRepository)
		{
			_loadRunRepository = loadRunRepository;
		}

		[Route("run")]
		public ActionResult Run()
		{
			var jmxFileDirectory = ConfigurationManager.AppSettings["JmxScripts"];
			var jmxFiles = Directory.GetFiles(Path.Combine(Server.MapPath("/"), jmxFileDirectory), "*.jmx", SearchOption.TopDirectoryOnly);

			var loadRuns = _loadRunRepository.GetMostRecentLoadRuns(10);

			return View(new RunPageModel {
				JmxFiles = jmxFiles, LoadRuns = loadRuns
			});
		}

		[Route("-/start-run")]
		public ActionResult StartRun(string filename, JmxSettings settings)
		{
			_loadRunRepository.CreateLoadRun(new LoadRun {
				Domain = settings.Domain,
				Duration = settings.Duration,
				JmxFilename = filename,
				RampUp = settings.RampUp,
				Status = LoadRunStatus.Pending
			});

			return Json("OK");
		}
	}
}

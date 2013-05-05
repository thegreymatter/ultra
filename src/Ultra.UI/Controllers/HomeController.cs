using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using MongoDB.Bson;
using Ultra.Config.Routes;
using Ultra.Dal.Repositories;
using Ultra.Models;
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
			var process = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = "Ultra.Util.exe",
					Arguments = string.Format("--filename {0} --domain {1} --duration {2} -- rampup {3}",
					                          filename, settings.Domain, settings.Duration, settings.RampUp)
				}
			};

			process.Start();

			return Content("OK");
		}
	}
}

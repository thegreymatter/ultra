using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;
using Ultra.Models;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILoadRunRepository _loadRunRepository;
		private readonly IJMeterOutputAnalyzer _jMeterOutputAnalyzer;
		private readonly IJmxRunner _jmxRunner;

		public HomeController(ILoadRunRepository loadRunRepository, IJMeterOutputAnalyzer jMeterOutputAnalyzer, IJmxRunner jmxRunner)
		{
			_loadRunRepository = loadRunRepository;
			_jMeterOutputAnalyzer = jMeterOutputAnalyzer;
			_jmxRunner = jmxRunner;
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
			var a = new SecureString();
			foreach (var ch in "MF12345") // TODO: put this in configuration
				a.AppendChar(ch);

			var processInfo = new ProcessStartInfo {
				FileName = Path.Combine(Server.MapPath("~"), "Ultra.Util.exe"),
				Arguments = string.Format("--filename {0} --domain {1} --duration {2} -- rampup {3}",
				                          filename, settings.Domain, settings.Duration, settings.RampUp),
				UseShellExecute = false,
				UserName = "ultra",
				Password = a,
				LoadUserProfile = true,
				CreateNoWindow = false,
				WindowStyle = ProcessWindowStyle.Normal
			};

			Process.Start(processInfo);

			return Json("OK");
		}

		[Route("-/delete-run")]
		public ActionResult DeleteRun(string loadRunId)
		{
			// TODO: remove the files as well
			var runId = ObjectId.Parse(loadRunId);
			_loadRunRepository.DeleteLoadRun(runId);

			return Json("OK");
		}

		[Route("analyze")]
		public ActionResult AnalyzeOutputForm()
		{
			var outputDirectory = JMeterOutputAnalyzer.JMeterOutputArchive;
			var outputFiles = Directory.GetFiles(Path.Combine(Server.MapPath("/"), outputDirectory), "*.csv", SearchOption.TopDirectoryOnly);
			
			return View(outputFiles);
		}

		[Route("-/analyze-output")]
		public ActionResult AnalyzeOutput(string output_file, string output_domain, int output_duration, int output_rampup)
		{
			var outputFile = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, output_file);
			var jmxSettings = new JmxSettings {
				Domain = output_domain,
				Duration = output_duration,
				RampUp = output_rampup
			};

			var outputResults = _jMeterOutputAnalyzer.Analyze(outputFile, jmxSettings);
			_jmxRunner.PersistRunResults(outputResults, onlyAnalysis:true);

			return Content("OK");
		}
	}
}

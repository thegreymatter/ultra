﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Web;
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
			return View();
		}

		[Route("-/analyze-output")]
		public ActionResult AnalyzeOutput(HttpPostedFileBase output_file, string output_domain, string output_duration, string output_rampup)
		{
			return Content("OK");
		}
	}
}

using System.IO;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class AnalysisController : Controller
	{
		private readonly IJMeterOutputAnalyzer _jMeterOutputAnalyzer;
		private readonly IJmxRunner _jmxRunner;

		public AnalysisController(IJMeterOutputAnalyzer jMeterOutputAnalyzer, IJmxRunner jmxRunner)
		{
			_jMeterOutputAnalyzer = jMeterOutputAnalyzer;
			_jmxRunner = jmxRunner;
		}

		[Route("show-analysis/{jmeterOutputFile}")]
		public ActionResult AnalyzeResults(string jmeterOutputFile)
		{
			var filepath = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, jmeterOutputFile);
			var results = new JMeterOutputAnalyzer().Analyze(filepath, new JmxSettings {
				Domain = "uat.shopyourway.com",
				Duration = 1800,
				RampUp = 150
			});

			return View(results);
		}

		[Route("analyze")]
		public ActionResult AnalyzeOutput()
		{
			var outputDirectory = JMeterOutputAnalyzer.JMeterOutputArchive;
			var outputFiles = Directory.GetFiles(Path.Combine(Server.MapPath("/"), outputDirectory), "*.csv", SearchOption.TopDirectoryOnly);

			return View(outputFiles);
		}

		[Route("-/analyze-output")]
		public ActionResult AnalyzeOutput(string output_file, string output_domain, int output_duration, int output_rampup)
		{
			var outputFile = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, output_file);
			var jmxSettings = new JmxSettings
			{
				Domain = output_domain,
				Duration = output_duration,
				RampUp = output_rampup
			};

			var outputResults = _jMeterOutputAnalyzer.Analyze(outputFile, jmxSettings);
			_jmxRunner.PersistRunResults(outputResults, onlyAnalysis: true);

			return Content("OK");
		}
	}
}
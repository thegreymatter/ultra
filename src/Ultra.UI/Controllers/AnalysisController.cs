using System.IO;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class AnalysisController : Controller
	{
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
	}
}
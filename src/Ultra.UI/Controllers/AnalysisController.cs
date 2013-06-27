using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Castle.Components.DictionaryAdapter;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;
using Ultra.Filters;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;

namespace Ultra.Controllers
{
	public class AnalysisController : Controller
	{
		private readonly IJMeterOutputAnalyzer _jMeterOutputAnalyzer;
		private readonly IJmxRunner _jmxRunner;
		private readonly ILoadRunRepository _loadRunRepository;

		public AnalysisController(IJMeterOutputAnalyzer jMeterOutputAnalyzer, IJmxRunner jmxRunner,ILoadRunRepository loadRunRepository)
		{
			_loadRunRepository = loadRunRepository;
			_jMeterOutputAnalyzer = jMeterOutputAnalyzer;
			_jmxRunner = jmxRunner;
		}

		[Route("show-analysis/{jmeterOutputFile}")]
		public ActionResult AnalyzeResults(string jmeterOutputFile)
		{
			

			var filepath = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, jmeterOutputFile);
			using (var reader = new StreamReader(filepath))
			{
				
				var results = _jMeterOutputAnalyzer.Analyze( filepath, new JmxSettings
					                                                           {
						                                                           Domain = "uat.shopyourway.com",
						                                                           Duration = 1800,
						                                                           RampUp = 150
					                                                           });
				return View(results);
			}
			

		}

		[Route("distribution-graph/{jmeterOutputFile}")]
		public ActionResult DistributionGraph(string jmeterOutputFile)
		{
			var distributions = new ArrayList();

			var filepath = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, jmeterOutputFile);
			using (var reader = new StreamReader(filepath))
			{

				var results = _jMeterOutputAnalyzer.Analyze(filepath, new JmxSettings
					                                                      {
						                                                      Domain = "uat.shopyourway.com",
						                                                      Duration = 1800,
						                                                      RampUp = 150
					                                                      });


				

				foreach (var t in results.Threads)
				{
					distributions.Add(new ArrayList {t.GetThreadPoolName(), t.GetRequestCount()});
				}
				
			}
			return Json(distributions);
		}






		[Route("analyze")]
		public ActionResult Analyze()
		{
			var outputDirectory = JMeterOutputAnalyzer.JMeterOutputArchive;
			var outputFiles = Directory.GetFiles(Path.Combine(Server.MapPath("/"), outputDirectory), "*.csv", SearchOption.TopDirectoryOnly);

			return View(outputFiles);
		}

		[ExceptionHandlingFilterAttribute]
		[HttpPost]
		[Route("-/analyze-output")]
		public ActionResult AnalyzeOutput( HttpPostedFileBase output_file_upload, string output_domain, int output_duration, int output_rampup)
		{
			var filepath = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, output_file_upload.FileName);
			output_file_upload.SaveAs(filepath);

			
				var jmxSettings = new JmxSettings
					                  {
						                  Domain = output_domain,
						                  Duration = output_duration,
						                  RampUp = output_rampup
					                  };

				var outputResults = _jMeterOutputAnalyzer.Analyze(filepath, jmxSettings);
				_jmxRunner.PersistRunResults(outputResults, onlyAnalysis: true);

				return RedirectToRoute("");
			
		}
	}
}
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using Ultra.Config.ExtensionMethods;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Services.JMeterOutput;

namespace Ultra.Services.Jmx
{
	public interface IJmxRunner
	{
		void Run(string filename, JmxSettings settings, bool waitForFinish = true);
		void PersistRunResults(RunResults runResults, bool onlyAnalysis = false);
	}

	public class JmxRunner : IJmxRunner
	{
		private string _runTime;
		private ObjectId _loadRunId;
		private string _outputFile;

		public static string JmxFileArchive = ConfigurationManager.AppSettings["JmxFileArchive"];
		public static string JMeterBatFile = ConfigurationManager.AppSettings["JMeterBatFile"];
		public static string OutputArchive = ConfigurationManager.AppSettings["OutputArchive"];

		private readonly IStorage<LoadRun> _storage;
		private readonly IJMeterOutputAnalyzer _jmeterOutputAnalyzer;

		public JmxRunner(IStorage<LoadRun> storage, IJMeterOutputAnalyzer jmeterOutputAnalyzer)
		{
			_storage = storage;
			_jmeterOutputAnalyzer = jmeterOutputAnalyzer;
		}

		public Action<object, EventArgs> LoadRunFinished { get; set; }

		public void Run(string filename, JmxSettings settings, bool waitForFinish = true)
		{
			_runTime = DateTime.Now.ToUnixTime().ToString();
			_loadRunId = ObjectId.GenerateNewId();
			_outputFile = Path.Combine(OutputArchive, _runTime + ".csv");

			var newJmxFilename = ArchiveJmxFile(filename);
			PersistRunSettings(newJmxFilename, settings);
			RunScript(newJmxFilename, settings, waitForFinish);

			var runResults = _jmeterOutputAnalyzer.Analyze(_outputFile, settings);
			PersistRunResults(runResults);
		}

		// TODO: remove the onlyAnalysis parameter, and find a nicer way to do this!
		public void PersistRunResults(RunResults runResults, bool onlyAnalysis = false)
		{
			var loadRun = !onlyAnalysis ? _storage.GetById(_loadRunId) : new LoadRun();

			loadRun.PageMetrics = runResults.Threads.Select(x => new PageMetric {
				AverageResponseTime = x.GetAvgResponseTime(),
				ErrorRate = x.GetErrorPercent(),
				IsAjax = x.IsAjax,
				PageName = x.GetThreadPoolName(),
				PercentOverThreshold = x.GetPercentAboveThreshold(),
				Percentile90 = x.GetPercentileX(90),
				RequestCount = x.GetRequestCount()
			}).ToArray();
			loadRun.TotalPvs = runResults.PVS;
			_storage.SaveOrUpdate(loadRun);
		}

		private void RunScript(string newJmxFilename, JmxSettings settings, bool waitForFinish)
		{
			var process = new Process();

			var arguments = string.Format("-Jhostname={0} -Joutputfile={1}.csv -Jrampup={2} -Jduration={3} -n -t {4}",
				settings.Domain, _runTime, settings.RampUp, settings.Duration, newJmxFilename);

			process.StartInfo = new ProcessStartInfo {
				FileName = JMeterBatFile,
				Arguments = arguments
			};

			process.EnableRaisingEvents = true;
			if (waitForFinish)
				process.Exited += SaveOutputScriptAndUpdateRun;

			process.Start();
			process.WaitForExit();
		}

		private void SaveOutputScriptAndUpdateRun(object sender, EventArgs eventArgs)
		{
			var outputFilename = _runTime + ".csv";
			File.Move(outputFilename, _outputFile);

			var loadRun = _storage.GetById(_loadRunId);
			loadRun.EndTime = DateTime.Now;
			loadRun.RunOutputFilename = outputFilename;
			_storage.SaveOrUpdate(loadRun);
		}

		private void PersistRunSettings(string filename, JmxSettings settings)
		{
			_storage.SaveOrUpdate(new LoadRun {
				Id = _loadRunId,
				StartTime = DateTime.Now,
				JmxFilename = filename,
				Domain = settings.Domain,
				Duration = settings.Duration,
				RampUp = settings.RampUp
			});
		}

		private string ArchiveJmxFile(string filename)
		{
			var newFilename = Path.Combine(JmxFileArchive, _runTime + ".jmx");
			File.Copy(filename, newFilename);
			return newFilename;
		}
	}

	public class JmxSettings
	{
		public string Domain { get; set; }
		public int Duration { get; set; }
		public int RampUp { get; set; }
	}
}
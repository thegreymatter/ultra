using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using Ultra.Config.ExtensionMethods;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Dal.Repositories;
using Ultra.Services.JMeterOutput;

namespace Ultra.Services.Jmx
{
	public interface IJmxRunner
	{
		void Run(ObjectId loadRunId);
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

		private readonly ILoadRunRepository _loadRunRepository;
		private readonly IStorage<LoadRun> _storage;
		private readonly IJMeterOutputAnalyzer _jmeterOutputAnalyzer;

		public JmxRunner(IStorage<LoadRun> storage, IJMeterOutputAnalyzer jmeterOutputAnalyzer, ILoadRunRepository loadRunRepository)
		{
			_storage = storage;
			_jmeterOutputAnalyzer = jmeterOutputAnalyzer;
			_loadRunRepository = loadRunRepository;
		}

		public Action<object, EventArgs> LoadRunFinished { get; set; }

		public void Run(ObjectId loadRunId)
		{
			var loadRun = _loadRunRepository.GetLoadRun(loadRunId);
			if (loadRun.Status != LoadRunStatus.Pending)
				return;

			loadRun.StartTime = DateTime.Now;
			loadRun.Status = LoadRunStatus.Running;
			_loadRunRepository.SaveOrUpdate(loadRun);

			_runTime = DateTime.Now.ToUnixTime().ToString();
			_outputFile = Path.Combine(OutputArchive, _runTime + ".csv");

			var newJmxFilename = ArchiveJmxFile(loadRun.JmxFilename);
			RunScript(newJmxFilename, loadRun);

			var settings = new JmxSettings {
				Domain = loadRun.Domain, Duration = loadRun.Duration, RampUp = loadRun.RampUp
			};

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
			loadRun.Duration = runResults.RunningTime;
			loadRun.StartTime = runResults.StartTime;
			loadRun.EndTime = runResults.EndTime;
			loadRun.Status = LoadRunStatus.Finished;
			loadRun.RunOutputFilename = Path.GetFileName(runResults.OutputFilename);

			_storage.SaveOrUpdate(loadRun);
		}

		private void RunScript(string newJmxFilename, LoadRun loadRun)
		{
			var settings = new JmxSettings {
				Domain = loadRun.Domain,
				Duration = loadRun.Duration,
				RampUp = loadRun.RampUp
			};

			var process = new Process();

			var arguments = string.Format("-Jhostname={0} -Joutputfile={1}.csv -Jrampup={2} -Jduration={3} -n -t {4}",
				settings.Domain, _runTime, settings.RampUp, settings.Duration, newJmxFilename);

			process.StartInfo = new ProcessStartInfo {
				FileName = JMeterBatFile,
				Arguments = arguments
			};

			process.EnableRaisingEvents = true;
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
			loadRun.Status = LoadRunStatus.Finished;
			_storage.SaveOrUpdate(loadRun);
		}

		private string ArchiveJmxFile(string filename)
		{
			try
			{
				var newFilename = Path.Combine(JmxFileArchive, _runTime + ".jmx");
				File.Copy(filename, newFilename);
				return newFilename;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
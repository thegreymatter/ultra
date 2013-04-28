using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using MongoDB.Bson;
using Ultra.Config.ExtensionMethods;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Services.Jmx
{
	public interface IJmxRunner
	{
		void Run(string filename, JmxSettings settings);
	}

	public class JmxRunner : IJmxRunner
	{
		private string _runTime;
		private ObjectId _loadRunId;

		public static string JmxFileArchive = ConfigurationManager.AppSettings["JmxFileArchive"];
		public static string JMeterBatFile = ConfigurationManager.AppSettings["JMeterBatFile"];

		private readonly IStorage<LoadRun> _storage;

		public JmxRunner(IStorage<LoadRun> storage)
		{
			_storage = storage;
		}

		public void Run(string filename, JmxSettings settings)
		{
			var newJmxFilename = ArchiveJmxFile(filename);
			PersistRunSettings(newJmxFilename, settings);
			RunScript(newJmxFilename, settings);
			//SaveScriptOutput();
		}

		private void RunScript(string newJmxFilename, JmxSettings settings)
		{
			var process = new Process();

			var arguments = string.Format("-Jhostname={0} -Joutputfile={1}.csv -Jrampup={2} -Jduration={3} -n -t {4}",
				settings.Domain, _runTime, settings.RampUp, settings.Duration, newJmxFilename);

			process.StartInfo = new ProcessStartInfo {
				FileName = JMeterBatFile,
				Arguments = arguments
			};

			process.Exited += LoadRunFinished;
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
		}

		private void LoadRunFinished(object sender, EventArgs eventArgs)
		{
			var loadRun = _storage.GetById(_loadRunId);
			loadRun.EndTime = DateTime.Now;
			_storage.SaveOrUpdate(loadRun);
		}

		private void PersistRunSettings(string filename, JmxSettings settings)
		{
			_loadRunId = ObjectId.GenerateNewId();
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
			_runTime = DateTime.Now.ToUnixTime().ToString();
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
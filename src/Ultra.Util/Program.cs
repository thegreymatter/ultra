using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Windsor;
using Ultra.Config;
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;
using Ultra.Util.ArgumentParsing;

namespace Ultra.Util
{
	public class Program
	{
		private static IWindsorContainer _container;
		private static string _filename;
		private static readonly Stopwatch Watch = new Stopwatch();
		private static string _directory;

		static void Main(string[] args)
		{

			Console.WriteLine("Starting to initialize utility...");
			Bootstrap();

			var runner = _container.Resolve<IJmxRunner>();
			var loadRunRepository = _container.Resolve<ILoadRunRepository>();
			var blackListRepository = _container.Resolve<IBlackListRepository>();
			var parser = new ArgumentParser();
			var parsedArguments = parser.ParseArguments(args);
			DisplayStartMessage(parsedArguments);

			// TODO: output parameters and run info

			_filename =   parsedArguments.KeyValues.ContainsKey("filename")?parsedArguments.KeyValues["filename"]:null;
			_directory = parsedArguments.KeyValues.ContainsKey("directory") ? parsedArguments.KeyValues["directory"] : null;

			var jmxSettings = new JmxSettings
								  {
									  Domain = parsedArguments.KeyValues["domain"],
									  Duration = int.Parse(parsedArguments.KeyValues["duration"]),
									  RampUp = int.Parse(parsedArguments.KeyValues["rampup"])

								  };

			if (parsedArguments.Flags.Contains("analyze"))
			{
				if (_filename != null)
				{
					using (var reader = new StreamReader(_filename))
					{

						var results = new JMeterOutputAnalyzer(blackListRepository).Analyze(_filename, jmxSettings);
						runner.PersistRunResults(results, true);
						
						DisplayEndMessage();

						if (parsedArguments.Flags.Contains("wait"))
							Console.ReadKey();

						return;
					}
				}
				if (_directory != null)
				{

					foreach (var csvFile in Directory.GetFiles(_directory,"*.csv"))
					{
						try
						{


						using (var reader = new StreamReader(csvFile))
						{
							if (loadRunRepository.GetLoadRunByOutPutFilename(Path.GetFileName(csvFile)) != null)
							{
								Console.WriteLine("Ignored {0}", csvFile);
								continue;
							}
							var results = new JMeterOutputAnalyzer(blackListRepository).Analyze(csvFile, jmxSettings);
							var destPath = Path.Combine(JMeterOutputAnalyzer.JMeterOutputArchive, Path.GetFileName(csvFile));
							runner.PersistRunResults(results, true);
							if (!File.Exists(destPath))
							{
								File.Copy(csvFile, destPath);
							}
							Console.WriteLine("Parsed {0}",csvFile);
						}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error {0}", csvFile);
							Console.WriteLine(ex.Message);
						}
					}
					DisplayEndMessage();

					if (parsedArguments.Flags.Contains("wait"))
						Console.ReadKey();
					return;
				}
			}

			// TODO: output run info...

			loadRunRepository.CreateLoadRun(new LoadRun
			{
				Servers = parsedArguments.KeyValues["servers"].Split(',').Select(x => x.Trim()).ToArray(),
				Domain = jmxSettings.Domain,
				Duration = jmxSettings.Duration,
				JmxFilename = _filename,
				RampUp = jmxSettings.RampUp,
				Status = LoadRunStatus.Pending
			});

			DisplayEndMessage();

			if (parsedArguments.Flags.Contains("wait"))
				Console.ReadKey();
		}

		private static void DisplayEndMessage()
		{
			Console.WriteLine("Finished running at : " + DateTime.Now);
			Watch.Stop();
			Console.WriteLine("Elapsed time : " + string.Format("{0:0.00}s", ((double)Watch.ElapsedMilliseconds / 1000)));
		}

		private static void DisplayStartMessage(UtilArguments arguments)
		{
			Console.WriteLine("Successfully parsed user input");
			Console.WriteLine("Arguments are : ");
			foreach (var arg in arguments.KeyValues)
				Console.WriteLine(arg.Key + " = " + arg.Value);
			Console.WriteLine("Flags are : ");
			foreach (var arg in arguments.Flags)
				Console.WriteLine(arg);

			Console.WriteLine("Started to run at : " + DateTime.Now);
			Watch.Start();
		}

		private static void Bootstrap()
		{
			var bootstrapper = new Bootstrapper(new WindsorContainer());
			bootstrapper.RegisterServices(MvcApplication.Assemblies);
			_container = bootstrapper._container;
		}
	}
}

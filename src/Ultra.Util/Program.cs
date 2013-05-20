using System;
using System.Diagnostics;
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

		static void Main(string[] args)
		{
			Console.WriteLine("Starting to initialize utility...");
			Bootstrap();

			var runner = _container.Resolve<IJmxRunner>();
			var loadRunRepository = _container.Resolve<ILoadRunRepository>();

			var parser = new ArgumentParser();
			var parsedArguments = parser.ParseArguments(args);
			DisplayStartMessage(parsedArguments);

			// TODO: output parameters and run info

			_filename = parsedArguments.KeyValues["filename"];
			var jmxSettings = new JmxSettings {
				Domain = parsedArguments.KeyValues["domain"],
				Duration = int.Parse(parsedArguments.KeyValues["duration"]),
				RampUp = int.Parse(parsedArguments.KeyValues["rampup"])
			};

			if (parsedArguments.Flags.Contains("analyze"))
			{
				var results = new JMeterOutputAnalyzer().Analyze(_filename, jmxSettings);
				runner.PersistRunResults(results, true);

				DisplayEndMessage();

				if (parsedArguments.Flags.Contains("wait"))
					Console.ReadKey();

				return;
			}

			// TODO: output run info...

			loadRunRepository.CreateLoadRun(new LoadRun {
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

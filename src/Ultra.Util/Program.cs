using System;
using Castle.Windsor;
using Ultra.Config;
using Ultra.Services.JMeterOutput;
using Ultra.Services.Jmx;
using Ultra.Util.ArgumentParsing;

namespace Ultra.Util
{
	public class Program
	{
		private static IWindsorContainer _container;
		private static string _filename;

		static void Main(string[] args)
		{
			Bootstrap();

			var runner = _container.Resolve<IJmxRunner>();

			var parser = new ArgumentParser();
			var parsedArguments = parser.ParseArguments(args);

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

				if (parsedArguments.Flags.Contains("wait"))
					Console.ReadKey();

				return;
			}

			// TODO: output run info...

			runner.Run(_filename, jmxSettings);

			if (parsedArguments.Flags.Contains("wait"))
				Console.ReadKey();
		}

		private static void Bootstrap()
		{
			var bootstrapper = new Bootstrapper(new WindsorContainer());
			bootstrapper.RegisterServices(MvcApplication.Assemblies);
			_container = bootstrapper._container;
		}
	}
}

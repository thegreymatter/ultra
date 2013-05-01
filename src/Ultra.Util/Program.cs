using Castle.Windsor;
using Ultra.Config;
using Ultra.Services.Jmx;
using Ultra.Util.ArgumentParsing;

namespace Ultra.Util
{
	public class Program
	{
		private static IWindsorContainer _container;
		private static string _jmxFilename;

		static void Main(string[] args)
		{
			Bootstrap();

			var runner = _container.Resolve<IJmxRunner>();

			var parser = new ArgumentParser();
			var parsedArguments = parser.ParseArguments(args);

			_jmxFilename = parsedArguments.KeyValues["filename"];
			var jmxSettings = new JmxSettings {
				Domain = parsedArguments.KeyValues["domain"],
				Duration = int.Parse(parsedArguments.KeyValues["duration"]),
				RampUp = int.Parse(parsedArguments.KeyValues["rampup"])
			};

			runner.Run(_jmxFilename, jmxSettings);
		}

		private static void Bootstrap()
		{
			var bootstrapper = new Bootstrapper(new WindsorContainer());
			bootstrapper.RegisterServices(MvcApplication.Assemblies);
			_container = bootstrapper._container;
		}
	}
}

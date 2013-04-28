using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Ultra.Config;
using Ultra.Services.Jmx;

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

			var settings = ParseArguments(args);
			runner.Run(_jmxFilename, settings);
		}

		private static JmxSettings ParseArguments(string[] args)
		{
			var settings = new JmxSettings();
			for (var i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "--filename":
						_jmxFilename = args[i + 1];
						break;
					case "--duration":
						settings.Duration = int.Parse(args[i + 1]);
						break;
					case "--rampup":
						settings.RampUp = int.Parse(args[i + 1]);
						break;
					case "--domain":
						settings.Domain = args[i + 1];
						break;
				}
			}
			return settings;
		}

		private static void Bootstrap()
		{
			var bootstrapper = new Bootstrapper(new WindsorContainer());
			bootstrapper.RegisterServices(MvcApplication.Assemblies);
			_container = bootstrapper._container;
		}
	}
}

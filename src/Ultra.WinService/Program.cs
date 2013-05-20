using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace Ultra.WinService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if (Environment.UserInteractive)
			{
				var assemblyLocation = Assembly.GetExecutingAssembly().Location;
				if (args.Contains("--install"))
				{
					ManagedInstallerClass.InstallHelper(new[] {assemblyLocation});
				}
				else if (args.Contains("--uninstall"))
				{
					ManagedInstallerClass.InstallHelper(new[] {"/u", assemblyLocation});
				}
			}
			else
			{
				var servicesToRun = new ServiceBase[] {
					new UltraWinService()
				};
				ServiceBase.Run(servicesToRun);
			}
		}
	}
}

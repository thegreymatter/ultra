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
		/// Run this exe file from the command line with '--install' to install as a windows service
		/// Run with '--uninstall' to remove
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

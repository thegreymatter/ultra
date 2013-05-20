using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Castle.Windsor;
using Ultra.Config;
using Ultra.Dal.Repositories;

namespace Ultra.WinService
{
	public partial class UltraWinService : ServiceBase
	{
		private Timer _timer;
		private Bootstrapper _bootstrapper;
		private IWindsorContainer _container;

		private ILoadRunRepository _loadRunRepository;

		public UltraWinService()
		{
			InitializeComponent();

			Debugger.Launch();

			InitializeContainer();

			_loadRunRepository = _container.Resolve<ILoadRunRepository>();

			_timer = new Timer {
				Enabled = true,
				Interval = 60000
			};
			_timer.Elapsed += CheckLoadRunsStatus;
		}

		private void InitializeContainer()
		{
			var assemblies = new[] {
				Assembly.GetExecutingAssembly(),
				typeof (Dal.Entities.EntityBase).Assembly,
				typeof (Services.Jmx.JmxRunner).Assembly
			};

			_container = new WindsorContainer();
			_bootstrapper = new Bootstrapper(_container);
			_bootstrapper.RegisterServices(assemblies);
		}

		private void CheckLoadRunsStatus(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			_timer.Stop();
			try
			{
				var pendingLoadRuns = _loadRunRepository.GetPendingLoadRuns();
				if (pendingLoadRuns.Any())
				{
					//
				}
			}
			finally
			{
				_timer.Start();
			}
		}

		protected override void OnStart(string[] args)
		{
			_timer.Start();
		}

		protected override void OnStop()
		{
		}
	}
}

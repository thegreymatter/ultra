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
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;
using Ultra.Services.Jmx;

namespace Ultra.WinService
{
	public partial class UltraWinService : ServiceBase
	{
		// TODO: add logging to this service!

		private Timer _timer;
		private Bootstrapper _bootstrapper;
		private IWindsorContainer _container;

		private ILoadRunRepository _loadRunRepository;
		private IJmxRunner _jmxRunner;

		public UltraWinService()
		{
			InitializeComponent();

			InitializeContainer();

			_loadRunRepository = _container.Resolve<ILoadRunRepository>();
			_jmxRunner = _container.Resolve<IJmxRunner>();

			_loadRunRepository.CreateLoadRun(new LoadRun
			{
				Domain = "uat.shopyourway.com",
				Duration = 1800,
				JmxFilename = "C:\\utils\\apache-jmeter-2.9\\jmx_archive\\1367244146.46075.jmx",
				RampUp = 120,
				Status = LoadRunStatus.Pending
			});

			_timer = new Timer {
				Enabled = true,
				Interval = 60000
			};

#if DEBUG
			_timer.Interval = 5000;
			Debugger.Launch();
#endif

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
					// TODO: currently knows how to handle 1 load run at a time
					var loadRun = pendingLoadRuns.Last();
					_jmxRunner.Run(loadRun.Id);
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
			// TODO: check if there's a running load test to stop as well
			_timer.Stop();
		}
	}
}

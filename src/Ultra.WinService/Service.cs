using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using Castle.Windsor;
using Ultra.Config;
using Ultra.Dal.Repositories;
using Ultra.Services.Jmx;

namespace Ultra.WinService
{
	public partial class UltraWinService : ServiceBase
	{
		// TODO: add logging to this service!

		private Timer _timer;

		private ILoadRunRepository _loadRunRepository;
		private IJmxRunner _jmxRunner;

		public UltraWinService()
		{
			InitializeComponent();

			InitializeContainer();
			
			InitializeServiceTimer();
		}

		private void InitializeServiceTimer()
		{
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

			var container = new WindsorContainer();
			var bootstrapper = new Bootstrapper(container);
			bootstrapper.RegisterServices(assemblies);

			RegisterServices(container);
		}

		private void RegisterServices(IWindsorContainer container)
		{
			_loadRunRepository = container.Resolve<ILoadRunRepository>();
			_jmxRunner = container.Resolve<IJmxRunner>();
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

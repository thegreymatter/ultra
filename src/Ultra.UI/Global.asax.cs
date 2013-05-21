using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NHibernate;
using Ultra.Config;
using Ultra.Config.Routes;
using Ultra.Controllers;
using Ultra.Controllers.Plumbing;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;
using Ultra.Services.Jmx;

namespace Ultra
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private IWindsorContainer _container;
		private Bootstrapper _bootstrapper;

		public static Assembly[] Assemblies = new[] {
			typeof (RunController).Assembly,	// Ultra.UI
			typeof (JmxRunner).Assembly,		// Ultra.Services
			typeof (EntityBase).Assembly		// Ultra.Dal
		};

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			RouteAttribute.MapDecoratedRoutes(routes);
		}

		protected void Application_Start()
		{
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			BootstrapContainer();
		}

		protected void Application_End()
		{
			if (_container != null)
				_container.Dispose();
		}

		private void BootstrapContainer()
		{
			_container = new WindsorContainer().Install(FromAssembly.This());
			var controllerFactory = new WindsorControllerFactory(_container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);

			// do registrations
			
			_bootstrapper = new Bootstrapper(_container);
			_bootstrapper.RegisterServices(Assemblies);

			// RegisterDbSessionFactory();
		}

		private void RegisterDbSessionFactory()
		{
			var config = DbPersistanceFactory.BuildDatabaseConfiguration();
			_container.Register(
					Component.For<ISessionFactory>()
							.UsingFactoryMethod(_ => config.BuildSessionFactory()),
					Component.For<ISession>()
							.UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession())
							.LifestylePerWebRequest()
					);
		}
	}
}
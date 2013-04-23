using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using Ultra.Config.Routes;
using Ultra.Controllers;
using Ultra.Controllers.Plumbing;
using Ultra.Dal.Plumbing;

namespace Ultra
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private IWindsorContainer _container;

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
			AreaRegistration.RegisterAllAreas();

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
			_container = new WindsorContainer();
			var controllerFactory = new WindsorControllerFactory(_container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);

			// do registrations
			
			// TODO: make registration of components convention based

			_container.Register(AllTypes.FromThisAssembly()
				.BasedOn<IController>()
				.If(Component.IsInSameNamespaceAs<HomeController>())
				.If(t => t.Name.EndsWith("Controller"))
				.Configure(t => t.LifestyleTransient()));

			_container.Register(
				Component.For(typeof (IStorage<>))
				.ImplementedBy(typeof (Storage<>))
				.LifestyleSingleton());

			_container.Register(
				Component.For<IMongoProvider>()
				.ImplementedBy<MongoProvider>()
				.LifestyleSingleton());

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
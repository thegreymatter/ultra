using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Ultra.Config
{
	public class Bootstrapper
	{
		public IWindsorContainer _container;

		public Bootstrapper(IWindsorContainer container)
		{
			_container = container;
		}

		public void RegisterServices(Assembly[] assemblies)
		{
			var allTypes = assemblies.SelectMany(x => x.GetTypes());
			foreach (var type in allTypes.Where(x => x.IsInterface))
			{
				_container.Register(
					Component.For(type)
					.ImplementedBy(allTypes.Single(x => !x.IsInterface && type.Name == "I" + x.Name))
					.LifestyleSingleton());
			}
		} 
	}
}
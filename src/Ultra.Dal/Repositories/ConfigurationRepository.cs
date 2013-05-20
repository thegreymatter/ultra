using System.Linq;
using Ultra.Dal.Entities;
using Ultra.Dal.Plumbing;

namespace Ultra.Dal.Repositories
{
	public interface IConfigurationRepository
	{
		UltraConfiguration GetConfiguration();
		void SaveConfiguration(UltraConfiguration config);
	}

	public class ConfigurationRepository : IConfigurationRepository
	{
		// TODO: add some kind of caching level here...

		private readonly IStorage<UltraConfiguration> _configStorage;

		public ConfigurationRepository(IStorage<UltraConfiguration> configStorage)
		{
			_configStorage = configStorage;
		}

		public UltraConfiguration GetConfiguration()
		{
			return _configStorage.GetAll().First();
		}

		public void SaveConfiguration(UltraConfiguration config)
		{
			var currentConfig = _configStorage.GetAll().First();
			config.Id = currentConfig.Id;
			_configStorage.SaveOrUpdate(config);
		}
	}
}
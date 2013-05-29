using System.Linq;
using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Entities;
using Ultra.Dal.Repositories;

namespace Ultra.Controllers
{
	public class ConfigurationController : Controller
	{
		private readonly IConfigurationRepository _configurationRepository;

		public ConfigurationController(IConfigurationRepository configurationRepository)
		{
			_configurationRepository = configurationRepository;
		}

		[Route("edit-configuration")]
		public ActionResult EditConfiguration()
		{
			var config = _configurationRepository.GetConfiguration() ?? new UltraConfiguration();

			return View(config);
		}

		[Route("-/save-configuration")]
		public ActionResult SaveConfiguration(UltraConfiguration configuration)
		{
			configuration.Servers = configuration.Servers[0].Split(',').Select(x => x.Trim()).ToArray();
			_configurationRepository.SaveConfiguration(configuration);

			return Json("OK");
		}
	}
}
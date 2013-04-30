using System.Web.Mvc;
using Ultra.Config.Routes;
using Ultra.Dal.Repositories;

namespace Ultra.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILoadRunRepository _loadRunRepository;

		public HomeController(ILoadRunRepository loadRunRepository)
		{
			_loadRunRepository = loadRunRepository;
		}

		[Route("")]
		public ActionResult Index()
		{
			var loadRuns = _loadRunRepository.GetMostRecentLoadRuns(10);

			return View(loadRuns);
		}
	}
}

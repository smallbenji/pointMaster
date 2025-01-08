using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pointMaster.Controllers
{
    public class StatsController : Controller
    {
        public StatsController() { }

        [Authorize(Policy = Roles.Editor)]
        public IActionResult Index()
        {
            return View();
        }
    }
}

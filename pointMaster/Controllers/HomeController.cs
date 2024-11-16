using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pointMaster.Models;
using System.Diagnostics;

namespace pointMaster.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new HomePageViewModel();

            vm.loggedIn = User.Identity.IsAuthenticated;

            return View(vm);
        }

        [Authorize(Policy = Roles.Admin)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    public class HomePageViewModel
    {
        public bool loggedIn { get; set; } = false;
    }
}

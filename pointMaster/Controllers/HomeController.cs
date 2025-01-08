using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pointMaster.Data;
using pointMaster.Models;
using System.Diagnostics;

namespace pointMaster.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomePageViewModel();

            var patruljer = await context.Patruljer.Include(p => p.Points).ToListAsync();

            vm.Samlet = new List<PatruljePlacering>();
            vm.Turnout = new List<PatruljePlacering>();
            vm.Points = new List<PatruljePlacering>();

            foreach (var patrulje in patruljer)
            {
                vm.Points.Add(new PatruljePlacering
                {
                    Patrulje = patrulje,
                    point = patrulje.Points.Sum(x => x.Points),
                });

                vm.Turnout.Add(new PatruljePlacering
                {
                    Patrulje = patrulje,
                    point = patrulje.Points.Sum(x => x.Turnout),
                });

                vm.Samlet.Add(new PatruljePlacering
                {
                    Patrulje = patrulje,
                    point = patrulje.Points.Sum(x => x.Points) + patrulje.Points.Sum(x => x.Turnout),
                });
            }

            vm.Samlet = vm.Samlet.OrderByDescending(x => x.point).ToList();
            vm.Points = vm.Points.OrderByDescending(x => x.point).ToList();
            vm.Turnout = vm.Turnout.OrderByDescending(x => x.point).ToList();


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
        public List<PatruljePlacering> Samlet { get; set; } = null!;
        public List<PatruljePlacering> Turnout { get; set; } = null!;
        public List<PatruljePlacering> Points { get; set; } = null!;
    }
    public class PatruljePlacering
    {
        public Patrulje Patrulje { get; set; } = null!;
        public int point { get; set; }
    }
}

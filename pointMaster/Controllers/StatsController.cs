using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pointMaster.Data;
using pointMaster.Models;

namespace pointMaster.Controllers
{
    public class StatsController : Controller
    {
        private readonly DataContext dataContext;

        public StatsController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pointMaster.Data;
using pointMaster.Models;

namespace pointMaster.Controllers
{
    [Authorize(Policy = Roles.Editor)]
    public class PosterController : Controller
    {
        private readonly DataContext _context;

        public PosterController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new RundeViewModel();
            vm.Rounds = await _context.Poster.ToListAsync();

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Location, Description")] Post runde)
        {
            if (ModelState.IsValid)
            {
                _context.Poster.Add(runde);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(runde);
        }

        [HttpGet]
        public async Task<IActionResult> SletPost(int id)
        {
            var post = _context.Poster.Find(id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Poster.Remove(post);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public class RundeViewModel
        {
            public List<Post> Rounds { get; set; }
        }
    }
}

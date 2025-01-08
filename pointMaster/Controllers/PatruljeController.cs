using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pointMaster.Data;
using pointMaster.Models;

namespace pointMaster.Controllers
{
    [Authorize(Policy = Roles.Editor)]
    public class PatruljeController : Controller
    {
        private readonly DataContext _context;

        public PatruljeController(
            DataContext context
            )
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel();

            vm.patruljePoints = new Dictionary<int, int>();
            vm.patruljeTurnout = new Dictionary<int, int>();

            vm.patruljeModels = await _context.Patruljer.Include(p => p.PatruljeMedlems).Include(p => p.Points).ToListAsync();

            foreach (var patrulje in vm.patruljeModels)
            {
                if (patrulje.Points == null)
                {
                    vm.patruljePoints.Add(patrulje.Id, 0);
                    vm.patruljeTurnout.Add(patrulje.Id, 0);

                    continue;
                }

                vm.patruljePoints.Add(patrulje.Id, patrulje.Points.Sum(x => x.Points));
                vm.patruljeTurnout.Add(patrulje.Id, patrulje.Points.Sum(x => x.Turnout));
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Patrulje patrulje)
        {
            patrulje.DateCreated = DateTime.UtcNow;

            await _context.Patruljer.AddAsync(patrulje);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddMedlem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrulje = await _context.Patruljer.FindAsync(id);
            if (patrulje == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMedlem(int id, [Bind("Name, Age")] PatruljeMedlem medlem)
        {
            try
            {
                var patrulje = await _context.Patruljer.FindAsync(id);

                medlem.Patrulje = patrulje;
                medlem.DateCreated = DateTime.UtcNow;

                _context.PatruljeMedlemmer.Add(medlem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Authorize(Policy = Roles.Editor)]
        public async Task<IActionResult> DeleteMedlem(int id)
        {
            var medlem = await _context.PatruljeMedlemmer.FindAsync(id);

            if (medlem != null)
            {
                _context.PatruljeMedlemmer.Remove(medlem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = Roles.Editor)]
        public async Task<IActionResult> DeletePatrulje(int id)
        {
            var patrulje = _context.Patruljer.Include(x => x.PatruljeMedlems).FirstOrDefault(x => x.Id == id);

            if (patrulje != null)
            {
                _context.Patruljer.Remove(patrulje);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public class IndexViewModel
        {
            public List<Patrulje> patruljeModels { get; set; } = null!;
            public Dictionary<int, int> patruljePoints { get; set; } = null!;
            public Dictionary<int, int> patruljeTurnout {  get; set; } = null!;
        }
    }
}

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
            vm.patruljeModels = await _context.Patruljer.Include(p => p.PatruljeMedlems).ToListAsync();

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Patrulje patrulje)
        {
            _context.Patruljer.Add(patrulje);
            _context.SaveChanges();

            return RedirectToAction("Index");
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
                _context.SaveChanges();
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
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public class IndexViewModel
        {
            public List<Patrulje> patruljeModels { get; set; }
        }
    }
}

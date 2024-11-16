using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pointMaster.Data;
using pointMaster.Models;

namespace pointMaster.Controllers
{
    [Authorize(Policy = Roles.Postmaster)]
    public class PointController : Controller
    {
        private readonly DataContext context;

        public PointController(DataContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SkiftPatrulje()
        {
            var vm = new SkiftPatruljeViewModel();

            vm.Patruljer = context.Patruljer.ToList();

            return View(vm);
        }
        
        public async Task<IActionResult> GivPoint(int id = 0)
        {
            if (id == 0)
            {
                if (string.IsNullOrEmpty(Request.Cookies["Post"]))
                {
                    return RedirectToAction(nameof(SelectPoster));
                }
                else
                {
                    return RedirectToAction(nameof(SkiftPatrulje));
                }
            }

            var vm = new GivPointViewModel();

            var patrulje = await context.Patruljer.FindAsync(id);

            if (patrulje != null) {
                vm.Patrulje = patrulje;
            }
            else
            {
                return NotFound();
            }

            var postId = Request.Cookies["Post"];

            if (postId == null)
            {
                return RedirectToAction(nameof(SelectPoster));
            }
            int.TryParse(postId, out var postIntId);

            var post = await context.Poster.FindAsync(postIntId);

            if (post != null)
            {
                vm.Runde = post;
            }
            else
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> GivPoint(int id, GivPointViewModel point)
        {
            var postId = Request.Cookies["Post"];

            int.TryParse(postId, out var postIntId);

            var p = new Point();

            var patrulje = await context.Patruljer.Include(x => x.PatruljeMedlems).FirstOrDefaultAsync(x => x.Id == id);
            var post = await context.Poster.FindAsync(postIntId);

            if (patrulje == null || post == null)
            {
                return NotFound();
            }

            p.Patrulje = patrulje;
            p.Poster = post;
            p.Turnout = point.points.Turnout;
            p.Points = point.points.Points;

            context.Points.Add(p);

            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SelectPoster()
        {
            var vm = new SelectPostViewModel();

            vm.Poster = await context.Poster.ToListAsync();

            return View(vm);
        }

        public IActionResult SelectPost(int id)
        {
            var cookieOptions = new CookieOptions();

            cookieOptions.Expires = DateTime.Now.AddDays(7);
            cookieOptions.Path = "/";

            Response.Cookies.Append("Post", id.ToString(), cookieOptions);

            return RedirectToAction("index", "home");
        }
    }

    public class SelectPostViewModel
    {
        public List<Post> Poster { get; set; } = null!;
    }

    public class GivPointViewModel
    {
        public Post Runde { get; set; } = null!;
        public Patrulje Patrulje { get; set; } = null!;
        public Point points { get; set; } = new Point();
    }
    public class SkiftPatruljeViewModel
    {
        public List<Patrulje> Patruljer { get; set; } = null!;
    }
}

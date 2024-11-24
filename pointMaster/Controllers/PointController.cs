﻿using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Index()
        {
            var vm = new PointViewModel();

            vm.points = await context.Points.Include(p => p.Patrulje).Include(p => p.Poster).ToListAsync();

            vm.AllowedToDelete = false;

            if (HttpContext.User.Claims.FirstOrDefault(x => x.Value == Roles.Editor) != null)
            {
                vm.AllowedToDelete = true;
            }

            return View(vm);
        }

        public async Task<IActionResult> SkiftPatrulje()
        {
            var vm = new SkiftPatruljeViewModel();

            if (string.IsNullOrEmpty(Request.Cookies["Post"]))
            {
                return RedirectToAction(nameof(SkiftPost));
            }

            int.TryParse(Request.Cookies["Post"], out var postId);

            vm.post = await context.Poster.FindAsync(postId);

            vm.Patruljer = context.Patruljer.ToList();

            return View(vm);
        }
        
        public async Task<IActionResult> GivPoint(int id = 0)
        {
            if (id == 0)
            {
                if (string.IsNullOrEmpty(Request.Cookies["Post"]))
                {
                    return RedirectToAction(nameof(SkiftPost));
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
                return RedirectToAction(nameof(SkiftPost));
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

            p.DateCreated = DateTime.UtcNow;

            context.Points.Add(p);

            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SkiftPost()
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

            return Redirect("/");
        }

        public ActionResult DeletePoint(int id)
        {
            var point = context.Points.FirstOrDefault(p => p.Id == id);
            if (point == null) { return NotFound(); }

            context.Points.Remove(point);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
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
        public Post? post { get; set; } = null!;
    }
    public class PointViewModel
    {
        public List<Point> points { get; set; } = null!;
        public bool AllowedToDelete { get; set; } = false;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pointMaster.Controllers
{
    public class HeaderNav : ViewComponent
    {
        private readonly IAuthorizationService authorizationService;

        public HeaderNav(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public IViewComponentResult Invoke()
        {
            var vm = new HeaderNavViewModel();

            vm.links = new List<NavUrl>();

            if (HttpContext.User.Claims.FirstOrDefault(x => x.Value == Roles.Editor) != null)
            {
                vm.links.Add(new NavUrl("Patruljer", "/Patrulje"));
                vm.links.Add(new NavUrl("Point", "/Point"));
                vm.links.Add(new NavUrl("Poster", "/Poster"));
                vm.links.Add(new NavUrl("Statistikker", "/Stats"));
            }
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                vm.links.Add(new NavUrl("Giv point", "/Point/GivPoint"));
                vm.links.Add(new NavUrl("Vælg post", "/Point/SkiftPost"));
                vm.links.Add(new NavUrl("Log ud", "/account/signout"));
            }
            else
            {
                vm.links.Add(new NavUrl("Log ind", "/account/signin"));
            }

            return View(vm);
        }
    }

    public class HeaderNavViewModel
    {
        public List<NavUrl> links { get; set; }
    }
    public class NavUrl
    {
        public NavUrl() { }
        public NavUrl(string title, string url)
        {
            this.Title = title;
            this.Url = url;
        }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pointMaster.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult SignIn(string redirectUrl = "/")
        {
            if (!this.User.Identity!.IsAuthenticated)
            {
                return this.Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }

            return Redirect(redirectUrl);
        }

        [AllowAnonymous]
        public async Task<IActionResult> SignOutAsync()
        {
            if (!this.User.Identity!.IsAuthenticated)
            {
                return this.Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }

            var idToken = await this.HttpContext.GetTokenAsync("id_token");

            var authResult = this
                .HttpContext.Features.Get<IAuthenticateResultFeature>()
                ?.AuthenticateResult;

            var tokens = authResult!.Properties!.GetTokens();
            var tokenNames = tokens.Select(token => token.Name).ToArray();


            return this.SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "/",
                    Items = { { "id_token_hint", idToken } }
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            );
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => this.RedirectToAction("AccessDenied", "Home");


    }
    public static class Roles
    {
        public const string Admin = "pointmaster-admin";
        public const string Editor = "pointmaster-editor";
        public const string Postmaster = "pointmaster-postmaster";
    }
}
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace pointMaster.Components
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;
            var groupClaims = identity.FindAll("groups").ToList();

            foreach (var claim in groupClaims)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));
            }

            return Task.FromResult(principal);
        }
    }
}

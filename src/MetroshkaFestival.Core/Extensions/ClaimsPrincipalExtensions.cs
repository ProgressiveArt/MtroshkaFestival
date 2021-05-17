using System.Linq;
using System.Security.Claims;
using static System.Int32;

namespace MetroshkaFestival.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdStr = user?.FindFirst(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            TryParse(userIdStr, out var userId);
            return userId;
        }

        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
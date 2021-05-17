using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MetroshkaFestival.Web.HtmlHelpers
{
    public static class UserHelper
    {
        public static string GetLogin(this IHtmlHelper html, ClaimsPrincipal user)
        {
            var login = user?.FindFirst(x => x.Type.Equals(ClaimTypes.Name))?.Value ?? "Anonim";
            return login;
        }

        public static string GetRole(this IHtmlHelper html, ClaimsPrincipal user)
        {
            var role = user?.FindFirst(x => x.Type.Equals(ClaimTypes.Role))?.Value ?? "Anonim";
            return role;
        }
    }
}
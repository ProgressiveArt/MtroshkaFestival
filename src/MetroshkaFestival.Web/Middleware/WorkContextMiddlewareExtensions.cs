using System;
using System.Linq;
using System.Security.Claims;
using MetroshkaFestival.Core.Extensions;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Core.WorkContext;
using Microsoft.AspNetCore.Builder;

namespace MetroshkaFestival.Web.Middleware
{
    public static class WorkContextMiddlewareExtensions
    {
        public static void UseWorkContext(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
                    {
                        var contextAccessor = (WorkContextAccessor) context.RequestServices.GetService(typeof(WorkContextAccessor));
                        contextAccessor.CurrentContext = new WorkContext(
                            context.User.GetUserId(),
                            GetRoles(context.User));
                        await next();
                    });
        }

        private static ApplicationRole[] GetRoles(ClaimsPrincipal user)
        {
            return user?.FindAll(x => x.Type.Equals(ClaimTypes.Role))
                       ?.Select(x => x.Value)
                        .Where(x => Enum.IsDefined(typeof(ApplicationRole), x))
                        .Select(x => Enum.TryParse<ApplicationRole>(x, ignoreCase: true, out var role) ? role : default)
                        .ToArray()
                   ?? Array.Empty<ApplicationRole>();
        }
    }
}
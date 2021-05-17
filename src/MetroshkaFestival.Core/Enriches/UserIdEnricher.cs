using MetroshkaFestival.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace MetroshkaFestival.Core.Enriches
{
    public class UserIdEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserIdEnricher() : this(new HttpContextAccessor()) { }

        public UserIdEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
            {
                return;
            }

            var property = new LogEventProperty("UserId", new ScalarValue(GetUserId()));
            logEvent.AddOrUpdateProperty(property);
        }

        private string GetUserId()
        {
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            return userId == 0 ? "" : $"{userId}";
        }
    }
}
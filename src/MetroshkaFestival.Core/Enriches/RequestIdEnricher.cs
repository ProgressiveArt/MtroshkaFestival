using System;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace MetroshkaFestival.Core.Enriches
{
    public class RequestIdEnricher : ILogEventEnricher
    {
        private const string RequestIdItemName = nameof(RequestIdEnricher) + "+RequestId";
        private const string CounterRequestIdItemName = nameof(RequestIdEnricher) + "+CounterRequestId";
        private readonly IHttpContextAccessor _contextAccessor;

        public RequestIdEnricher() : this(new HttpContextAccessor()) { }

        public RequestIdEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
            {
                return;
            }

            var property = new LogEventProperty("RequestId", new ScalarValue(GetRequestId()));
            logEvent.AddOrUpdateProperty(property);
        }

        private string GetRequestId()
        {
            var obj1 = _contextAccessor.HttpContext.Items[RequestIdItemName];

            if (obj1 == null)
            {
                _contextAccessor.HttpContext.Items[RequestIdItemName] = Guid.NewGuid().ToString();
                _contextAccessor.HttpContext.Items[CounterRequestIdItemName] = 0;
            }

            var requestId = (string) _contextAccessor.HttpContext.Items[RequestIdItemName];
            var counterRequestId = (int) _contextAccessor.HttpContext.Items[CounterRequestIdItemName];

            _contextAccessor.HttpContext.Items[CounterRequestIdItemName] = ++counterRequestId;
            return $"{requestId} {counterRequestId}";
        }
    }
}
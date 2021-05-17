using System;
using MetroshkaFestival.Core.Enriches;
using Serilog;
using Serilog.Configuration;

namespace MetroshkaFestival.Core.Extensions
{
    public static class EnrichExtensions
    {
        public static LoggerConfiguration WithRequestId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With<RequestIdEnricher>()
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }

        public static LoggerConfiguration WithUserId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With<UserIdEnricher>()
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }
    }
}
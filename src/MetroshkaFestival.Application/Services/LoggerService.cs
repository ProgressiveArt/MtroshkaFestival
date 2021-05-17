using Interfaces.Core;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace MetroshkaFestival.Application.Services
{
    public class LoggerService : IService
    {
        public void Information(string message, HttpContext context)
        {
            Log.Information($"{message}. " +
                            $"request made with IP:[{context.Connection.RemoteIpAddress}] " +
                            $"and UserAgent: [{context.Request.Headers["User-Agent"].ToString()}]");
        }
    }
}
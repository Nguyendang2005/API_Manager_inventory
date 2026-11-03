using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Middlewares
{
    /// <summary>
    /// Middleware gán mã X-Correlation-ID cho mỗi request để hỗ trợ distributed tracing.
    /// </summary>
    public class RequestCorrelationMiddleware
    {
        public const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public RequestCorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) ||
                string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Items[CorrelationIdHeader] = correlationId.ToString();

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                {
                    context.Response.Headers.Append(CorrelationIdHeader, correlationId.ToString());
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}

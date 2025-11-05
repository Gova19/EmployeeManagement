using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Serilog.Context;

namespace EmployeeManagement.Api.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Create a unique correlation ID for each request
            var correlationId = Guid.NewGuid().ToString();
            context.Response.Headers.Append("X-Correlation-ID", correlationId);

            // Add correlation ID to log context
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    _logger.LogInformation("➡️ Handling {Method} {Path}", context.Request.Method, context.Request.Path);

                    await _next(context);

                    stopwatch.Stop();
                    _logger.LogInformation("✅ Completed {Method} {Path} with {StatusCode} in {Elapsed:0.000} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex,
                        "❌ Unhandled exception for {Method} {Path} in {Elapsed:0.000} ms",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.Elapsed.TotalMilliseconds);

                    await HandleExceptionAsync(context, ex);
                }
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later.",
                Detail = ex.Message,
                CorrelationId = context.Response.Headers["X-Correlation-ID"].ToString()
            };

            var json = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(json);
        }
    }
}

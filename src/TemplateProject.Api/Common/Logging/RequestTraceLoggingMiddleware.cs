using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace TemplateProject.Api.Common.Logging;

public class RequestTraceLoggingMiddleware
{
    private readonly ILogger<RequestTraceLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestTraceLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestTraceLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();
        context.Response.Headers["X-Trace-Id"] = traceId;

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("Method", context.Request.Method))
        using (LogContext.PushProperty("Path", context.Request.Path.Value ?? string.Empty))
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request completed with status code {StatusCode} in {ElapsedMilliseconds} ms.",
                    context.Response.StatusCode,
                    Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2));
            }
        }
    }
}

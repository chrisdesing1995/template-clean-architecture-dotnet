namespace TemplateProject.Api.Common.Logging;

public static class LoggingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRequestTraceLoggingContext(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.UseMiddleware<RequestTraceLoggingMiddleware>();
    }
}

using Serilog;

namespace TemplateProject.Api.Common.Logging;

public static class SerilogHostBuilderExtensions
{
    public static ConfigureHostBuilder UseConfiguredSerilog(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return hostBuilder;
    }
}

using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TemplateProject.Api.Swagger;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TemplateProject API",
                Version = "v1",
                Description = "Base enterprise para Clean Architecture + DDD Lite con response estándar global."
            });

            foreach (var xmlPath in GetXmlDocumentationPaths())
            {
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            }

            options.OperationFilter<SwaggerExamplesOperationFilter>();
        });

        return services;
    }

    private static IEnumerable<string> GetXmlDocumentationPaths()
    {
        var basePath = AppContext.BaseDirectory;
        var assemblies = new[]
        {
            Assembly.GetExecutingAssembly().GetName().Name,
            "TemplateProject.Application"
        };

        return assemblies
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => Path.Combine(basePath, $"{name}.xml"));
    }
}

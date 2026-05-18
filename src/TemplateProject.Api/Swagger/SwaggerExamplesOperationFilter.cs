using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TemplateProject.Api.Swagger;

public class SwaggerExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AddCommonErrorExamples(operation);
        AddPersonExamples(operation, context);
    }

    private static void AddCommonErrorExamples(OpenApiOperation operation)
    {
        AddResponseExample(operation, "400", BuildValidationErrorResponseExample());
        AddResponseExample(operation, "500", BuildInternalServerErrorResponseExample());
    }

    private static void AddPersonExamples(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];
        if (!string.Equals(controllerName, "Persons", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var actionName = context.ApiDescription.ActionDescriptor.RouteValues["action"];
        var httpMethod = context.ApiDescription.HttpMethod;

        if (string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            AddRequestExample(operation, BuildCreatePersonRequestExample());
            AddResponseExample(operation, "200", BuildSinglePersonSuccessResponseExample("Persona creada exitosamente."));
            return;
        }

        if (string.Equals(httpMethod, "PUT", StringComparison.OrdinalIgnoreCase))
        {
            AddRequestExample(operation, BuildUpdatePersonRequestExample());
            AddResponseExample(operation, "200", BuildSinglePersonSuccessResponseExample("Persona actualizada exitosamente."));
            return;
        }

        if (string.Equals(httpMethod, "DELETE", StringComparison.OrdinalIgnoreCase))
        {
            AddResponseExample(operation, "200", BuildDeleteSuccessResponseExample());
            return;
        }

        if (string.Equals(actionName, "GetById", StringComparison.OrdinalIgnoreCase))
        {
            AddResponseExample(operation, "200", BuildSinglePersonSuccessResponseExample("Operación exitosa."));
            return;
        }

        AddResponseExample(operation, "200", BuildPersonCollectionSuccessResponseExample());
    }

    private static void AddRequestExample(OpenApiOperation operation, IOpenApiAny example)
    {
        if (operation.RequestBody?.Content is null)
        {
            return;
        }

        if (operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
        {
            mediaType.Example = example;
        }
    }

    private static void AddResponseExample(OpenApiOperation operation, string statusCode, IOpenApiAny example)
    {
        if (!operation.Responses.TryGetValue(statusCode, out var response))
        {
            return;
        }

        if (response.Content.TryGetValue("application/json", out var mediaType))
        {
            mediaType.Example = example;
        }
    }

    private static OpenApiObject BuildCreatePersonRequestExample()
    {
        return new OpenApiObject
        {
            ["firstName"] = new OpenApiString("Grace"),
            ["lastName"] = new OpenApiString("Hopper"),
            ["email"] = new OpenApiString("grace.hopper@example.com")
        };
    }

    private static OpenApiObject BuildUpdatePersonRequestExample()
    {
        return new OpenApiObject
        {
            ["firstName"] = new OpenApiString("Grace"),
            ["lastName"] = new OpenApiString("Hopper"),
            ["email"] = new OpenApiString("grace.hopper@example.com"),
            ["isActive"] = new OpenApiBoolean(true)
        };
    }

    private static OpenApiObject BuildPersonCollectionSuccessResponseExample()
    {
        return new OpenApiObject
        {
            ["traceId"] = new OpenApiString("f4e674b3-64c4-4755-a01a-53f65ba65c0d"),
            ["success"] = new OpenApiBoolean(true),
            ["code"] = new OpenApiInteger(200),
            ["message"] = new OpenApiString("Operación exitosa."),
            ["data"] = new OpenApiArray
            {
                BuildPersonObject()
            },
            ["errors"] = new OpenApiNull()
        };
    }

    private static OpenApiObject BuildSinglePersonSuccessResponseExample(string message)
    {
        return new OpenApiObject
        {
            ["traceId"] = new OpenApiString("f4e674b3-64c4-4755-a01a-53f65ba65c0d"),
            ["success"] = new OpenApiBoolean(true),
            ["code"] = new OpenApiInteger(200),
            ["message"] = new OpenApiString(message),
            ["data"] = BuildPersonObject(),
            ["errors"] = new OpenApiNull()
        };
    }

    private static OpenApiObject BuildDeleteSuccessResponseExample()
    {
        return new OpenApiObject
        {
            ["traceId"] = new OpenApiString("f4e674b3-64c4-4755-a01a-53f65ba65c0d"),
            ["success"] = new OpenApiBoolean(true),
            ["code"] = new OpenApiInteger(200),
            ["message"] = new OpenApiString("Persona eliminada exitosamente."),
            ["data"] = new OpenApiNull(),
            ["errors"] = new OpenApiNull()
        };
    }

    private static OpenApiObject BuildValidationErrorResponseExample()
    {
        return new OpenApiObject
        {
            ["traceId"] = new OpenApiString("f4e674b3-64c4-4755-a01a-53f65ba65c0d"),
            ["success"] = new OpenApiBoolean(false),
            ["code"] = new OpenApiInteger(400),
            ["message"] = new OpenApiString("Error técnico."),
            ["data"] = new OpenApiNull(),
            ["errors"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["code"] = new OpenApiString("VALIDATION_ERROR"),
                    ["message"] = new OpenApiString("El correo es obligatorio.")
                }
            }
        };
    }

    private static OpenApiObject BuildInternalServerErrorResponseExample()
    {
        return new OpenApiObject
        {
            ["traceId"] = new OpenApiString("f4e674b3-64c4-4755-a01a-53f65ba65c0d"),
            ["success"] = new OpenApiBoolean(false),
            ["code"] = new OpenApiInteger(500),
            ["message"] = new OpenApiString("Error técnico."),
            ["data"] = new OpenApiNull(),
            ["errors"] = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["code"] = new OpenApiString("INTERNAL_SERVER_ERROR"),
                    ["message"] = new OpenApiString("Ocurrió un error interno del servidor.")
                }
            }
        };
    }

    private static OpenApiObject BuildPersonObject()
    {
        return new OpenApiObject
        {
            ["id"] = new OpenApiString("8b2fe2f4-4e2d-4524-83ce-aea4d6a5f4b7"),
            ["firstName"] = new OpenApiString("Ada"),
            ["lastName"] = new OpenApiString("Lovelace"),
            ["email"] = new OpenApiString("ada.lovelace@example.com"),
            ["isActive"] = new OpenApiBoolean(true),
            ["createdAt"] = new OpenApiString("2026-05-17T00:00:00Z")
        };
    }
}

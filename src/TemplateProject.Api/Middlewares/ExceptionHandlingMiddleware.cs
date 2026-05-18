using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using TemplateProject.Application.Common.Responses;

namespace TemplateProject.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var (statusCode, errors) = MapException(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>
        {
            TraceId = traceId,
            Success = false,
            Code = statusCode,
            Message = "Error técnico.",
            Data = null,
            Errors = errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonSerializerOptions));
    }

    private static (int StatusCode, List<ApiError> Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => ((int)HttpStatusCode.BadRequest, validationException.Errors
                .Select(error => new ApiError
                {
                    Code = string.IsNullOrWhiteSpace(error.ErrorCode) ? "VALIDATION_ERROR" : error.ErrorCode,
                    Message = error.ErrorMessage
                })
                .ToList()),
            ApplicationException applicationException => ((int)HttpStatusCode.BadRequest, new List<ApiError>
            {
                new()
                {
                    Code = "APPLICATION_ERROR",
                    Message = applicationException.Message
                }
            }),
            _ => ((int)HttpStatusCode.InternalServerError, new List<ApiError>
            {
                new()
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "Ocurrió un error interno del servidor."
                }
            })
        };
    }
}

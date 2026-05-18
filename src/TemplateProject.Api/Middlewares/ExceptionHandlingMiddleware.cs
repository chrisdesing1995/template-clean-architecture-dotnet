using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TemplateProject.Application.Common.Responses;

namespace TemplateProject.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, _logger);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var (statusCode, errors) = MapException(exception);
        LogException(logger, exception, traceId, context.Request.Method, context.Request.Path.Value ?? string.Empty, statusCode);

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

    private static void LogException(
        ILogger<ExceptionHandlingMiddleware> logger,
        Exception exception,
        string traceId,
        string method,
        string path,
        int statusCode)
    {
        const string messageTemplate = "Exception handled for {Method} {Path} with status code {StatusCode} and TraceId {TraceId}";

        switch (exception)
        {
            case ValidationException:
            case ApplicationException:
                logger.LogWarning(exception, messageTemplate, method, path, statusCode, traceId);
                break;
            default:
                logger.LogError(exception, messageTemplate, method, path, statusCode, traceId);
                break;
        }
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

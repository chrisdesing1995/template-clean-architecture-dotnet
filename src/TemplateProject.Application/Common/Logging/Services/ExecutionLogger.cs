using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TemplateProject.Application.Common.Logging.Interfaces;

namespace TemplateProject.Application.Common.Logging.Services;

public class ExecutionLogger : IExecutionLogger
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    private readonly ILogger<ExecutionLogger> _logger;

    public ExecutionLogger(ILogger<ExecutionLogger> logger)
    {
        _logger = logger;
    }

    public async Task<T> ExecuteAsync<T>(
        string title,
        string startMessage,
        string endMessage,
        string errorMessage,
        Func<Task<T>> action,
        object? request = null)
    {
        var traceId = GetTraceId();
        var stopwatch = Stopwatch.StartNew();
        var serializedRequest = SerializeSafely(request);

        LogStart(title, traceId, startMessage, serializedRequest);

        try
        {
            var response = await action();
            stopwatch.Stop();

            LogEnd(title, traceId, endMessage, stopwatch.ElapsedMilliseconds, SerializeSafely(response));

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            LogError(title, traceId, errorMessage, stopwatch.ElapsedMilliseconds, serializedRequest, exception);

            throw;
        }
    }

    public async Task ExecuteAsync(
        string title,
        string startMessage,
        string endMessage,
        string errorMessage,
        Func<Task> action,
        object? request = null)
    {
        var traceId = GetTraceId();
        var stopwatch = Stopwatch.StartNew();
        var serializedRequest = SerializeSafely(request);

        LogStart(title, traceId, startMessage, serializedRequest);

        try
        {
            await action();
            stopwatch.Stop();

            LogEnd(title, traceId, endMessage, stopwatch.ElapsedMilliseconds, null);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            LogError(title, traceId, errorMessage, stopwatch.ElapsedMilliseconds, serializedRequest, exception);

            throw;
        }
    }

    private void LogStart(string title, string traceId, string message, string? request)
    {
        _logger.LogInformation(
            "[START] Title: {Title} TraceId: {TraceId} Message: {Message} Request: {Request}",
            title,
            traceId,
            message,
            request);
    }

    private void LogEnd(string title, string traceId, string message, long elapsedMs, string? response)
    {
        _logger.LogInformation(
            "[END] Title: {Title} TraceId: {TraceId} Message: {Message} ElapsedMs: {ElapsedMs} Response: {Response}",
            title,
            traceId,
            message,
            elapsedMs,
            response);
    }

    private void LogError(
        string title,
        string traceId,
        string message,
        long elapsedMs,
        string? request,
        Exception exception)
    {
        _logger.LogError(
            exception,
            "[ERROR] Title: {Title} TraceId: {TraceId} Message: {Message} ElapsedMs: {ElapsedMs} Request: {Request} ExceptionMessage: {ExceptionMessage}",
            title,
            traceId,
            message,
            elapsedMs,
            request,
            exception.Message);
    }

    private static string GetTraceId()
    {
        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }

    private static string? SerializeSafely(object? value)
    {
        if (value is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
        catch
        {
            return "{\"serializationError\":true}";
        }
    }
}

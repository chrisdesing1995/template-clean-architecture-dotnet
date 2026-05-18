namespace TemplateProject.Application.Common.Logging.Interfaces;

public interface IExecutionLogger
{
    Task<T> ExecuteAsync<T>(
        string title,
        string startMessage,
        string endMessage,
        string errorMessage,
        Func<Task<T>> action,
        object? request = null);

    Task ExecuteAsync(
        string title,
        string startMessage,
        string endMessage,
        string errorMessage,
        Func<Task> action,
        object? request = null);
}

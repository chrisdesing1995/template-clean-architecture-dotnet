namespace TemplateProject.Application.Common.Responses;

public class ApiResponse<T>
{
    public string TraceId { get; set; } = default!;

    public bool Success { get; set; }

    public int Code { get; set; }

    public string Message { get; set; } = default!;

    public T? Data { get; set; }

    public List<ApiError>? Errors { get; set; }
}

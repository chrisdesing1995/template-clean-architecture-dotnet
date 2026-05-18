using TemplateProject.Application.Common.Responses;
using TemplateProject.Application.Features.Persons.DTOs;

namespace TemplateProject.Application.Features.Persons.Interfaces;

public interface IPersonService
{
    Task<ApiResponse<IReadOnlyCollection<PersonDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<PersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApiResponse<PersonDto>> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PersonDto>> UpdateAsync(Guid id, UpdatePersonRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object?>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

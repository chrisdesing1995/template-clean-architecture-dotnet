using FluentValidation;
using TemplateProject.Application.Common.Logging.Interfaces;
using TemplateProject.Application.Common.Responses;
using TemplateProject.Application.Features.Persons.DTOs;
using TemplateProject.Application.Features.Persons.Interfaces;
using TemplateProject.Domain.Entities;

namespace TemplateProject.Application.Features.Persons.Services;

public class PersonService : IPersonService
{
    private readonly IExecutionLogger _executionLogger;
    private readonly IPersonRepository _personRepository;
    private readonly IValidator<CreatePersonRequest> _createPersonValidator;
    private readonly IValidator<UpdatePersonRequest> _updatePersonValidator;

    public PersonService(
        IExecutionLogger executionLogger,
        IPersonRepository personRepository,
        IValidator<CreatePersonRequest> createPersonValidator,
        IValidator<UpdatePersonRequest> updatePersonValidator)
    {
        _executionLogger = executionLogger;
        _personRepository = personRepository;
        _createPersonValidator = createPersonValidator;
        _updatePersonValidator = updatePersonValidator;
    }

    public Task<ApiResponse<IReadOnlyCollection<PersonDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _executionLogger.ExecuteAsync(
            title: "GetAllPersons",
            startMessage: "Starting person retrieval process.",
            endMessage: "Persons retrieved successfully.",
            errorMessage: "Error retrieving persons.",
            request: null,
            action: async () =>
            {
                var persons = await _personRepository.GetAllAsync(cancellationToken);
                var result = persons
                    .Select(person => person.ToDto())
                    .ToList()
                    .AsReadOnly();

                return CreateSuccessResponse<IReadOnlyCollection<PersonDto>>(result, "Operación exitosa.");
            });
    }

    public Task<ApiResponse<PersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _executionLogger.ExecuteAsync(
            title: "GetPersonById",
            startMessage: "Starting person retrieval by id.",
            endMessage: "Person retrieved successfully.",
            errorMessage: "Error retrieving person by id.",
            request: new { PersonId = id },
            action: async () =>
            {
                var person = await _personRepository.GetByIdAsync(id, cancellationToken);
                if (person is null)
                {
                    throw new ApplicationException("La persona no existe.");
                }

                return CreateSuccessResponse(person.ToDto(), "Operación exitosa.");
            });
    }

    public Task<ApiResponse<PersonDto>> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default)
    {
        return _executionLogger.ExecuteAsync(
            title: "CreatePerson",
            startMessage: "Starting person creation process.",
            endMessage: "Person created successfully.",
            errorMessage: "Error creating person.",
            request: request,
            action: async () =>
            {
                await _createPersonValidator.ValidateAndThrowAsync(request, cancellationToken);

                var person = new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Email = request.Email.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPerson = await _personRepository.AddAsync(person, cancellationToken);

                return CreateSuccessResponse(createdPerson.ToDto(), "Persona creada exitosamente.");
            });
    }

    public Task<ApiResponse<PersonDto>> UpdateAsync(Guid id, UpdatePersonRequest request, CancellationToken cancellationToken = default)
    {
        return _executionLogger.ExecuteAsync(
            title: "UpdatePerson",
            startMessage: "Starting person update process.",
            endMessage: "Person updated successfully.",
            errorMessage: "Error updating person.",
            request: new
            {
                PersonId = id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.IsActive
            },
            action: async () =>
            {
                await _updatePersonValidator.ValidateAndThrowAsync(request, cancellationToken);

                var existingPerson = await _personRepository.GetByIdAsync(id, cancellationToken);
                if (existingPerson is null)
                {
                    throw new ApplicationException("La persona no existe.");
                }

                existingPerson.FirstName = request.FirstName.Trim();
                existingPerson.LastName = request.LastName.Trim();
                existingPerson.Email = request.Email.Trim();
                existingPerson.IsActive = request.IsActive;

                var updatedPerson = await _personRepository.UpdateAsync(existingPerson, cancellationToken);

                return CreateSuccessResponse(updatedPerson.ToDto(), "Persona actualizada exitosamente.");
            });
    }

    public async Task<ApiResponse<object?>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _executionLogger.ExecuteAsync(
            title: "DeletePerson",
            startMessage: "Starting person deletion process.",
            endMessage: "Person deleted successfully.",
            errorMessage: "Error deleting person.",
            request: new { PersonId = id },
            action: async () =>
            {
                var deleted = await _personRepository.DeleteAsync(id, cancellationToken);
                if (!deleted)
                {
                    throw new ApplicationException("La persona no existe.");
                }
            });

        return CreateSuccessResponse<object?>(null, "Persona eliminada exitosamente.");
    }

    private static ApiResponse<T> CreateSuccessResponse<T>(T data, string message)
    {
        return new ApiResponse<T>
        {
            TraceId = GetTraceId(),
            Success = true,
            Code = 200,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    private static string GetTraceId()
    {
        return System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Application.Common.Responses;
using TemplateProject.Application.Features.Persons.DTOs;
using TemplateProject.Application.Features.Persons.Interfaces;
using TemplateProject.Domain.Entities;

namespace TemplateProject.Api.Controllers;

[ApiController]
[Route("api/v1/persons")]
[Produces("application/json")]
public class PersonsController : ControllerBase
{
    private readonly IPersonRepository _personRepository;
    private readonly IValidator<CreatePersonRequest> _createPersonValidator;
    private readonly IValidator<UpdatePersonRequest> _updatePersonValidator;

    public PersonsController(
        IPersonRepository personRepository,
        IValidator<CreatePersonRequest> createPersonValidator,
        IValidator<UpdatePersonRequest> updatePersonValidator)
    {
        _personRepository = personRepository;
        _createPersonValidator = createPersonValidator;
        _updatePersonValidator = updatePersonValidator;
    }

    /// <summary>
    /// Gets all persons.
    /// </summary>
    /// <returns>A standardized response with the list of persons.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PersonDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var persons = await _personRepository.GetAllAsync(cancellationToken);
        var result = persons
            .Select(person => person.ToDto())
            .ToList()
            .AsReadOnly();

        return Ok(CreateSuccessResponse(result, "Operación exitosa."));
    }

    /// <summary>
    /// Gets a person by identifier.
    /// </summary>
    /// <param name="id">Person identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A standardized response with the requested person.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(id, cancellationToken);
        if (person is null)
        {
            throw new ApplicationException("La persona no existe.");
        }

        return Ok(CreateSuccessResponse(person.ToDto(), "Operación exitosa."));
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="request">Payload used to create the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A standardized response with the created person.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest? request, CancellationToken cancellationToken)
    {
        var validRequest = EnsureRequestBody(request);
        await _createPersonValidator.ValidateAndThrowAsync(validRequest, cancellationToken);

        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = validRequest.FirstName.Trim(),
            LastName = validRequest.LastName.Trim(),
            Email = validRequest.Email.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdPerson = await _personRepository.AddAsync(person, cancellationToken);

        return Ok(CreateSuccessResponse(createdPerson.ToDto(), "Persona creada exitosamente."));
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">Person identifier.</param>
    /// <param name="request">Payload used to update the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A standardized response with the updated person.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonRequest? request, CancellationToken cancellationToken)
    {
        var validRequest = EnsureRequestBody(request);
        await _updatePersonValidator.ValidateAndThrowAsync(validRequest, cancellationToken);

        var existingPerson = await _personRepository.GetByIdAsync(id, cancellationToken);
        if (existingPerson is null)
        {
            throw new ApplicationException("La persona no existe.");
        }

        existingPerson.FirstName = validRequest.FirstName.Trim();
        existingPerson.LastName = validRequest.LastName.Trim();
        existingPerson.Email = validRequest.Email.Trim();
        existingPerson.IsActive = validRequest.IsActive;

        var updatedPerson = await _personRepository.UpdateAsync(existingPerson, cancellationToken);

        return Ok(CreateSuccessResponse(updatedPerson.ToDto(), "Persona actualizada exitosamente."));
    }

    /// <summary>
    /// Deletes a person by identifier.
    /// </summary>
    /// <param name="id">Person identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A standardized response confirming the deletion.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _personRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new ApplicationException("La persona no existe.");
        }

        return Ok(CreateSuccessResponse<object?>(null, "Persona eliminada exitosamente."));
    }

    private static TRequest EnsureRequestBody<TRequest>(TRequest? request)
        where TRequest : class
    {
        if (request is null)
        {
            throw new ApplicationException("El cuerpo de la solicitud es obligatorio.");
        }

        return request;
    }

    private ApiResponse<T> CreateSuccessResponse<T>(T data, string message)
    {
        return new ApiResponse<T>
        {
            TraceId = HttpContext.TraceIdentifier,
            Success = true,
            Code = StatusCodes.Status200OK,
            Message = message,
            Data = data,
            Errors = null
        };
    }
}

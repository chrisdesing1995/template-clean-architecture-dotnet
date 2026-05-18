using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Application.Common.Responses;
using TemplateProject.Application.Features.Persons.DTOs;
using TemplateProject.Application.Features.Persons.Interfaces;

namespace TemplateProject.Api.Controllers;

[ApiController]
[Route("api/v1/persons")]
[Produces("application/json")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonsController(
        IPersonService personService)
    {
        _personService = personService;
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
        var response = await _personService.GetAllAsync(cancellationToken);
        return Ok(response);
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
        var response = await _personService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
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
        var response = await _personService.CreateAsync(validRequest, cancellationToken);
        return Ok(response);
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
        var response = await _personService.UpdateAsync(id, validRequest, cancellationToken);
        return Ok(response);
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
        var response = await _personService.DeleteAsync(id, cancellationToken);
        return Ok(response);
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
}

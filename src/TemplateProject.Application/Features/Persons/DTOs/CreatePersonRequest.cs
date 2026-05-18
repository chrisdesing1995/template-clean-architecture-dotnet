namespace TemplateProject.Application.Features.Persons.DTOs;

/// <summary>
/// Request payload used to create a person.
/// </summary>
public class CreatePersonRequest
{
    /// <summary>
    /// First name of the person.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Last name of the person.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Email address of the person.
    /// </summary>
    public string Email { get; set; } = default!;
}

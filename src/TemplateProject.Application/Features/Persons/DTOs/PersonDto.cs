namespace TemplateProject.Application.Features.Persons.DTOs;

/// <summary>
/// Represents a person returned by the API.
/// </summary>
public class PersonDto
{
    /// <summary>
    /// Unique identifier of the person.
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    /// Indicates whether the person is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

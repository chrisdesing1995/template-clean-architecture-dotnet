namespace TemplateProject.Domain.Entities;

public class Person
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

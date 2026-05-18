namespace TemplateProject.Application.Features.Persons.Commands;

public class CreatePersonCommand
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;
}

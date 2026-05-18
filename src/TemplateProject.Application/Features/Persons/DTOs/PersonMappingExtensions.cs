using TemplateProject.Domain.Entities;

namespace TemplateProject.Application.Features.Persons.DTOs;

public static class PersonMappingExtensions
{
    public static PersonDto ToDto(this Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            IsActive = person.IsActive,
            CreatedAt = person.CreatedAt
        };
    }
}

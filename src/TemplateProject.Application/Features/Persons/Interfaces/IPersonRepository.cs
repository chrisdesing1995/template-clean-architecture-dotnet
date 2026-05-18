using TemplateProject.Domain.Entities;

namespace TemplateProject.Application.Features.Persons.Interfaces;

public interface IPersonRepository
{
    Task<IReadOnlyCollection<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default);

    Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

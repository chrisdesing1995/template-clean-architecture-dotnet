using TemplateProject.Application.Features.Persons.Interfaces;
using TemplateProject.Domain.Entities;

namespace TemplateProject.Infrastructure.Persistence.Repositories;

public class PersonRepository : IPersonRepository
{
    private static readonly object SyncRoot = new();

    private static readonly List<Person> Persons =
    [
        new Person
        {
            Id = Guid.Parse("8b2fe2f4-4e2d-4524-83ce-aea4d6a5f4b7"),
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada.lovelace@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }
    ];

    public Task<IReadOnlyCollection<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (SyncRoot)
        {
            var items = Persons
                .Select(Clone)
                .ToList()
                .AsReadOnly();

            return Task.FromResult<IReadOnlyCollection<Person>>(items);
        }
    }

    public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (SyncRoot)
        {
            var person = Persons.FirstOrDefault(item => item.Id == id);
            return Task.FromResult(person is null ? null : Clone(person));
        }
    }

    public Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        lock (SyncRoot)
        {
            var createdPerson = Clone(person);
            Persons.Add(createdPerson);
            return Task.FromResult(Clone(createdPerson));
        }
    }

    public Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken = default)
    {
        lock (SyncRoot)
        {
            var existingPerson = Persons.First(item => item.Id == person.Id);
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.Email = person.Email;
            existingPerson.IsActive = person.IsActive;

            return Task.FromResult(Clone(existingPerson));
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (SyncRoot)
        {
            var person = Persons.FirstOrDefault(item => item.Id == id);
            if (person is null)
            {
                return Task.FromResult(false);
            }

            Persons.Remove(person);
            return Task.FromResult(true);
        }
    }

    private static Person Clone(Person person)
    {
        return new Person
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

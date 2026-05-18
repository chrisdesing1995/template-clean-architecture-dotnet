using FluentValidation;
using TemplateProject.Application.Features.Persons.DTOs;

namespace TemplateProject.Application.Features.Persons.Validators;

public class UpdatePersonRequestValidator : AbstractValidator<UpdatePersonRequest>
{
    public UpdatePersonRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .WithErrorCode("VALIDATION_ERROR")
            .WithMessage("El nombre es obligatorio.");

        RuleFor(request => request.LastName)
            .NotEmpty()
            .WithErrorCode("VALIDATION_ERROR")
            .WithMessage("El apellido es obligatorio.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithErrorCode("VALIDATION_ERROR")
            .WithMessage("El correo es obligatorio.");
    }
}

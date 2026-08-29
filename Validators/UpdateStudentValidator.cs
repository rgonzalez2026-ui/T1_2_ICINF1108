using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

/// <summary>
/// Valida únicamente los campos enviados en una actualización parcial.
/// </summary>
public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentValidator()
    {
        RuleFor(student => student.Name)
            .NotEmpty().WithMessage("El nombre no puede estar vacío.")
            .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres.")
            .Matches(@"^[^<>]*$").WithMessage("El nombre no puede contener etiquetas HTML.")
            .When(student => student.Name is not null);

        RuleFor(student => student.Email)
            .NotEmpty().WithMessage("El email no puede estar vacío.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .When(student => student.Email is not null);

        RuleFor(student => student.Age)
            .InclusiveBetween(18, 99).WithMessage("La edad debe estar entre 18 y 99.")
            .When(student => student.Age.HasValue);
    }
}

using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

// Reglas usadas por POST /api/students mediante FluentValidation:
// name (3-100 caracteres, sin HTML), email valido, age entre 18 y 99.
public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres.")
            .Matches(@"^[^<>]*$").WithMessage("El nombre no puede contener etiquetas HTML.");

        RuleFor(s => s.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato valido.");

        RuleFor(s => s.Age)
            .InclusiveBetween(18, 99).WithMessage("La edad debe estar entre 18 y 99.");
    }
}

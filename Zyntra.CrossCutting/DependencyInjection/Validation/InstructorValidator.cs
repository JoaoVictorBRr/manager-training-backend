using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class InstructorValidator : AbstractValidator<Instructor>
{
    public InstructorValidator()
    {
        RuleFor(i => i.UserId)
            .GreaterThan(0).WithMessage("O campo UserId deve ser maior que zero.");

        RuleFor(i => i.Specialty)
            .NotEmpty().WithMessage("O campo especialidade não pode ser vazio.")
            .MaximumLength(100).WithMessage("O campo especialidade não pode ter mais de 100 caracteres.");

        RuleFor(i => i.Cref)
            .NotEmpty().WithMessage("O campo CREF não pode ser vazio.")
            .MaximumLength(20).WithMessage("O campo CREF não pode ter mais de 20 caracteres.");
    }
}

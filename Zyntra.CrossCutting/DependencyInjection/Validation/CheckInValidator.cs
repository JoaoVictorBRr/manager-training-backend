using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class CheckInValidator : AbstractValidator<CheckIn>
{
    public CheckInValidator()
    {
        RuleFor(c => c.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(c => c.Unit)
            .NotEmpty().WithMessage("O campo unidade não pode ser vazio.")
            .MaximumLength(80).WithMessage("O campo unidade não pode ter mais de 80 caracteres.");
    }
}

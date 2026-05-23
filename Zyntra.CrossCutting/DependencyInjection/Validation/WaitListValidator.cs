using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class WaitListValidator : AbstractValidator<WaitList>
{
    public WaitListValidator()
    {
        RuleFor(w => w.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(w => w.ClassId)
            .GreaterThan(0).WithMessage("O campo ClassId deve ser maior que zero.");

        RuleFor(w => w.Position)
            .GreaterThan(0).WithMessage("A posição na fila deve ser maior que zero.");

        RuleFor(w => w.InclusionDateTime)
            .NotEmpty().WithMessage("O campo data de inclusão não pode ser vazio.");
    }
}

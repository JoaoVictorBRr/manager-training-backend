using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class AdministratorValidator : AbstractValidator<Administrator>
{
    public AdministratorValidator()
    {
        RuleFor(a => a.UserId)
            .GreaterThan(0).WithMessage("O campo UserId deve ser maior que zero.");

        RuleFor(a => a.AccessLevel)
            .GreaterThan(0).WithMessage("O nível de acesso deve ser maior que zero.");
    }
}

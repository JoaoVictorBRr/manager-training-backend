using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class PartnerIntegrationValidator : AbstractValidator<PartnerIntegration>
{
    public PartnerIntegrationValidator()
    {
        RuleFor(p => p.PartnerName)
            .NotEmpty().WithMessage("O campo nome do parceiro não pode ser vazio.")
            .MaximumLength(80).WithMessage("O campo nome do parceiro não pode ter mais de 80 caracteres.");

        RuleFor(p => p.Token)
            .NotEmpty().WithMessage("O campo token não pode ser vazio.")
            .MaximumLength(500).WithMessage("O campo token não pode ter mais de 500 caracteres.");
    }
}

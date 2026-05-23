using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator()
    {
        RuleFor(p => p.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(p => p.Amount)
            .GreaterThan(0).WithMessage("O valor do pagamento deve ser maior que zero.");

        RuleFor(p => p.DueDate)
            .NotEmpty().WithMessage("O campo data de vencimento não pode ser vazio.");
    }
}

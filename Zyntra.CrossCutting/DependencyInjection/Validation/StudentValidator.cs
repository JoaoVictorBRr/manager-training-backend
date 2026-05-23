using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class StudentValidator : AbstractValidator<Student>
{
    public StudentValidator()
    {
        RuleFor(s => s.UserId)
            .GreaterThan(0).WithMessage("O campo UserId deve ser maior que zero.");

        RuleFor(s => s.BirthDate)
            .NotEmpty().WithMessage("O campo data de nascimento não pode ser vazio.")
            .LessThan(DateTime.Now).WithMessage("A data de nascimento deve ser anterior à data atual.");

        RuleFor(s => s.PaymentStatus)
            .NotEmpty().WithMessage("O campo status de pagamento não pode ser vazio.")
            .MaximumLength(30).WithMessage("O campo status de pagamento não pode ter mais de 30 caracteres.");
    }
}

using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class NotificationValidator : AbstractValidator<Notification>
{
    public NotificationValidator()
    {
        RuleFor(n => n.UserId)
            .GreaterThan(0).WithMessage("O campo UserId deve ser maior que zero.");

        RuleFor(n => n.Title)
            .NotEmpty().WithMessage("O campo título não pode ser vazio.")
            .MaximumLength(150).WithMessage("O campo título não pode ter mais de 150 caracteres.");

        RuleFor(n => n.Message)
            .NotEmpty().WithMessage("O campo mensagem não pode ser vazio.")
            .MaximumLength(500).WithMessage("O campo mensagem não pode ter mais de 500 caracteres.");

        RuleFor(n => n.SendDateTime)
            .NotEmpty().WithMessage("O campo data de envio não pode ser vazio.");
    }
}

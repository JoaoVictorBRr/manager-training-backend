using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class ChatMessageValidator : AbstractValidator<ChatMessage>
{
    public ChatMessageValidator()
    {
        RuleFor(c => c.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(c => c.InstructorId)
            .GreaterThan(0).WithMessage("O campo InstructorId deve ser maior que zero.");

        RuleFor(c => c.Message)
            .NotEmpty().WithMessage("O campo mensagem não pode ser vazio.")
            .MaximumLength(2000).WithMessage("A mensagem não pode ter mais de 2000 caracteres.");

        RuleFor(c => c.MessageDateTime)
            .NotEmpty().WithMessage("O campo data da mensagem não pode ser vazio.");
    }
}

using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("O campo nome não pode ser vazio")
            .MaximumLength(100).WithMessage("O campo nome não pode ter mais de 100 caracteres");

        RuleFor(c => c.Cpf)
            .NotEmpty().WithMessage("O campo cpf não pode ser vazio")
            .MaximumLength(14).WithMessage("O campo cpf não pode ter mais de 14 caracteres");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("O campo senha não pode ser vazio")
            .MaximumLength(50).WithMessage("O campo senha pode ter no máximo 50 caracteres")
            .MinimumLength(6).WithMessage("O campo senhaprecisar ter no mínimo 6 caracteres");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O campo e-mail não pode ser vazio")
            .MaximumLength(80).WithMessage("O campo e-mail pode ter no máximo 80 caracteres");
    }
}

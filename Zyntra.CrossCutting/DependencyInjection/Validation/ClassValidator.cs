using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class ClassValidator : AbstractValidator<Class>
{
    public ClassValidator()
    {
        RuleFor(c => c.Modality)
            .NotEmpty().WithMessage("O campo modalidade não pode ser vazio.")
            .MaximumLength(80).WithMessage("O campo modalidade não pode ter mais de 80 caracteres.");

        RuleFor(c => c.MaxCapacity)
            .GreaterThan(0).WithMessage("A capacidade máxima deve ser maior que zero.");

        RuleFor(c => c.AvailableSlots)
            .GreaterThanOrEqualTo(0).WithMessage("As vagas disponíveis não podem ser negativas.");

        RuleFor(c => c.Unit)
            .NotEmpty().WithMessage("O campo unidade não pode ser vazio.")
            .MaximumLength(80).WithMessage("O campo unidade não pode ter mais de 80 caracteres.");

        RuleFor(c => c.InstructorId)
            .GreaterThan(0).WithMessage("O campo InstructorId deve ser maior que zero.");
    }
}

using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class PhysicalAssessmentValidator : AbstractValidator<PhysicalAssessment>
{
    public PhysicalAssessmentValidator()
    {
        RuleFor(p => p.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(p => p.Weight)
            .GreaterThan(0).WithMessage("O peso deve ser maior que zero.");

        RuleFor(p => p.Height)
            .GreaterThan(0).WithMessage("A altura deve ser maior que zero.");

        RuleFor(p => p.AssessmentDate)
            .NotEmpty().WithMessage("O campo data de avaliação não pode ser vazio.");
    }
}

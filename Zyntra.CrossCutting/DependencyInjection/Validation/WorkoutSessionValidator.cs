using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class WorkoutSessionValidator : AbstractValidator<WorkoutSession>
{
    public WorkoutSessionValidator()
    {
        RuleFor(s => s.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(s => s.WorkoutDay)
            .NotEmpty().WithMessage("O dia do treino não pode ser vazio.");
    }
}

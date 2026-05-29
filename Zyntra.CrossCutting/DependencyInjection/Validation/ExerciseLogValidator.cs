using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class ExerciseLogValidator : AbstractValidator<ExerciseLog>
{
    public ExerciseLogValidator()
    {
        RuleFor(l => l.WorkoutSessionId)
            .GreaterThan(0).WithMessage("O campo WorkoutSessionId deve ser maior que zero.");

        RuleFor(l => l.ExerciseName)
            .NotEmpty().WithMessage("O nome do exercício não pode ser vazio.");
    }
}

using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class ExerciseValidator : AbstractValidator<Exercise>
{
    public ExerciseValidator()
    {
        RuleFor(e => e.WorkoutSheetId)
            .GreaterThan(0).WithMessage("O campo WorkoutSheetId deve ser maior que zero.");

        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("O campo nome não pode ser vazio.")
            .MaximumLength(100).WithMessage("O campo nome não pode ter mais de 100 caracteres.");

        RuleFor(e => e.MuscleGroup)
            .NotEmpty().WithMessage("O campo grupo muscular não pode ser vazio.")
            .MaximumLength(80).WithMessage("O campo grupo muscular não pode ter mais de 80 caracteres.");

        RuleFor(e => e.Sets)
            .GreaterThan(0).WithMessage("O número de séries deve ser maior que zero.");

        RuleFor(e => e.Repetitions)
            .NotEmpty().WithMessage("O campo repetições não pode ser vazio.")
            .MaximumLength(30).WithMessage("O campo repetições não pode ter mais de 30 caracteres.");
    }
}

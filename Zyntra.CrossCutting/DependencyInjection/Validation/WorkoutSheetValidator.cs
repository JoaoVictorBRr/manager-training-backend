using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class WorkoutSheetValidator : AbstractValidator<WorkoutSheet>
{
    public WorkoutSheetValidator()
    {
        RuleFor(w => w.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(w => w.InstructorId)
            .GreaterThan(0).WithMessage("O campo InstructorId deve ser maior que zero.");

        RuleFor(w => w.StartDate)
            .NotEmpty().WithMessage("O campo data de início não pode ser vazio.");
    }
}

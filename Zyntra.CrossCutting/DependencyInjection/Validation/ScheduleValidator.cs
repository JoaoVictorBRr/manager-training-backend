using FluentValidation;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.Validation;

public class ScheduleValidator : AbstractValidator<Schedule>
{
    public ScheduleValidator()
    {
        RuleFor(s => s.StudentId)
            .GreaterThan(0).WithMessage("O campo StudentId deve ser maior que zero.");

        RuleFor(s => s.ClassId)
            .GreaterThan(0).WithMessage("O campo ClassId deve ser maior que zero.");

        RuleFor(s => s.ReservationDate)
            .NotEmpty().WithMessage("O campo data de reserva não pode ser vazio.");
    }
}

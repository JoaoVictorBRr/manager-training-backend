using Zyntra.Domain.Entities;

namespace Zyntra.Domain.Interface.Service;

public interface IWorkoutTemplateService
{
    Task<WorkoutSheet> GenerateWorkoutAsync(long studentId, int trainingDays);
}

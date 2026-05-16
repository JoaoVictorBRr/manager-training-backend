using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IWorkoutSheetRepository : IRepositoryBase<WorkoutSheet>
{
    Task<WorkoutSheet> GetActiveByStudentAsync(long studentId);
}

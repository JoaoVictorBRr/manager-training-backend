using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IStudentAchievementRepository : IRepositoryBase<StudentAchievement>
{
    Task<IEnumerable<StudentAchievement>> GetByStudentAsync(long studentId);
    Task<bool> ExistsAsync(long studentId, string achievementKey);
}

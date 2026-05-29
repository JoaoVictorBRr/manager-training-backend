using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class StudentAchievementRepository(ZyntraContext zyntraContext) : BaseRepository<StudentAchievement>(zyntraContext), IStudentAchievementRepository
{
    public async Task<IEnumerable<StudentAchievement>> GetByStudentAsync(long studentId)
        => await DbSet.AsNoTracking()
            .Where(a => a.StudentId == studentId && a.Situation == Domain.Enum.Situation.Active)
            .OrderBy(a => a.UnlockedAt)
            .ToListAsync();

    public async Task<bool> ExistsAsync(long studentId, string achievementKey)
        => await DbSet.AsNoTracking()
            .AnyAsync(a => a.StudentId == studentId && a.AchievementKey == achievementKey && a.Situation == Domain.Enum.Situation.Active);
}

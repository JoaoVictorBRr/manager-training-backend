using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class PhysicalAssessmentRepository(ZyntraContext zyntraContext) : BaseRepository<PhysicalAssessment>(zyntraContext), IPhysicalAssessmentRepository
{
    public async Task<IEnumerable<PhysicalAssessment>> GetHistoryByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.AssessmentDate)
            .ToListAsync();
    }
}

using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IPhysicalAssessmentRepository : IRepositoryBase<PhysicalAssessment>
{
    Task<IEnumerable<PhysicalAssessment>> GetHistoryByStudentAsync(long studentId);
}

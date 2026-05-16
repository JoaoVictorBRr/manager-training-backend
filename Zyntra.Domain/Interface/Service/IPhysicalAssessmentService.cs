using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IPhysicalAssessmentService : IServiceBase<PhysicalAssessment>
{
    Task<IEnumerable<PhysicalAssessment>> GetHistoryByStudentAsync(long studentId);
}

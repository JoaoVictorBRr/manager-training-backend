using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class PhysicalAssessmentService(IPhysicalAssessmentRepository physicalAssessmentRepository) : BaseService<PhysicalAssessment>(physicalAssessmentRepository), IPhysicalAssessmentService
{
    private readonly IPhysicalAssessmentRepository _physicalAssessmentRepository = physicalAssessmentRepository;

    public override async Task<PhysicalAssessment> AddAsync(PhysicalAssessment entity)
    {
        entity.Bmi = Math.Round(entity.Weight / (entity.Height * entity.Height), 2);
        return await base.AddAsync(entity);
    }

    public Task<IEnumerable<PhysicalAssessment>> GetHistoryByStudentAsync(long studentId)
        => _physicalAssessmentRepository.GetHistoryByStudentAsync(studentId);
}

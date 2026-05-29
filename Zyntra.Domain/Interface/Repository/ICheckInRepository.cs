using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface ICheckInRepository : IRepositoryBase<CheckIn>
{
    Task<CheckIn> GetTodayByStudentAsync(long studentId);
    Task<IEnumerable<CheckIn>> GetByStudentAsync(long studentId);
}

using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IScheduleRepository : IRepositoryBase<Schedule>
{
    Task<PagedListDto<Schedule>> FilterAllSchedules(ScheduleRequestListDto filter);
    Task<Schedule> GetByStudentAndClassAsync(long studentId, long classId);
    Task<IEnumerable<Schedule>> GetActiveByStudentAsync(long studentId);
}

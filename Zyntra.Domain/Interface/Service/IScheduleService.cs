using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IScheduleService : IServiceBase<Schedule>
{
    Task<PagedListDto<Schedule>> FilterAllSchedules(ScheduleRequestListDto filter);
    Task<Schedule> ReserveAsync(long studentId, long classId);
    Task<Schedule> CancelAsync(long scheduleId);
    Task<WaitList> JoinWaitListAsync(long studentId, long classId);
}

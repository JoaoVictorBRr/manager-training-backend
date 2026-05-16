using Zyntra.Domain.Dtos.Base.List;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ScheduleDto;

public class ScheduleRequestListDto : BaseRequestListDto
{
    public long? StudentId { get; set; }
    public long? ClassId { get; set; }
    public ScheduleStatus? Status { get; set; }
}

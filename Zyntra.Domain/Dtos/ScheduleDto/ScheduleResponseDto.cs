using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ScheduleDto;

public class ScheduleResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public long ClassId { get; set; }
    public string ClassModality { get; set; }
    public DateTime ClassDateTime { get; set; }
    public ScheduleStatus Status { get; set; }
    public DateTime ReservationDate { get; set; }
}

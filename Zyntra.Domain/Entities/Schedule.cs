using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class Schedule : EntityBase
{
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public ScheduleStatus Status { get; set; }
    public DateTime ReservationDate { get; set; }

    public Student Student { get; set; }
    public Class Class { get; set; }
}

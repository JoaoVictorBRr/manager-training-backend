using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class CheckIn : EntityBase
{
    public long StudentId { get; set; }
    public DateTime DateTime { get; set; }
    public string Unit { get; set; }
    public CheckInType AccessType { get; set; }
    public CheckInStatus ValidationStatus { get; set; }
    public string? WorkoutDayPerformed { get; set; }

    public Student Student { get; set; }
}

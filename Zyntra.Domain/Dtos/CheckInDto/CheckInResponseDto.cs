using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.CheckInDto;

public class CheckInResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public DateTime DateTime { get; set; }
    public string Unit { get; set; }
    public CheckInType AccessType { get; set; }
    public CheckInStatus ValidationStatus { get; set; }
    public string? WorkoutDayPerformed { get; set; }
}

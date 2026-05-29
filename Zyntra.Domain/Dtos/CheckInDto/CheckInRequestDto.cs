using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.CheckInDto;

public class CheckInRequestDto
{
    public long StudentId { get; set; }
    public CheckInType AccessType { get; set; }
    public string? WorkoutDayPerformed { get; set; }
}

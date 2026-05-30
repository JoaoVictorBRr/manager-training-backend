namespace Zyntra.Domain.Dtos.CheckInDto;

public class CheckInHistoryDto
{
    public long Id { get; set; }
    public DateTime DateTime { get; set; }
    public string? WorkoutDayPerformed { get; set; }
}

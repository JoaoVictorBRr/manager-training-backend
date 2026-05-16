namespace Zyntra.Domain.Dtos.WorkoutSheetDto;

public class WorkoutSheetUpdateDto
{
    public long Id { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Notes { get; set; }
}

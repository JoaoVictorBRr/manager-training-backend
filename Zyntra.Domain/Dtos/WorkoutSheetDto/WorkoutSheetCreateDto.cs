namespace Zyntra.Domain.Dtos.WorkoutSheetDto;

public class WorkoutSheetCreateDto
{
    public long StudentId { get; set; }
    public long InstructorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Notes { get; set; }
}

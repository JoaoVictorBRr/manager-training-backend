using Zyntra.Domain.Dtos.ExerciseDto;

namespace Zyntra.Domain.Dtos.WorkoutSheetDto;

public class WorkoutSheetResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Notes { get; set; }
    public IEnumerable<ExerciseResponseDto> Exercises { get; set; }
}

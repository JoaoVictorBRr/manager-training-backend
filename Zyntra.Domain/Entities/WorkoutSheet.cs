using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class WorkoutSheet : EntityBase
{
    public long StudentId { get; set; }
    public long InstructorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Notes { get; set; }

    public Student Student { get; set; }
    public Instructor Instructor { get; set; }
    public ICollection<Exercise> Exercises { get; set; }
}

using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Class : EntityBase
{
    public string Modality { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxCapacity { get; set; }
    public int AvailableSlots { get; set; }
    public string Unit { get; set; }
    public long InstructorId { get; set; }

    public Instructor Instructor { get; set; }
    public ICollection<Schedule> Schedules { get; set; }
    public ICollection<WaitList> WaitLists { get; set; }
}

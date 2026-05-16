using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class ChatMessage : EntityBase
{
    public long StudentId { get; set; }
    public long InstructorId { get; set; }
    public string Message { get; set; }
    public DateTime MessageDateTime { get; set; }
    public bool IsRead { get; set; }

    public Student Student { get; set; }
    public Instructor Instructor { get; set; }
}

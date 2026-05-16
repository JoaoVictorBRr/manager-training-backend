using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ClassDto;

public class ClassResponseDto
{
    public long Id { get; set; }
    public string Modality { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxCapacity { get; set; }
    public int AvailableSlots { get; set; }
    public string Unit { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; }
    public Situation Situation { get; set; }
}

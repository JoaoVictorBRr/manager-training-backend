namespace Zyntra.Domain.Dtos.ClassDto;

public class ClassCreateDto
{
    public string Modality { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxCapacity { get; set; }
    public string Unit { get; set; }
    public long InstructorId { get; set; }
}

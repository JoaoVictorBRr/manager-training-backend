using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.InstructorDto;

public class InstructorResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Specialty { get; set; }
    public string Cref { get; set; }
    public Situation Situation { get; set; }
}

using Zyntra.Domain.Dtos.Base.List;

namespace Zyntra.Domain.Dtos.InstructorDto;

public class InstructorRequestListDto : BaseRequestListDto
{
    public string Name { get; set; }
    public string Specialty { get; set; }
}

using Zyntra.Domain.Dtos.Base.List;

namespace Zyntra.Domain.Dtos.StudentDto;

public class StudentRequestListDto : BaseRequestListDto
{
    public string Name { get; set; }
    public string Email { get; set; }
}

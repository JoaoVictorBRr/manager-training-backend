using Zyntra.Domain.Dtos.Base.List;

namespace Zyntra.Domain.Dtos.ClassDto;

public class ClassRequestListDto : BaseRequestListDto
{
    public string Modality { get; set; }
    public string Unit { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool? OnlyAvailable { get; set; }
}

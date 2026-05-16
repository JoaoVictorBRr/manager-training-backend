using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.Base.List;

public class BaseRequestListDto
{
    public Situation Situation { get; set; }
    public int Start { get; set; }
    public int Take { get; set; } = 10;
    public int Draw { get; set; }
}

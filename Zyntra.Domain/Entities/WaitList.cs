using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class WaitList : EntityBase
{
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public int Position { get; set; }
    public DateTime InclusionDateTime { get; set; }

    public Student Student { get; set; }
    public Class Class { get; set; }
}

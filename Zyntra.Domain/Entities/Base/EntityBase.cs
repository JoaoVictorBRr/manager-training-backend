using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities.Base;

public class EntityBase
{
    public long Id { get; set; }
    public Situation Situation { get; set; } = Situation.Active;
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public long UserIdCreated { get; set; }
    public DateTime? DateModified { get; set; }
    public long? UserIdModified { get; set; }
    public DateTime? DateDeleted { get; set; }
    public long? UserIdDeleted { get; set; }
    public string Observation { get; set; }
}

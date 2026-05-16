using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Administrator : EntityBase
{
    public long UserId { get; set; }
    public int AccessLevel { get; set; }

    public User User { get; set; }
}

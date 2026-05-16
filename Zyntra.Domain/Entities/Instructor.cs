using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Instructor : EntityBase
{
    public long UserId { get; set; }
    public string Specialty { get; set; }
    public string Cref { get; set; }

    public User User { get; set; }
}

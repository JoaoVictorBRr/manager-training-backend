using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Student : EntityBase
{
    public long UserId { get; set; }
    public DateTime BirthDate { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime? LastAccessDate { get; set; }

    public User User { get; set; }
}

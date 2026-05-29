using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class HydrationLog : EntityBase
{
    public long StudentId { get; set; }
    public DateTime LogDate { get; set; } = DateTime.Today;
    public decimal AmountMl { get; set; }

    public Student Student { get; set; }
}

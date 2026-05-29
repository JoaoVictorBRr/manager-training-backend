using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class EvolutionPhoto : EntityBase
{
    public long StudentId { get; set; }
    public string ImagePath { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.Now;
    public string? Notes { get; set; }

    public Student Student { get; set; }
}

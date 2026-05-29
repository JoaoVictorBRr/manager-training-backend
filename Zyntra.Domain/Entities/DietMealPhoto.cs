using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class DietMealPhoto : EntityBase
{
    public long DietMealOptionId { get; set; }
    public string ImagePath { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public DietMealOption DietMealOption { get; set; }
}

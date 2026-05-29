using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class DietMeal : EntityBase
{
    public long StudentDietId { get; set; }
    public MealType MealType { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public StudentDiet StudentDiet { get; set; }
    public ICollection<DietMealOption> Options { get; set; } = [];
}

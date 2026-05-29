using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class DietMealOption : EntityBase
{
    public long DietMealId { get; set; }
    public string FoodName { get; set; }
    public string Quantity { get; set; }
    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public string? Ingredients { get; set; }
    public string? PreparationMethod { get; set; }

    public DietMeal DietMeal { get; set; }
    public ICollection<DietMealPhoto> Photos { get; set; } = [];
}

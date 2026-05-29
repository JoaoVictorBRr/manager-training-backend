using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.DietDto;

public class CreateDietDto
{
    public string Name { get; set; } = "Plano Alimentar";
    public List<CreateDietMealDto> Meals { get; set; } = [];
}

public class CreateDietMealDto
{
    public MealType MealType { get; set; }
    public List<CreateDietMealOptionDto> Options { get; set; } = [];
}

public class CreateDietMealOptionDto
{
    public string FoodName { get; set; }
    public string Quantity { get; set; }
    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public string? Ingredients { get; set; }
    public string? PreparationMethod { get; set; }
}

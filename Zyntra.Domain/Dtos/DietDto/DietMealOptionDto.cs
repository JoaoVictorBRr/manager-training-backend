namespace Zyntra.Domain.Dtos.DietDto;

public class DietMealOptionDto
{
    public long Id { get; set; }
    public string FoodName { get; set; }
    public string Quantity { get; set; }
    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public string? Ingredients { get; set; }
    public string? PreparationMethod { get; set; }
    public List<string> PhotoUrls { get; set; } = [];
}

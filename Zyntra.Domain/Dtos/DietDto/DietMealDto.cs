using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.DietDto;

public class DietMealDto
{
    public long Id { get; set; }
    public MealType MealType { get; set; }
    public string MealTypeName { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<DietMealOptionDto> Options { get; set; } = [];
}

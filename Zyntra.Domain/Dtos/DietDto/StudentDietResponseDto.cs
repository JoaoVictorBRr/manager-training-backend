namespace Zyntra.Domain.Dtos.DietDto;

public class StudentDietResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public List<DietMealDto> Meals { get; set; } = [];
    public int TotalCalories { get; set; }
    public int ConsumedCalories { get; set; }
}

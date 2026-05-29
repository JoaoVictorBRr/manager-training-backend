namespace Zyntra.Domain.Dtos.HydrationDto;

public class HydrationLogDto
{
    public long Id { get; set; }
    public DateTime LogDate { get; set; }
    public decimal AmountMl { get; set; }
}

public class AddHydrationDto
{
    public decimal AmountMl { get; set; }
}

public class HydrationSummaryDto
{
    public decimal TotalMl { get; set; }
    public decimal GoalMl { get; set; }
    public decimal PercentageAchieved { get; set; }
    public List<HydrationLogDto> Logs { get; set; } = [];
}

namespace Zyntra.Domain.Dtos.PhysicalAssessmentDto;

public class LatestAssessmentDto
{
    public DateTime? AssessmentDate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Bmi { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public string? Measurements { get; set; }
}

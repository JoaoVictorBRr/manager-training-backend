namespace Zyntra.Domain.Dtos.PhysicalAssessmentDto;

public class PhysicalAssessmentCreateDto
{
    public long StudentId { get; set; }
    public DateTime AssessmentDate { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public string Measurements { get; set; }
    public string Notes { get; set; }
}

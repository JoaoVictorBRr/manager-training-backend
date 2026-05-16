namespace Zyntra.Domain.Dtos.PhysicalAssessmentDto;

public class PhysicalAssessmentResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public DateTime AssessmentDate { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Bmi { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public string Measurements { get; set; }
    public string Notes { get; set; }
}

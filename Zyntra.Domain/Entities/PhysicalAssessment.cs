using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class PhysicalAssessment : EntityBase
{
    public long StudentId { get; set; }
    public DateTime AssessmentDate { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Bmi { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public string Measurements { get; set; }
    public string Notes { get; set; }

    public Student Student { get; set; }
}

namespace Zyntra.Domain.Dtos.StudentDto;

public class AddBodyMeasurementDto
{
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public string? Notes { get; set; }
    public string? Measurements { get; set; }
}

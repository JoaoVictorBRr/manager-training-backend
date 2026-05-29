namespace Zyntra.Domain.Dtos.StudentDto;

public class UpdateProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string CellphoneNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}

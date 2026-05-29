namespace Zyntra.Domain.Dtos.StudentDto;

public class StudentCreateDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string? CellphoneNumber { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
}

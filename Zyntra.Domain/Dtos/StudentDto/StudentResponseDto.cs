using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.StudentDto;

public class StudentResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public string CellphoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime? LastAccessDate { get; set; }
    public Situation Situation { get; set; }
}

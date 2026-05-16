using Zyntra.Domain.Enum;
using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class User : EntityBase
{
    public string Name { get; set; }
    public string CellphoneNumber { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public Role Role { get; set; }
    public string Token { get; set; }
    public DateTime? ExpirationDateToken { get; set; }
}

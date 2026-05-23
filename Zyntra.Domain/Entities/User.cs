using Zyntra.Domain.Enum;
using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class User : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string CellphoneNumber { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? ExpirationDateToken { get; set; }
}

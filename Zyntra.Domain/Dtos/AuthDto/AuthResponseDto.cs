using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.AuthDto;

public class AuthResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDateToken { get; set; }
}

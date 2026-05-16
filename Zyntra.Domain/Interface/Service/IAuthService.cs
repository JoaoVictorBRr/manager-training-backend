using Zyntra.Domain.Dtos.AuthDto;
using Zyntra.Domain.Dtos.StudentDto;

namespace Zyntra.Domain.Interface.Service;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(AuthLoginDto dto);
    Task<AuthResponseDto> RegisterAsync(StudentCreateDto dto);
    Task ForgotPasswordAsync(ForgotPasswordDto dto);
    Task ResetPasswordAsync(ResetPasswordDto dto);
}

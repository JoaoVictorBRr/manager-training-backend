using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zyntra.Domain.Dtos.AuthDto;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Shared.Helpers;

namespace Zyntra.Service.Service;

public class AuthService(
    IUserRepository userRepository,
    IStudentRepository studentRepository,
    IConfiguration configuration) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IStudentRepository _studentRepository = studentRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto)
    {
        var user = await _userRepository.GetAsync(u =>
            (u.Email == dto.EmailOrCpf || u.Cpf == dto.EmailOrCpf) && u.Situation == Situation.Active);

        if (user == null)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var isValid = PasswordHasher.VerifyPassword(dto.Password, user.Password, user.Salt);
        if (!isValid)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        user.Token = token;
        user.ExpirationDateToken = expiration;
        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpirationDateToken = expiration
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(StudentCreateDto dto)
    {
        var existing = await _userRepository.GetAsync(u => u.Email == dto.Email || u.Cpf == dto.Cpf);
        if (existing != null)
            throw new InvalidOperationException("E-mail ou CPF já cadastrado.");

        var (hash, salt, _) = PasswordHasher.HashPassword(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Cpf = dto.Cpf,
            CellphoneNumber = dto.CellphoneNumber ?? string.Empty,
            Password = hash,
            Salt = salt,
            Role = Role.Student
        };

        await _userRepository.AddAsync(user);

        var student = new Student
        {
            UserId = user.Id,
            BirthDate = dto.BirthDate,
            PaymentStatus = PaymentStatus.Pending.ToString()
        };

        await _studentRepository.AddAsync(student);

        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24"));

        user.Token = token;
        user.ExpirationDateToken = expiration;
        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpirationDateToken = expiration
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userRepository.GetAsync(u => u.Email == dto.Email && u.Situation == Situation.Active);
        if (user == null) return;

        var resetToken = Guid.NewGuid().ToString("N");
        user.Token = resetToken;
        user.ExpirationDateToken = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userRepository.GetAsync(u =>
            u.Token == dto.Token && u.ExpirationDateToken > DateTime.UtcNow);

        if (user == null)
            throw new InvalidOperationException("Token inválido ou expirado.");

        var (hash, salt, _) = PasswordHasher.HashPassword(dto.NewPassword);
        user.Password = hash;
        user.Salt = salt;
        user.Token = null;
        user.ExpirationDateToken = null;
        await _userRepository.UpdateAsync(user);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("UserId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

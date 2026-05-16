using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public long GetCurrentUserId()
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return long.TryParse(claim?.Value, out var id) ? id : 0;
    }

    public string GetCurrentUserName() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public string GetCurrentUserRole() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
}

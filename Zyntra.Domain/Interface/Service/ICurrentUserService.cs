namespace Zyntra.Domain.Interface.Service;

public interface ICurrentUserService
{
    long GetCurrentUserId();
    string GetCurrentUserName();
    string GetCurrentUserRole();
}

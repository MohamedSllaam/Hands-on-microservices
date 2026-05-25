using System.Security.Claims;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        string? GetUserName();
        string? GetEmail();
        List<string> GetRoles();
        bool IsAuthenticated();
        bool IsInRole(string role);
        ClaimsPrincipal? GetUser();
    }
}

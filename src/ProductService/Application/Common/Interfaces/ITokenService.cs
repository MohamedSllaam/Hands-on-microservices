using Domain.Entities;
using System.Security.Claims;

namespace Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateJWTAsync(User user, IList<string>? roles = null);
       ClaimsPrincipal? ValidateToken(string token);
        bool IsTokenExpired(string token);
    }
}

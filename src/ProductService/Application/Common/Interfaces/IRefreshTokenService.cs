using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken();
        Task<bool> StoreRefreshTokenAsync(User user, string refreshToken);
        Task<bool> ValidateRefreshTokenAsync(User user, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(User user);
        Task<bool> RotateRefreshTokenAsync(User user, string oldRefreshToken);
        Task RevokeAllUserTokensAsync(User user);
    }
}

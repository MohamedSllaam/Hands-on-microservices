

namespace Application.Features.Auth.Common;

public class RefreshTokenResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? Message { get; set; }

    public static RefreshTokenResult Success(string accessToken, string refreshToken, DateTime expiresAt) =>
        new()
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };

    public static RefreshTokenResult Failure(string message) =>
        new()
        {
            Succeeded = false,
            Message = message
        };
}

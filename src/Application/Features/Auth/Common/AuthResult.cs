

namespace Application.Features.Auth.Common;


public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string>? Errors { get; set; }

    public static AuthResult Success(string accessToken, string refreshToken, DateTime expiresAt,
        string userName, string email, string firstName, string lastName) =>
        new()
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

  
}

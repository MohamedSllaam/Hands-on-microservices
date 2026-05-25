using Domain.Entities;
// API/Services/JWTService.cs
// API/Services/JWTService.cs

namespace Infrastructure.Services.Authentication;

using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
// API/Services/Authentication/JWTService.cs (Updated)
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


public class JWTService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _jwtKey;
    private readonly ILogger<JWTService> _logger;

    public JWTService(
        IOptions<JwtSettings> jwtSettings,
        ILogger<JWTService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
    }

    public async Task<string> CreateJWTAsync(User user, IList<string>? roles = null)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            // Add roles if provided, otherwise fetch them
            var userRoles = roles ?? new List<string>();
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpiresInDays),
                SigningCredentials = credentials,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating JWT for user {UserId}", user.Id);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtKey,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;
            return expiration <= DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
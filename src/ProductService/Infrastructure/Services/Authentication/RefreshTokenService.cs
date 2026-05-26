namespace Infrastructure.Services.Authentication;

using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;


public class RefreshTokenService: IRefreshTokenService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenService(
        UserManager<User> userManager,
        IOptions<JwtSettings> jwtSettings,
        ILogger<RefreshTokenService> logger)
    {
        _userManager = userManager;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> StoreRefreshTokenAsync(User user, string refreshToken)
    {
        try
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to store refresh token for user {UserId}: {Errors}",
                    user.Id, string.Join(", ", result.Errors));
                return false;
            }

            _logger.LogInformation("Refresh token stored for user {UserId}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing refresh token for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(User user, string refreshToken)
    {
        if (user == null || string.IsNullOrEmpty(refreshToken))
            return false;

        if (user.RefreshToken != refreshToken)
        {
            _logger.LogWarning("Invalid refresh token attempt for user {UserId}", user.Id);
            return false;
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("Expired refresh token attempt for user {UserId}", user.Id);
            return false;
        }

        return  true;
    }

    public async Task<bool> RevokeRefreshTokenAsync(User user)
    {
        try
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to revoke refresh token for user {UserId}", user.Id);
                return false;
            }

            _logger.LogInformation("Refresh token revoked for user {UserId}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> RotateRefreshTokenAsync(User user, string oldRefreshToken)
    {
        if (!await ValidateRefreshTokenAsync(user, oldRefreshToken))
            return false;

        var newRefreshToken = GenerateRefreshToken();
        return await StoreRefreshTokenAsync(user, newRefreshToken);
    }

    public async Task RevokeAllUserTokensAsync(User user)
    {
        try
        {
            await _userManager.UpdateSecurityStampAsync(user);
            await RevokeRefreshTokenAsync(user);
            _logger.LogInformation("All tokens revoked for user {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user {UserId}", user.Id);
        }
    }
}
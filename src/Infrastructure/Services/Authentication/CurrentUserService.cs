namespace Infrastructure.Services.Authentication;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

 

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }

        return userId;
    }

    public string? GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Name)?.Value;
    }

    public string? GetEmail()
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;
    }

    public List<string> GetRoles()
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public ClaimsPrincipal? GetUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}
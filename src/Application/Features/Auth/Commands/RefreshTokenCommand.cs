
using System.IdentityModel.Tokens.Jwt;

namespace Application.Features.Auth.Commands;

using System.IdentityModel.Tokens.Jwt;
using Shared.Exceptions;

public class RefreshTokenCommand : ICommand<RefreshTokenResult>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate expired access token
        var principal = _tokenService.ValidateToken(request.AccessToken);
        if (principal == null)
            throw new UnauthorizedAccessException("Invalid or expired access token");

        // Extract user ID from token
        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid token: User ID not found");

        // Get user
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with ID {userId} not found");

        // Validate refresh token
        var isValidRefreshToken = await _refreshTokenService.ValidateRefreshTokenAsync(user, request.RefreshToken);
        if (!isValidRefreshToken)
            throw new UnauthorizedAccessException("Invalid or expired refresh token. Please login again.");

        // Generate new tokens
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = await _tokenService.CreateJWTAsync(user, roles);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.StoreRefreshTokenAsync(user, newRefreshToken);

        return RefreshTokenResult.Success(newAccessToken, newRefreshToken, DateTime.UtcNow.AddDays(1));
    }
}
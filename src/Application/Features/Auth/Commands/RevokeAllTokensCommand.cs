
namespace Application.Features.Auth.Commands;
using Domain.Entities;
// MyApp.Application/Features/Auth/Commands/RevokeAllTokensCommand.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.CQRS;

public class RevokeAllTokensCommand : ICommand<bool>
{
    public string UserId { get; set; } = string.Empty;
}

public class RevokeAllTokensCommandHandler : ICommandHandler<RevokeAllTokensCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;

    public RevokeAllTokensCommandHandler(
        UserManager<User> userManager,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<bool> Handle(RevokeAllTokensCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return false;

        await _refreshTokenService.RevokeAllUserTokensAsync(user);
        return true;
    }
}
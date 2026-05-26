
namespace Application.Features.Auth.Commands;


using Application.Common.Interfaces;
using Domain.Entities;
// MyApp.Application/Features/Auth/Commands/LogoutCommand.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.CQRS;

public class LogoutCommand : ICommand<bool>
{
    public string UserId { get; set; } = string.Empty;
}

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(
        UserManager<User> userManager,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return false;

        return await _refreshTokenService.RevokeRefreshTokenAsync(user);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Common.Interfaces;
using Application.Features.Auth.Commands;
using Application.Features.Auth.Queries; 

namespace API.Controllers; // API/Controllers/AuthController.cs


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return Unauthorized(result.Errors);

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return Unauthorized(result.Message);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = _currentUserService.GetUserId();
        if (userId == null)
            return BadRequest();

        var result = await _mediator.Send(new LogoutCommand { UserId = userId });

        return result ? Ok(new { message = "Logged out successfully" }) : BadRequest();
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var userId = _currentUserService.GetUserId();
        if (userId == null)
            return BadRequest();

        var result = await _mediator.Send(new RevokeAllTokensCommand { UserId = userId });

        return result ? Ok(new { message = "All tokens revoked successfully" }) : BadRequest();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetCurrentUserQuery { UserId = userId });

        return result != null ? Ok(result) : NotFound();
    }
}
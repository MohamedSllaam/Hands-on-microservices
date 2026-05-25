
namespace Application.Features.Auth.Queries;

public class GetCurrentUserQuery : IQuery<CurrentUserResult>, IRequest<CurrentUserResult>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, CurrentUserResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        return new CurrentUserResult
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            Roles = _currentUserService.GetRoles()
        };
    }
}
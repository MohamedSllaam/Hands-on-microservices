namespace Application.Features.Auth.Commands;

using Microsoft.AspNetCore.Identity;

public class RegisterCommand : ICommand<AuthResult>
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");
    }
}

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResult>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly RoleManager<Role> _roleManager;

    public RegisterCommandHandler(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new BadRequestException($"User with email '{request.Email}' already exists");

        // Check if username exists
        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
            throw new BadRequestException($"Username '{request.UserName}' is already taken");

        // Create user
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException($"Failed to create user: {errors}");
        }

        // Ensure default role exists
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            var roleResult = await _roleManager.CreateAsync(new Role { Name = "User" });
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create default role: {roleErrors}");
            }
        }

        // Add user to role
        var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!addToRoleResult.Succeeded)
        {
            var roleErrors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign user to role: {roleErrors}");
        }

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.CreateJWTAsync(user, roles);
        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.StoreRefreshTokenAsync(user, refreshToken);

        return AuthResult.Success(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddDays(1),
            user.UserName!,
            user.Email!,
            user.FirstName!,
            user.LastName!
        );
    }
}
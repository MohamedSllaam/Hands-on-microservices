using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServicesServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<User>(options =>
        {
            // password configuration
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;

            // for email confirmation
            options.SignIn.RequireConfirmedEmail = false;
        })
.AddRoles<Role>()  // ← Changed: Added <int>
.AddRoleManager<RoleManager<Role>>()  // ← Changed: Added <int>
.AddEntityFrameworkStores<MyAppDbContext>()
.AddSignInManager<SignInManager<User>>()
.AddUserManager<UserManager<User>>()
.AddDefaultTokenProviders(); // be able to create tokens for email confirmation
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             // validate the token based on the key we have provided inside appsettings.development.json JWT:Key
             ValidateIssuerSigningKey = true,
             // the issuer singning key based on JWT:Key
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!)),
             // the issuer which in here is the api project url we are using
             ValidIssuer = config["JWT:Issuer"],
             // validate the issuer (who ever is issuing the JWT)
             ValidateIssuer = true,
             // don't validate audience (Vue js side)
             ValidateAudience = false
         };
     });

        return services;
    }
}


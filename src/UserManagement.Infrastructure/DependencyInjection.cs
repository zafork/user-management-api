using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Application.Ports.Out;
using UserManagement.Domain.Constants;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Persistence.Repositories;
using UserManagement.Infrastructure.Security;

namespace UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddSecurity(services, configuration);
        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection'.");

            options.UseOracle(connectionString, oracle => oracle
                .UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion23)
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddSecurity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = AppClaimTypes.Name,
                    RoleClaimType = AppClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in PermissionCodes.All)
            {
                options.AddPolicy(permission, policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim(AppClaimTypes.Permission, permission));
            }
        });
    }
}
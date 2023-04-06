using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Options;
using Authentication.Infrastructure.Persistence;
using Authentication.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    private const string UseInMemoryDatabaseKey = "UseInMemoryDatabase";
    private const string PostgresKey = "Postgres";
    private const string InMemAuthenticationDbName = "Authentication-DB";

    public static IServiceCollection AddAuthenticationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigureDatabase(services, configuration);

        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddOptions<JwtGeneratorOptions>()
            .Bind(configuration.GetSection(JwtGeneratorOptions.SectionName))
            .Validate(options =>
                !(string.IsNullOrWhiteSpace(options.SecretKey) && string.IsNullOrWhiteSpace(options.Audience) &&
                  string.IsNullOrWhiteSpace(options.Issuer) && options.Lifetime > 0));

        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IApplicationContext, ApplicationContext>(builder => {
            if (configuration.GetSection(UseInMemoryDatabaseKey).Get<bool>())
                builder.UseInMemoryDatabase(InMemAuthenticationDbName);
            else builder.UseNpgsql(configuration.GetConnectionString(PostgresKey));
        });
    }
}

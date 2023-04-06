using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Services;
using Authentication.UnitTests.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.UnitTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<IApplicationContext, TestContext>(builder =>
            builder.UseInMemoryDatabase("Test-DB"));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }
}

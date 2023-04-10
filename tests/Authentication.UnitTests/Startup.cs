using System.Reflection;
using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Extensions;
using Authentication.UnitTests.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Authentication.UnitTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", false)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
            .Build();

        services.AddAuthenticationInfrastructure(configuration);

        services.RemoveAll(typeof(IApplicationContext));
        services.AddDbContext<IApplicationContext, TestContext>(builder =>
            builder.UseInMemoryDatabase("Test-DB"));
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Authentication.Infrastructure.Persistence;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        string connection = args[0];
        
        optionsBuilder.UseMySql(connection, ServerVersion.AutoDetect(connection));

        return new ApplicationContext(optionsBuilder.Options);
    }
}

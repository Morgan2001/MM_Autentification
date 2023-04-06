using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Persistence;

public class ApplicationContext : DbContext, IApplicationContext
{
    public DbSet<GuestAccount> GuestAccounts { get; set; } = null!;
    public DbSet<ProtectedAccount> ProtectedAccounts { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GuestAccountConfiguration).Assembly);
    }
}

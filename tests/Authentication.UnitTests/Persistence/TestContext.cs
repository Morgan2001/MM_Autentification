using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Authentication.UnitTests.Persistence;

public class TestContext : DbContext, IApplicationContext
{
    public DbSet<GuestAccount> GuestAccounts { get; set; } = null!;
    public DbSet<ProtectedAccount> ProtectedAccounts { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public TestContext(DbContextOptions<TestContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GuestAccountConfiguration).Assembly);
    }
}

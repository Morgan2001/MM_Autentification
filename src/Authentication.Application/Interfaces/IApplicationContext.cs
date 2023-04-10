using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Application.Interfaces;

public interface IApplicationContext
{
    DbSet<GuestAccount> GuestAccounts { get; }
    DbSet<ProtectedAccount> ProtectedAccounts { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<VerificationCode> VerificationCodes { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

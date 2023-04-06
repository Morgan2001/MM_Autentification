using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Application.Interfaces;

public interface IApplicationContext
{
    DbSet<GuestAccount> GuestAccounts { get; }
    DbSet<ProtectedAccount> ProtectedAccounts { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using FluentResults;

namespace Authentication.Infrastructure.Services;

public class AccountsService : IAccountsService
{
    private readonly IApplicationContext _context;

    public AccountsService(IApplicationContext context)
    {
        _context = context;
    }

    public Task<Result<GuestAccount>> RegisterGuestAccount(string deviceId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProtectedAccount>> ProtectAccount(string deviceId, string email, string password)
    {
        throw new NotImplementedException();
    }
}

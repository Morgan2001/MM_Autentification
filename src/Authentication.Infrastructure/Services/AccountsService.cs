using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Services;

public class AccountsService : IAccountsService
{
    private readonly IApplicationContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AccountsService(IApplicationContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<GuestAccount>> RegisterGuestAccount(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return Result.Fail<GuestAccount>("DeviceId can't be null or empty");

        var foundAccount = await _context.GuestAccounts.FirstOrDefaultAsync(x => x.DeviceId == deviceId);
        if (foundAccount is not null)
            return Result.Fail<GuestAccount>("Account with this DeviceId already exist");

        var account = new GuestAccount(deviceId);
        var result = (await _context.GuestAccounts.AddAsync(account)).Entity;
        await _context.SaveChangesAsync();

        return Result.Ok(result);
    }

    public async Task<Result<ProtectedAccount>> ProtectAccount(string deviceId, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return Result.Fail<ProtectedAccount>("DeviceId can't be empty");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.Fail<ProtectedAccount>("Email or password can't be empty");

        var foundGuestAccount = await _context.GuestAccounts.FirstOrDefaultAsync(x => x.DeviceId == deviceId);
        if (foundGuestAccount is null)
            return Result.Fail<ProtectedAccount>("Account with this DeviceId not found");

        var foundProtectedAccount = await _context.ProtectedAccounts.FirstOrDefaultAsync(x => x.Email == email);
        if (foundProtectedAccount is not null)
            return Result.Fail<ProtectedAccount>("Email already registered");

        string hash = _passwordHasher.HashPassword(password, out byte[] salt);

        var protectedAccount = new ProtectedAccount(email, hash, Convert.ToBase64String(salt), foundGuestAccount);
        var result = (await _context.ProtectedAccounts.AddAsync(protectedAccount)).Entity;
        await _context.SaveChangesAsync();
        
        return Result.Ok(result);
    }
}

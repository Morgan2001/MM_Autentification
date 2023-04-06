using Authentication.Application.Interfaces;
using Authentication.Domain.Common;
using Authentication.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApplicationContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtGenerator _jwtGenerator;

    public AuthenticationService(IApplicationContext context, IPasswordHasher passwordHasher,
        IJwtGenerator jwtGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<Result<JwtTokens>> Authenticate(string email, string password)
    {
        var account = await _context.ProtectedAccounts
            .Include(x => x.GuestAccount)
            .FirstOrDefaultAsync(x => x.Email == email);
        if (account is null) return Result.Fail<JwtTokens>("Account not found");

        if (!_passwordHasher.VerifyPassword(password, account.PasswordHash,
                Convert.FromBase64String(account.PasswordSalt)))
            return Result.Fail<JwtTokens>("Wrong credentials");

        string deviceId = account.GuestAccount.DeviceId;
        string accessToken = _jwtGenerator.GenerateAccessToken(deviceId);
        string refreshToken = _jwtGenerator.GenerateRefreshToken(deviceId);

        await _context.RefreshTokens.AddAsync(new RefreshToken(refreshToken, deviceId));
        await _context.SaveChangesAsync();

        return Result.Ok(new JwtTokens(accessToken, refreshToken));
    }

    public async Task<Result<JwtTokens>> RefreshToken(string refreshToken)
    {
        var foundToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
        if (foundToken is null) return Result.Fail<JwtTokens>("RefreshToken not found");

        string accessToken = _jwtGenerator.GenerateAccessToken(foundToken.DeviceId);
        string newRefreshToken = _jwtGenerator.GenerateRefreshToken(foundToken.DeviceId);

        await _context.RefreshTokens.AddAsync(new RefreshToken(newRefreshToken, foundToken.DeviceId));
        _context.RefreshTokens.Remove(foundToken);

        await _context.SaveChangesAsync();
        var tokens = new JwtTokens(accessToken, newRefreshToken);

        return Result.Ok(tokens);
    }
}

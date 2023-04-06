using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Services;
using FluentAssertions;

namespace Authentication.UnitTests.Services;

public class AuthenticationServiceTests
{
    private readonly IApplicationContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AuthenticationService _authenticationService;
    private static string DeviceId => Guid.NewGuid().ToString();
    private static string Email => $"test-{Guid.NewGuid()}@test.com";
    private const string Password = "my-password";

    public AuthenticationServiceTests(IApplicationContext context, IPasswordHasher passwordHasher,
        IJwtGenerator jwtGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;

        _authenticationService = new AuthenticationService(context, passwordHasher, jwtGenerator);
    }

    private async Task<ProtectedAccount> CreateProtectedAccount(string deviceId, string email, string password)
    {
        var guestAccount = new GuestAccount(deviceId);
        await _context.GuestAccounts.AddAsync(guestAccount);

        string hash = _passwordHasher.HashPassword(Password, out byte[] salt);

        var protectedAccount = new ProtectedAccount(email, hash, Convert.ToBase64String(salt), guestAccount);
        await _context.ProtectedAccounts.AddAsync(protectedAccount);
        await _context.SaveChangesAsync();

        return protectedAccount;
    }

    [Fact]
    private async Task Authenticate_WithValidCredentials_ShouldBeSuccessful()
    {
        string deviceId = DeviceId;
        string email = Email;

        await CreateProtectedAccount(deviceId, email, Password);

        var result = await _authenticationService.Authenticate(email, Password);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    private async Task Authenticate_WithInvalidCredentials_ShouldBeFailed()
    {
        string deviceId = DeviceId;
        string email = Email;

        await CreateProtectedAccount(deviceId, email, Password);

        var result = await _authenticationService.Authenticate(email, "invalid-password");

        result.IsFailed.Should().BeTrue();
    }
    
    [Fact]
    private async Task Authenticate_WithUnregisteredAccount_ShouldBeFailed()
    {
        string email = Email;

        var result = await _authenticationService.Authenticate(email, Password);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task RefreshToken_WithValidToken_ShouldBeSuccessful()
    {
        string deviceId = DeviceId;
        string email = Email;

        await CreateProtectedAccount(deviceId, email, Password);

        var authenticateResult = await _authenticationService.Authenticate(email, Password);

        var tokens = await _authenticationService.RefreshToken(authenticateResult.Value.RefreshToken);

        tokens.IsSuccess.Should().BeTrue();
        tokens.Value.Should().NotBeNull();
        tokens.Value.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    private async Task RefreshToken_WithInvalidToken_ShouldBeFailed()
    {
        var tokens = await _authenticationService.RefreshToken("invalid-token");

        tokens.IsFailed.Should().BeTrue();
    }
}

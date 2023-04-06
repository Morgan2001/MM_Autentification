using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Services;
using FluentAssertions;

namespace Authentication.UnitTests.Services;

public class AccountsServiceTests
{
    private readonly AccountsService _accountsService;
    private static string DeviceId => Guid.NewGuid().ToString();
    private static string Email => $"test-{Guid.NewGuid()}@test.com";
    private const string Password = "test-password";

    public AccountsServiceTests(IApplicationContext context, IPasswordHasher passwordHasher)
    {
        _accountsService = new AccountsService(context, passwordHasher);
    }

    [Fact]
    private async Task RegisterGuestAccount_WithValidDeviceId_ShouldBeSuccessful()
    {
        string deviceId = DeviceId;
        var result = await _accountsService.RegisterGuestAccount(deviceId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.ValueOrDefault.DeviceId.Should().Be(deviceId);
    }

    [Fact]
    private async Task RegisterGuestAccount_WithInvalidDeviceId_ShouldBeFailed()
    {
        string deviceId = string.Empty;
        var result = await _accountsService.RegisterGuestAccount(deviceId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task RegisterGuestAccount_WithSameDeviceId_ShouldBeFailed()
    {
        string deviceId = DeviceId;
        await _accountsService.RegisterGuestAccount(deviceId);
        var result = await _accountsService.RegisterGuestAccount(deviceId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithRegisteredAccount_ShouldBeSuccessful()
    {
        string deviceId = DeviceId;
        var registrationResult = await _accountsService.RegisterGuestAccount(deviceId);

        string email = Email;
        var protectResult = await _accountsService.ProtectAccount(deviceId, email, Password);

        protectResult.IsSuccess.Should().BeTrue();
        protectResult.Value.Should().NotBeNull();
        protectResult.ValueOrDefault.Email.Should().Be(email);
        protectResult.ValueOrDefault.GuestAccount.Should().Be(registrationResult.ValueOrDefault);
    }

    [Fact]
    private async Task ProtectAccount_WithUnregisteredAccount_ShouldBeFailed()
    {
        string deviceId = DeviceId;
        string email = Email;
        var protectResult = await _accountsService.ProtectAccount(deviceId, email, Password);

        protectResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithAlreadyRegisteredAccount_ShouldBeFailed()
    {
        string deviceId = DeviceId;
        await _accountsService.RegisterGuestAccount(deviceId);

        string email = Email;
        await _accountsService.ProtectAccount(deviceId, email, Password);

        var protectResult = await _accountsService.ProtectAccount(deviceId, email, Password);

        protectResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithInvalidCredentials_ShouldBeFailed()
    {
        string deviceId = string.Empty;
        string email = string.Empty;
        string password = string.Empty;
        
        var resultWithInvalidCredentials =
            await _accountsService.ProtectAccount(deviceId, email, password);

        resultWithInvalidCredentials.IsFailed.Should().BeTrue();
    }
}

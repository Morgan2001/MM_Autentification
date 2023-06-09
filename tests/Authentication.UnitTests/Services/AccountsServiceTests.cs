﻿using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Services;
using Authentication.Tests.Shared.Factories;
using FluentAssertions;

namespace Authentication.UnitTests.Services;

public class AccountsServiceTests
{
    private readonly AccountsService _accountsService;

    public AccountsServiceTests(IApplicationContext context, IPasswordHasher passwordHasher)
    {
        _accountsService = new AccountsService(context, passwordHasher);
    }

    [Fact]
    private async Task RegisterGuestAccount_WithValidDeviceId_ShouldBeSuccessful()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
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
        string deviceId = FakeDataFactory.GenerateDeviceId();
        await _accountsService.RegisterGuestAccount(deviceId);
        var result = await _accountsService.RegisterGuestAccount(deviceId);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithRegisteredAccount_ShouldBeSuccessful()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        var registrationResult = await _accountsService.RegisterGuestAccount(deviceId);

        string email = FakeDataFactory.GenerateEmail();
        var protectResult = await _accountsService.ProtectAccount(deviceId, email, FakeDataFactory.GeneratePassword());

        protectResult.IsSuccess.Should().BeTrue();
        protectResult.Value.Should().NotBeNull();
        protectResult.ValueOrDefault.Email.Should().Be(email);
        protectResult.ValueOrDefault.GuestAccount.Should().Be(registrationResult.ValueOrDefault);
    }

    [Fact]
    private async Task ProtectAccount_WithUnregisteredAccount_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = FakeDataFactory.GenerateEmail();
        var protectResult = await _accountsService.ProtectAccount(deviceId, email, FakeDataFactory.GeneratePassword());

        protectResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithAlreadyRegisteredAccount_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        await _accountsService.RegisterGuestAccount(deviceId);

        string email = FakeDataFactory.GenerateEmail();
        await _accountsService.ProtectAccount(deviceId, email, FakeDataFactory.GeneratePassword());

        var protectResult = await _accountsService.ProtectAccount(deviceId, email, FakeDataFactory.GeneratePassword());

        protectResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithInvalidDeviceId_ShouldBeFailed()
    {
        string deviceId = string.Empty;
        string email = FakeDataFactory.GenerateEmail();

        var resultWithInvalidCredentials =
            await _accountsService.ProtectAccount(deviceId, email, FakeDataFactory.GeneratePassword());

        resultWithInvalidCredentials.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ProtectAccount_WithInvalidCredentials_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = string.Empty;
        string password = string.Empty;

        var resultWithInvalidCredentials =
            await _accountsService.ProtectAccount(deviceId, email, password);

        resultWithInvalidCredentials.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ChangePassword_WithValidOldPassword_ShouldBeSuccessful()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = FakeDataFactory.GenerateEmail();
        string password = FakeDataFactory.GeneratePassword();

        var guestAccountResult = await _accountsService.RegisterGuestAccount(deviceId);
        await _accountsService.ProtectAccount(guestAccountResult.Value.DeviceId, email, password);

        var changePasswordResult =
            await _accountsService.ChangePassword(email, password, "New-Password");

        changePasswordResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    private async Task ChangePassword_WithInvalidOldPassword_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = FakeDataFactory.GenerateEmail();

        var guestAccountResult = await _accountsService.RegisterGuestAccount(deviceId);
        await _accountsService.ProtectAccount(guestAccountResult.Value.DeviceId, email, FakeDataFactory.GeneratePassword());

        var changePasswordResult = await _accountsService.ChangePassword(deviceId, "Invalid-Password", "New-Password");

        changePasswordResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task ChangePassword_WithUnregisteredAccount_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();

        var changePasswordResult = await _accountsService.ChangePassword(deviceId, "Invalid-Password", "New-Password");

        changePasswordResult.IsFailed.Should().BeTrue();
    }
}

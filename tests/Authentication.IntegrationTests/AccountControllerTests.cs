using System.Net;
using System.Net.Http.Json;
using Authentication.Api.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Authentication.IntegrationTests;

public class AccountControllerTests : WebApplicationFactory<Program>
{
    private static string DeviceId => Guid.NewGuid().ToString();
    private static string Email => $"test-{Guid.NewGuid()}@test.com";
    private const string Password = "my-password";

    [Fact]
    private async Task RegisterGuestAccount_WithValidDeviceId_ShouldReturn200()
    {
        var client = CreateClient();
        var dto = new CreateGuestAccountDto(DeviceId);
        var result = await client.PostAsJsonAsync("/account/guest", dto);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    private async Task RegisterGuestAccount_WithInvalidDeviceId_ShouldReturn400()
    {
        var client = CreateClient();
        var dto = new CreateGuestAccountDto(string.Empty);
        var result = await client.PostAsJsonAsync("/account/guest", dto);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task RegisterGuestAccount_WithAlreadyTakenDeviceId_ShouldReturn400()
    {
        var client = CreateClient();

        var dto = new CreateGuestAccountDto(DeviceId);
        await client.PostAsJsonAsync("/account/guest", dto);
        var result = await client.PostAsJsonAsync("/account/guest", dto);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task ProtectAccount_WithRegisteredAccount_ShouldReturn200()
    {
        var client = CreateClient();
        string deviceId = DeviceId;
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("/account/guest", guestDto);

        var protectDto = new ProtectAccountDto(deviceId, Email, Password);
        var result = await client.PostAsJsonAsync("/account/protect", protectDto);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    private async Task ProtectAccount_WithUnregisteredAccount_ShouldReturn400()
    {
        var client = CreateClient();
        string deviceId = DeviceId;

        var protectDto = new ProtectAccountDto(deviceId, Email, Password);
        var result = await client.PostAsJsonAsync("/account/protect", protectDto);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task ProtectAccount_WithEmptyAccount_ShouldReturn400()
    {
        var client = CreateClient();

        var protectDto = new ProtectAccountDto(string.Empty, string.Empty, string.Empty);
        var result = await client.PostAsJsonAsync("/account/protect", protectDto);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task ChangePassword_WithRegisteredAccount_ShouldReturn200()
    {
        var client = CreateClient();
        string deviceId = DeviceId;
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("/account/guest", guestDto);

        var protectDto = new ProtectAccountDto(deviceId, Email, Password);
        await client.PostAsJsonAsync("/account/protect", protectDto);

        var changePasswordDto = new ChangePasswordDto(deviceId, Password, "new-password");
        var result = await client.PatchAsJsonAsync("/account", changePasswordDto);
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    private async Task ChangePassword_WithUnregisteredAccount_ShouldReturn400()
    {
        var client = CreateClient();
        string deviceId = DeviceId;

        var changePasswordDto = new ChangePasswordDto(deviceId, Password, "new-password");
        var result = await client.PatchAsJsonAsync("/account", changePasswordDto);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task ChangePassword_WithWrongPassword_ShouldReturn400()
    {
        var client = CreateClient();
        string deviceId = DeviceId;
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("/account/guest", guestDto);

        var protectDto = new ProtectAccountDto(deviceId, Email, Password);
        await client.PostAsJsonAsync("/account/protect", protectDto);

        var changePasswordDto = new ChangePasswordDto(deviceId, "wrong-password", "new-password");
        var result = await client.PatchAsJsonAsync("/account", changePasswordDto);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task ChangePassword_WithEmptyPassword_ShouldReturn400()
    {
        var client = CreateClient();
        string deviceId = DeviceId;
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("/account/guest", guestDto);

        var protectDto = new ProtectAccountDto(deviceId, Email, Password);
        await client.PostAsJsonAsync("/account/protect", protectDto);

        var changePasswordDto = new ChangePasswordDto(deviceId, string.Empty, string.Empty);
        var result = await client.PatchAsJsonAsync("/account", changePasswordDto);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

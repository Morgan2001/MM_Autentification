using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Authentication.Api;
using Authentication.Api.Dto;
using Authentication.Domain.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Authentication.IntegrationTests.Controllers;

public class AuthenticationControllerTests : WebApplicationFactory<Program>
{
    private static string DeviceId => Guid.NewGuid().ToString();
    private static string Email => $"test-{Guid.NewGuid()}@test.com";
    private const string Password = "my-password";
    private static JsonSerializerOptions _jsonWebSerializerOptions = new(JsonSerializerDefaults.Web);

    private async Task CreateProtectedAccount(string deviceId, string email, string password)
    {
        var client = CreateClient();
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("account/guest", guestDto);
        var protectedDto = new ProtectAccountDto(deviceId, email, password);
        await client.PostAsJsonAsync("account/protect", protectedDto);
    }

    [Fact]
    private async Task Authenticate_WithRegisteredAccount_ShouldReturn200()
    {
        var client = CreateClient();

        string deviceId = DeviceId;
        string email = Email;
        await CreateProtectedAccount(deviceId, email, Password);

        var dto = new AuthenticateDto(email, Password);
        var result = await client.PostAsJsonAsync("/authenticate", dto);
        var tokens = JsonSerializer.Deserialize<JwtTokens>(await result.Content.ReadAsStringAsync(), _jsonWebSerializerOptions);
        
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        tokens.Should().NotBeNull();
        tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    private async Task Authenticate_WithUnregisteredAccount_ShouldReturn400()
    {
        var client = CreateClient();

        string email = Email;

        var dto = new AuthenticateDto(email, Password);
        var result = await client.PostAsJsonAsync("/authenticate", dto);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task Authenticate_WithEmptyAccount_ShouldReturn400()
    {
        var client = CreateClient();

        var dto = new AuthenticateDto(string.Empty, string.Empty);
        var result = await client.PostAsJsonAsync("/authenticate", dto);
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    private async Task RefreshTokens_WithValidToken_ShouldReturn200()
    {
        var client = CreateClient();

        string deviceId = DeviceId;
        string email = Email;
        await CreateProtectedAccount(deviceId, email, Password);

        var authenticateDto = new AuthenticateDto(email, Password);
        var authenticationResult = await client.PostAsJsonAsync("/authenticate", authenticateDto);

        var tokens = JsonSerializer.Deserialize<JwtTokens>(await authenticationResult.Content.ReadAsStringAsync(),
            _jsonWebSerializerOptions);
        var refreshDto = new RefreshTokenDto(tokens?.RefreshToken);

        var refreshResult = await client.PostAsJsonAsync("/authenticate/refresh", refreshDto);
        var refreshedTokens = JsonSerializer.Deserialize<JwtTokens>(await refreshResult.Content.ReadAsStringAsync(),
            _jsonWebSerializerOptions);

        refreshedTokens.Should().NotBeNull();
        refreshedTokens.AccessToken.Should().NotBeNullOrWhiteSpace();
        refreshedTokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    private async Task RefreshTokens_WithAlreadyUsedToken_ShouldReturn400()
    {
        var client = CreateClient();

        string deviceId = DeviceId;
        string email = Email;
        await CreateProtectedAccount(deviceId, email, Password);

        var authenticateDto = new AuthenticateDto(email, Password);
        var authenticationResult = await client.PostAsJsonAsync("/authenticate", authenticateDto);

        var tokens = JsonSerializer.Deserialize<JwtTokens>(await authenticationResult.Content.ReadAsStringAsync(),
            _jsonWebSerializerOptions);
        var refreshDto = new RefreshTokenDto(tokens?.RefreshToken);

        await client.PostAsJsonAsync("/authenticate/refresh", refreshDto);
        var secondRefreshResult = await client.PostAsJsonAsync("/authenticate/refresh", refreshDto);

        secondRefreshResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    private async Task RefreshTokens_WithNonExistentToken_ShouldReturn200()
    {
        var client = CreateClient();
        
        var refreshDto = new RefreshTokenDto(Guid.NewGuid().ToString());

        var refreshResult = await client.PostAsJsonAsync("/authenticate/refresh", refreshDto);

        refreshResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    private async Task RefreshTokens_WithEmptyToken_ShouldReturn200()
    {
        var client = CreateClient();
        
        var refreshDto = new RefreshTokenDto(string.Empty);

        var refreshResult = await client.PostAsJsonAsync("/authenticate/refresh", refreshDto);

        refreshResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

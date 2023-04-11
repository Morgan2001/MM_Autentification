using System.Net.Http.Json;
using Authentication.Api.Dto;

namespace Authentication.IntegrationTests.Factories;

public static class ProtectedClientExtensions
{
    public async static Task CreateProtectedAccount(this HttpClient client, string deviceId, string email, string password)
    {
        var guestDto = new CreateGuestAccountDto(deviceId);
        await client.PostAsJsonAsync("account/guest", guestDto);
        var protectedDto = new ProtectAccountDto(deviceId, email, password);
        await client.PostAsJsonAsync("account/protect", protectedDto);
    }
}

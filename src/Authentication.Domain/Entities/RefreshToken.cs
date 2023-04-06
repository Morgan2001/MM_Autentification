namespace Authentication.Domain.Entities;

public record RefreshToken(string Token, string DeviceId)
{
    public long Id { get; }
}

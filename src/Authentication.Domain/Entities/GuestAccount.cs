namespace Authentication.Domain.Entities;

public record GuestAccount(string DeviceId)
{
    public long Id { get; }
}

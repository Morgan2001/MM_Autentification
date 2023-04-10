namespace Authentication.Domain.Entities;

public record ProtectedAccount
{
    private ProtectedAccount() { }

    public ProtectedAccount(string email, string passwordHash, string passwordSalt, GuestAccount guestAccount)
    {
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        GuestAccount = guestAccount;
    }

    public long Id { get; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public GuestAccount GuestAccount { get; set; } = null!;
    public bool IsVerified { get; set; } = false;
}

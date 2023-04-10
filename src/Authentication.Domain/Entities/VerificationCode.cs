namespace Authentication.Domain.Entities;

public record VerificationCode(string Email, string Code)
{
    public long Id { get; }
};

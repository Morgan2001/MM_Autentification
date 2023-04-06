using Authentication.Application.Interfaces;

namespace Authentication.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password, out byte[] salt)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string hash, byte[] salt)
    {
        throw new NotImplementedException();
    }
}
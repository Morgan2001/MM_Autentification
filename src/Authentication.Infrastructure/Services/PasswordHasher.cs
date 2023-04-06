using System.Security.Cryptography;
using System.Text;
using Authentication.Application.Interfaces;

namespace Authentication.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public string HashPassword(string password, out byte[] salt)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException(password);

        salt = RandomNumberGenerator.GetBytes(KeySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            _hashAlgorithm,
            KeySize);

        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash, byte[] salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithm, KeySize);
        return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
    }
}

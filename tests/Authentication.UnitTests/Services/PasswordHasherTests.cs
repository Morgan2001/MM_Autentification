using Authentication.Infrastructure.Services;
using FluentAssertions;

namespace Authentication.UnitTests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();
    private const string ValidPassword = "MySecretPa$$w0rd!";

    [Fact]
    private void HashPassword_WithValidPassword_ShouldBeSuccessful()
    {
        string hash = _passwordHasher.HashPassword(ValidPassword, out byte[] salt);

        hash.Should().NotBeNullOrWhiteSpace();
        salt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    private void HashPassword_WithInvalidPassword_ShouldThrow()
    {
        const string invalidPassword = "";
        var act = () => _passwordHasher.HashPassword(invalidPassword, out byte[] salt);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    private void VerifyPassword_WithRightPassword_ShouldBeSuccessful()
    {
        string hash = _passwordHasher.HashPassword(ValidPassword, out byte[] salt);
        bool result = _passwordHasher.VerifyPassword(ValidPassword, hash, salt);

        result.Should().BeTrue();
    }
    
    [Fact]
    private void VerifyPassword_WithWrongPassword_ShouldBeFailed()
    {
        const string invalidPassword = "";
        string hash = _passwordHasher.HashPassword(ValidPassword, out byte[] salt);
        bool result = _passwordHasher.VerifyPassword(invalidPassword, hash, salt);

        result.Should().BeFalse();
    }
}

using Authentication.Infrastructure.Options;
using Authentication.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Authentication.UnitTests.Services;

public class JwtGeneratorTests
{
    private readonly JwtGenerator _jwtGenerator;

    public JwtGeneratorTests(IOptions<JwtGeneratorOptions> options)
    {
        _jwtGenerator = new JwtGenerator(options);
    }

    [Fact]
    private void GenerateAccessToken_WithValidDeviceId_ShouldNotBeNullOrEmpty()
    {
        string result = _jwtGenerator.GenerateAccessToken(Guid.NewGuid().ToString());

        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    private void GenerateAccessToken_WithInvalidDeviceId_ShouldNotBeNullOrEmpty()
    {
        var act = () => _jwtGenerator.GenerateAccessToken(string.Empty);
        
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    private void GenerateRefreshToken_WithValidDeviceId_ShouldNotBeNullOrEmpty()
    {
        string result = _jwtGenerator.GenerateRefreshToken(Guid.NewGuid().ToString());

        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    private void GenerateRefreshToken_WithInvalidDeviceId_ShouldNotBeNullOrEmpty()
    {
        var act = () => _jwtGenerator.GenerateRefreshToken(string.Empty);
        
        act.Should().Throw<ArgumentException>();
    }
}

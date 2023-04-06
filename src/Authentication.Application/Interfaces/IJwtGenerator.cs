namespace Authentication.Application.Interfaces;

public interface IJwtGenerator
{
    string GenerateAccessToken(string deviceId);
    string GenerateRefreshToken(string deviceId);
}

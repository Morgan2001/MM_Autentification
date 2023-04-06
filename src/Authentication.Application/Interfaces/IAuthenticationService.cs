using Authentication.Domain.Common;
using FluentResults;

namespace Authentication.Application.Interfaces;

public interface IAuthenticationService
{
    Task<Result<JwtTokens>> Authenticate(string email, string password);
    Task<Result<JwtTokens>> RefreshToken(string refreshToken);
}

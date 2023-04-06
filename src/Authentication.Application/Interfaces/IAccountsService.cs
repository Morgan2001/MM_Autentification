using Authentication.Domain.Entities;
using FluentResults;

namespace Authentication.Application.Interfaces;

public interface IAccountsService
{
    Task<Result<GuestAccount>> RegisterGuestAccount(string deviceId);
    Task<Result<ProtectedAccount>> ProtectAccount(string deviceId, string email, string password);
}

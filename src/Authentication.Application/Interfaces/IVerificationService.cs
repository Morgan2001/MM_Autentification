using Authentication.Domain.Entities;
using FluentResults;

namespace Authentication.Application.Interfaces;

public interface IVerificationService
{
    Task<Result<VerificationCode>> SendVerificationCode(string email);
    Task<Result> SubmitVerificationCode(string code);
}

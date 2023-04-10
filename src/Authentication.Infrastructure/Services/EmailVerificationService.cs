using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Options;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Authentication.Infrastructure.Services;

public class EmailVerificationService : IVerificationService
{
    private readonly IApplicationContext _context;
    private readonly EmailOptions _options;

    public EmailVerificationService(IApplicationContext context, IOptions<EmailOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public async Task<Result<VerificationCode>> SendVerificationCode(string email)
    {
        var foundAccount = await _context.ProtectedAccounts.FirstOrDefaultAsync(x => x.Email == email);

        if (foundAccount is null) return Result.Fail<VerificationCode>("Account not found");
        if (foundAccount.IsVerified) return Result.Fail<VerificationCode>("Account already verified");

        var foundCode = await _context.VerificationCodes.FirstOrDefaultAsync(x => x.Email == email);

        if (foundCode is not null) return Result.Fail<VerificationCode>("Verification code already sent");

        var code = new VerificationCode(email, Guid.NewGuid().ToString());
        await _context.VerificationCodes.AddAsync(code);
        await _context.SaveChangesAsync();

        string link = $"{_options.BaseUrl}/verify/{code.Code}";

        var message = new PostmarkMessage
        {
            To = email,
            From = _options.From,
            TrackOpens = true,
            Subject = _options.Subject,
            TextBody = $"Follow the link to verify your account - {link}"
        };

        var client = new PostmarkClient(_options.PostmarkToken);
        var sendResult = await client.SendMessageAsync(message);

        return sendResult.Status == PostmarkStatus.Success
            ? Result.Ok(code)
            : Result.Fail<VerificationCode>(sendResult.Message);
    }

    public async Task<Result> SubmitVerificationCode(string code)
    {
        var foundCode = await _context.VerificationCodes.FirstOrDefaultAsync(x => x.Code == code);

        if (foundCode is null) return Result.Fail("Verification code not found");

        var foundAccount = await _context.ProtectedAccounts.FirstOrDefaultAsync(x => x.Email == foundCode.Email);

        if (foundAccount is null) return Result.Fail("Account not found");
        if (foundAccount.IsVerified) return Result.Fail("Account already verified");

        foundAccount.IsVerified = true;
        _context.VerificationCodes.Remove(foundCode);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }
}

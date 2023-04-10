using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Authentication.UnitTests.Factories;

public static class VerificationServiceMockFactory
{
    public static IVerificationService Create(IApplicationContext context)
    {
        var mock = new Mock<IVerificationService>();

        SetupSendVerificationCode(context, mock);

        SetupSubmitVerificationCode(context, mock);

        return mock.Object;
    }

    private static void SetupSubmitVerificationCode(IApplicationContext context, Mock<IVerificationService> mock)
    {
        mock.Setup(x => x.SubmitVerificationCode(It.IsAny<string>()))
            .Returns(async (string code) => {
                var foundCode = await context.VerificationCodes.FirstOrDefaultAsync(x => x.Code == code);
                if (foundCode is null) return Result.Fail("Code not found");
                var foundAccount = await context.ProtectedAccounts.FirstOrDefaultAsync(x => x.Email == foundCode.Email);
                if (foundAccount is null) return Result.Fail("Account not found");

                foundAccount.IsVerified = true;
                await context.SaveChangesAsync();
                return Result.Ok();
            });
    }

    private static void SetupSendVerificationCode(IApplicationContext context, Mock<IVerificationService> mock)
    {
        mock.Setup(x => x.SendVerificationCode(It.IsAny<string>()))
            .Returns(async (string email) => {
                var code = await context.VerificationCodes.AddAsync(new VerificationCode(email, Guid.NewGuid().ToString()));
                await context.SaveChangesAsync();

                return Result.Ok(code.Entity);
            });
    }
}

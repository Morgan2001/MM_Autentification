using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Options;
using Authentication.Infrastructure.Services;
using Authentication.Tests.Shared.Factories;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Authentication.UnitTests.Services;

public class EmailVerificationServiceTests
{
    private readonly IApplicationContext _context;
    private readonly EmailVerificationService _verificationService;
    private readonly IAccountsService _accountsService;

    public EmailVerificationServiceTests(IApplicationContext context, IOptions<EmailOptions> options,
        IAccountsService accountsService)
    {
        _context = context;
        _accountsService = accountsService;
        _verificationService = new EmailVerificationService(context, options);
    }

    private async Task<ProtectedAccount> CreateProtectedAccount(string deviceId, string email, string password)
    {
        await _accountsService.RegisterGuestAccount(deviceId);
        var protectedAccount = await _accountsService.ProtectAccount(deviceId, email, password);
        return protectedAccount.Value;
    }

    [Fact]
    private async Task SendVerificationCode_WithRegisteredAccount_ShouldBeSuccessful()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        var account = await CreateProtectedAccount(deviceId, "roman@mogames.xyz", FakeDataFactory.GeneratePassword());
        var verificationResult = await _verificationService.SendVerificationCode(account.Email);

        verificationResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    private async Task SendVerificationCode_WithAlreadyVerifiedAccount_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = FakeDataFactory.GenerateEmail();

        var account = await CreateProtectedAccount(deviceId, email, FakeDataFactory.GeneratePassword());
        account.IsVerified = true;

        var verificationResult = await _verificationService.SendVerificationCode(account.Email);

        verificationResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task SendVerificationCode_WithAlreadySentCode_ShouldBeFailed()
    {
        string deviceId = FakeDataFactory.GenerateDeviceId();
        string email = FakeDataFactory.GenerateEmail();

        var account = await CreateProtectedAccount(deviceId, email, FakeDataFactory.GeneratePassword());
        await _context.VerificationCodes.AddAsync(new VerificationCode(email, Guid.NewGuid().ToString()));
        await _context.SaveChangesAsync();
        var verificationResult = await _verificationService.SendVerificationCode(account.Email);

        verificationResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task SendVerificationCode_WithUnregisteredAccount_ShouldBeFailed()
    {
        var verificationResult = await _verificationService.SendVerificationCode(FakeDataFactory.GenerateEmail());

        verificationResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    private async Task SubmitVerificationCode_WithValidCode_ShouldBeSuccessful()
    {
        string generatedEmail = FakeDataFactory.GenerateEmail();
        var account = await CreateProtectedAccount(
            FakeDataFactory.GenerateDeviceId(),
            generatedEmail,
            FakeDataFactory.GeneratePassword());

        var verificationServiceMock = VerificationServiceMockFactory.Create(_context);
        var code = await verificationServiceMock.SendVerificationCode(generatedEmail);

        var result = await verificationServiceMock.SubmitVerificationCode(code.Value.Code);
        
        result.IsSuccess.Should().BeTrue();
        account.IsVerified.Should().BeTrue();
    }
    
    [Fact]
    private async Task SubmitVerificationCode_WithInvalidCode_ShouldBeSuccessful()
    {
        string generatedEmail = FakeDataFactory.GenerateEmail();
        var account = await CreateProtectedAccount(
            FakeDataFactory.GenerateDeviceId(),
            generatedEmail,
            FakeDataFactory.GeneratePassword());

        var verificationServiceMock = VerificationServiceMockFactory.Create(_context);
        await verificationServiceMock.SendVerificationCode(generatedEmail);

        var result = await verificationServiceMock.SubmitVerificationCode(string.Empty);
        
        result.IsFailed.Should().BeTrue();
        account.IsVerified.Should().BeFalse();
    }
}

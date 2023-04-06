using Authentication.Api.Dto;
using Authentication.Api.Extensions;
using Authentication.Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IValidator<CreateGuestAccountDto> _guestAccountValidator;
    private readonly IValidator<ProtectAccountDto> _protectAccountValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly IAccountsService _accountsService;

    public AccountController(IValidator<CreateGuestAccountDto> guestAccountValidator,
        IValidator<ProtectAccountDto> protectAccountValidator, IValidator<ChangePasswordDto> changePasswordValidator,
        IAccountsService accountsService)
    {
        _guestAccountValidator = guestAccountValidator;
        _protectAccountValidator = protectAccountValidator;
        _changePasswordValidator = changePasswordValidator;
        _accountsService = accountsService;
    }

    [HttpPost("/account/guest")]
    public async Task<IActionResult> RegisterGuestAccount(CreateGuestAccountDto dto)
    {
        var validationResult = await _guestAccountValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _accountsService.RegisterGuestAccount(dto.DeviceId);
        if (!result.IsFailed) return Ok();

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }

    [HttpPost("/account/protect")]
    public async Task<IActionResult> ProtectAccount(ProtectAccountDto dto)
    {
        var validationResult = await _protectAccountValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _accountsService.ProtectAccount(dto.DeviceId, dto.Email, dto.Password);
        if (!result.IsFailed) return Ok();

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }

    [HttpPatch("/account")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }
        var result = await _accountsService.ChangePassword(dto.DeviceId, dto.OldPassword, dto.NewPassword);
        if (!result.IsFailed) return Ok();
        
        result.AddToModelState(ModelState);
        
        return ValidationProblem();
    }
}

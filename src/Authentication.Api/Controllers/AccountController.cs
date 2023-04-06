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
    private readonly IValidator<CreateGuestAccountDto> _guestAccountDtoValidator;
    private readonly IValidator<ProtectAccountDto> _protectAccountDtoValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordDtoValidator;
    private readonly IAccountsService _accountsService;

    public AccountController(IValidator<CreateGuestAccountDto> guestAccountDtoValidator,
        IValidator<ProtectAccountDto> protectAccountDtoValidator, IValidator<ChangePasswordDto> changePasswordDtoValidator,
        IAccountsService accountsService)
    {
        _guestAccountDtoValidator = guestAccountDtoValidator;
        _protectAccountDtoValidator = protectAccountDtoValidator;
        _changePasswordDtoValidator = changePasswordDtoValidator;
        _accountsService = accountsService;
    }

    [HttpPost("/account/guest")]
    public async Task<IActionResult> RegisterGuestAccount(CreateGuestAccountDto dto)
    {
        var validationResult = await _guestAccountDtoValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _accountsService.RegisterGuestAccount(dto.DeviceId);
        if (result.IsSuccess) return Ok();

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }

    [HttpPost("/account/protect")]
    public async Task<IActionResult> ProtectAccount(ProtectAccountDto dto)
    {
        var validationResult = await _protectAccountDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _accountsService.ProtectAccount(dto.DeviceId, dto.Email, dto.Password);
        if (result.IsSuccess) return Ok();

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }

    [HttpPatch("/account")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var validationResult = await _changePasswordDtoValidator.ValidateAsync(dto);
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

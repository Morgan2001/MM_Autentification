using Authentication.Api.Dto;
using Authentication.Api.Extensions;
using Authentication.Application.Interfaces;
using Authentication.Domain.Common;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IValidator<AuthenticateDto> _authenticateDtoValidator;
    private readonly IValidator<RefreshTokenDto> _refreshTokenDtoValidator;
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IValidator<AuthenticateDto> authenticateDtoValidator,
        IValidator<RefreshTokenDto> refreshTokenDtoValidator, IAuthenticationService authenticationService)
    {
        _authenticateDtoValidator = authenticateDtoValidator;
        _refreshTokenDtoValidator = refreshTokenDtoValidator;
        _authenticationService = authenticationService;
    }

    [HttpPost("/authenticate")]
    public async Task<ActionResult<JwtTokens>> Authenticate(AuthenticateDto dto)
    {
        var validationResult = await _authenticateDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _authenticationService.Authenticate(dto.Email, dto.Password);
        if (result.IsSuccess) return Ok(result.Value);

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }

    [HttpPost("/authenticate/refresh")]
    public async Task<ActionResult<JwtTokens>> RefreshTokens(RefreshTokenDto dto)
    {
        var validationResult = await _refreshTokenDtoValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return ValidationProblem();
        }

        var result = await _authenticationService.RefreshToken(dto.RefreshToken);
        if (result.IsSuccess) return Ok(result.Value);

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }
}

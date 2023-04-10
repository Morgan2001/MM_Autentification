using Authentication.Api.Extensions;
using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers;

[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;

    public VerificationController(IVerificationService verificationService)
    {
        _verificationService = verificationService;
    }

    [HttpPost("/verify/{email}")]
    public async Task<IActionResult> SendVerificationCode(string email)
    {
        var result = await _verificationService.SendVerificationCode(email);
        if (!result.IsFailed) return Ok();

        result.AddToModelState(ModelState);
        return ValidationProblem();
    }

    [HttpGet("/verify/{code}")]
    public async Task<IActionResult> SubmitVerificationCode(string code)
    {
        var result = await _verificationService.SubmitVerificationCode(code);
        if (!result.IsFailed) return Ok();

        result.AddToModelState(ModelState);

        return ValidationProblem();
    }
}

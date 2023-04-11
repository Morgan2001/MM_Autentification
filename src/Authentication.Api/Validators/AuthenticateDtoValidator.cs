using Authentication.Api.Dto;
using FluentValidation;

namespace Authentication.Api.Validators;

public class AuthenticateDtoValidator : AbstractValidator<AuthenticateDto>
{
    public AuthenticateDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

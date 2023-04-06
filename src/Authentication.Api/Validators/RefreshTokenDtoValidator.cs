using Authentication.Api.Dto;
using FluentValidation;

namespace Authentication.Api.Validators;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

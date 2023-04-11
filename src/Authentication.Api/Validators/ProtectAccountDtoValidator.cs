using Authentication.Api.Dto;
using FluentValidation;

namespace Authentication.Api.Validators;

public class ProtectAccountDtoValidator : AbstractValidator<ProtectAccountDto>
{
    public ProtectAccountDtoValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

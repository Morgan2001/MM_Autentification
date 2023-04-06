using Authentication.Api.Dto;
using FluentValidation;

namespace Authentication.Api.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.OldPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}

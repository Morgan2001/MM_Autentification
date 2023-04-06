using Authentication.Api.Dto;
using FluentValidation;

namespace Authentication.Api.Validators;

public class CreateGuestAccountDtoValidator : AbstractValidator<CreateGuestAccountDto>
{
    public CreateGuestAccountDtoValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
    }
}

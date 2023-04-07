namespace Authentication.Api.Dto;

public record ChangePasswordDto(string Email, string OldPassword, string NewPassword);

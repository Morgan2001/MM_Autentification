namespace Authentication.Api.Dto;

public record ChangePasswordDto(string DeviceId, string OldPassword, string NewPassword);
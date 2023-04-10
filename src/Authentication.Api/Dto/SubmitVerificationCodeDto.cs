namespace Authentication.Api.Dto;

public record SubmitVerificationCodeDto(string Email, string Code);
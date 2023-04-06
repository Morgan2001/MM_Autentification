namespace Authentication.Infrastructure.Options;

public record JwtGeneratorOptions
{
    public const string SectionName = "JwtGeneratorOptions";
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public long Lifetime { get; init; } = 1800;
    public int RefreshTokenLength { get; init; } = 32;
};

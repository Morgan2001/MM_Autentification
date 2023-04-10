namespace Authentication.Infrastructure.Options;

public record EmailOptions
{
    public const string SectionName = "EmailOptions";
    public string PostmarkToken { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
};
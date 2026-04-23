namespace PetRadar.API.Infrastructure.Options;

public sealed class FrontendCorsOptions
{
    public const string SectionName = "Cors:Frontend";

    public List<string> AllowedOrigins { get; init; } = [];
}

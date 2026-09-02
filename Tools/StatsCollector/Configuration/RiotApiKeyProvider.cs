namespace StatsCollector.Configuration;

public sealed class RiotApiKeyProvider
{
    private const string EnvironmentVariableName = "RIOT_API_KEY";

    public string? GetApiKey()
    {
        var apiKey = Environment.GetEnvironmentVariable(EnvironmentVariableName);

        return string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;
    }
}

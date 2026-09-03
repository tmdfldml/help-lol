using System.Text.Json;

namespace StatsCollector.Storage;

public sealed class SeedRotationStateStore
{
    private const int SchemaVersion = 1;
    private readonly string _statePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public SeedRotationStateStore(string collectorStateDirectory)
    {
        _statePath = Path.Combine(
            collectorStateDirectory,
            "discovery-state.json");
    }

    public SeedRotationStateLoadResult Load()
    {
        if (!File.Exists(_statePath))
        {
            return new SeedRotationStateLoadResult(new SeedRotationState());
        }

        try
        {
            var state = JsonSerializer.Deserialize<SeedRotationState>(
                File.ReadAllText(_statePath),
                _jsonOptions);

            if (state is null
                || state.SchemaVersion != SchemaVersion
                || state.NextSeedIndex < 0
                || state.LastKnownSeedPoolSize < 0)
            {
                return SeedRotationStateLoadResult.Reset;
            }

            return new SeedRotationStateLoadResult(state);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException)
        {
            return SeedRotationStateLoadResult.Reset;
        }
    }

    public async Task SaveAsync(
        int nextSeedIndex,
        int seedPoolSize,
        CancellationToken cancellationToken)
    {
        var state = new SeedRotationState(
            SchemaVersion,
            nextSeedIndex,
            seedPoolSize,
            DateTimeOffset.UtcNow);
        var temporaryPath = _statePath + ".tmp";

        await using (var stream = new FileStream(
                         temporaryPath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None))
        {
            await JsonSerializer.SerializeAsync(
                stream,
                state,
                _jsonOptions,
                cancellationToken);
            await stream.FlushAsync(cancellationToken);
            stream.Flush(true);
        }

        File.Move(temporaryPath, _statePath, true);
    }
}

public sealed record SeedRotationState(
    int SchemaVersion = 1,
    int NextSeedIndex = 0,
    int LastKnownSeedPoolSize = 0,
    DateTimeOffset UpdatedAtUtc = default);

public sealed record SeedRotationStateLoadResult(
    SeedRotationState State,
    bool WasReset = false)
{
    public static SeedRotationStateLoadResult Reset { get; } =
        new(new SeedRotationState(), true);
}

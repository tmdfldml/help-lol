using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using StatsCollector.Analysis;
using StatsCollector.Aggregation;
using StatsCollector.Collection;
using StatsCollector.Configuration;

namespace StatsCollector.Storage;

public sealed class CollectorStateStore
{
    private const int SchemaVersion = 1;
    private readonly string _discoveredPath;
    private readonly string _resultsPath;
    private readonly string _checkpointPath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly HashSet<string> _discoveredMatchIds =
        new(StringComparer.Ordinal);
    private readonly HashSet<string> _terminalMatchIds =
        new(StringComparer.Ordinal);
    private readonly Dictionary<MatchValidationOutcome, int> _outcomeCounts = [];

    public CollectorStateStore(CollectorOptions options)
    {
        var scopeDirectory = options.TierScope.ToDirectoryName();
        var regionDirectory = options.Region.Trim().ToUpperInvariant();
        var queueDirectory = options.QueueId.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        if (!PatchVersionParser.TryParseTargetPatch(
                options.TargetPatch,
                out var patch))
        {
            throw new ArgumentException(
                "TargetPatch must use the major.minor format.",
                nameof(options));
        }

        StateDirectory = Path.Combine(
            options.StateRootPath,
            scopeDirectory,
            regionDirectory,
            queueDirectory,
            patch.ToString());
        Directory.CreateDirectory(StateDirectory);
        _discoveredPath = Path.Combine(StateDirectory, "discovered-matches.jsonl");
        _resultsPath = Path.Combine(StateDirectory, "match-results.jsonl");
        _checkpointPath = Path.Combine(StateDirectory, "checkpoint.json");

        foreach (var outcome in Enum.GetValues<MatchValidationOutcome>())
        {
            _outcomeCounts[outcome] = 0;
        }
    }

    public string StateDirectory { get; }

    public IReadOnlySet<string> DiscoveredMatchIds => _discoveredMatchIds;

    public IReadOnlySet<string> TerminalMatchIds => _terminalMatchIds;

    public IReadOnlyDictionary<MatchValidationOutcome, int> OutcomeCounts =>
        _outcomeCounts;

    public int AcceptedMatchCount => _outcomeCounts[MatchValidationOutcome.Accepted];

    public int RejectedMatchCount =>
        _terminalMatchIds.Count - AcceptedMatchCount;

    public int ObservationCount { get; private set; }

    public int DuplicateTerminalRecordCount { get; private set; }

    public int IgnoredIncompleteLastLineCount { get; private set; }

    public CollectorStateRestoreSummary Restore(
        Action<IReadOnlyList<MatchupObservation>> replayAcceptedObservations)
    {
        LoadDiscoveredMatches();
        LoadTerminalResults(replayAcceptedObservations);

        return new CollectorStateRestoreSummary(
            _discoveredMatchIds.Count,
            _terminalMatchIds.Count,
            AcceptedMatchCount,
            ObservationCount,
            DuplicateTerminalRecordCount,
            IgnoredIncompleteLastLineCount);
    }

    public int AppendDiscovered(IEnumerable<string> matchIds)
    {
        var newIds = matchIds
            .Where(matchId => !string.IsNullOrWhiteSpace(matchId))
            .Where(matchId => !_discoveredMatchIds.Contains(matchId))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (newIds.Count == 0)
        {
            return 0;
        }

        using var stream = OpenAppendStream(_discoveredPath);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));

        foreach (var matchId in newIds)
        {
            writer.WriteLine(JsonSerializer.Serialize(
                new DiscoveredMatchRecord(SchemaVersion, matchId),
                _jsonOptions));
        }

        writer.Flush();
        stream.Flush(true);

        foreach (var matchId in newIds)
        {
            _discoveredMatchIds.Add(matchId);
        }

        return newIds.Count;
    }

    public void AppendAccepted(
        string matchId,
        string patch,
        IReadOnlyList<MatchupObservation> observations)
    {
        if (observations.Count != RoleNormalizer.StandardRoles.Count)
        {
            throw new ArgumentException(
                "An accepted match must contain exactly five observations.",
                nameof(observations));
        }

        AppendTerminal(new MatchTerminalRecord(
            SchemaVersion,
            matchId,
            MatchValidationOutcome.Accepted,
            patch,
            observations));
    }

    public void AppendRejected(string matchId, MatchValidationOutcome outcome)
    {
        if (outcome == MatchValidationOutcome.Accepted)
        {
            throw new ArgumentException(
                "Accepted matches must include observations.",
                nameof(outcome));
        }

        AppendTerminal(new MatchTerminalRecord(
            SchemaVersion,
            matchId,
            outcome,
            null,
            null));
    }

    public async Task WriteCheckpointAsync(
        CollectorOptions options,
        MatchupAccumulator accumulator)
    {
        var checkpoint = new CollectorCheckpoint(
            SchemaVersion,
            options.TierScope,
            options.Region.ToUpperInvariant(),
            options.QueueId,
            options.TargetPatch,
            _discoveredMatchIds.Count,
            _terminalMatchIds.Count,
            AcceptedMatchCount,
            RejectedMatchCount,
            ObservationCount,
            accumulator.Aggregates.Count,
            _discoveredMatchIds.Count - _terminalMatchIds.Count,
            DateTimeOffset.UtcNow);
        var temporaryPath = _checkpointPath + ".tmp";

        await using (var stream = new FileStream(
                         temporaryPath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None))
        {
            await JsonSerializer.SerializeAsync(stream, checkpoint, _jsonOptions);
            await stream.FlushAsync();
            stream.Flush(true);
        }

        File.Move(temporaryPath, _checkpointPath, true);
    }

    public long GetStorageSizeBytes() => Directory
        .EnumerateFiles(StateDirectory, "*", SearchOption.AllDirectories)
        .Sum(path => new FileInfo(path).Length);

    private void LoadDiscoveredMatches()
    {
        ReadJsonLines<DiscoveredMatchRecord>(
            _discoveredPath,
            record =>
            {
                if (record.SchemaVersion == SchemaVersion
                    && !string.IsNullOrWhiteSpace(record.MatchId))
                {
                    _discoveredMatchIds.Add(record.MatchId);
                }
            });
    }

    private void LoadTerminalResults(
        Action<IReadOnlyList<MatchupObservation>> replayAcceptedObservations)
    {
        ReadJsonLines<MatchTerminalRecord>(
            _resultsPath,
            record =>
            {
                if (record.SchemaVersion != SchemaVersion
                    || string.IsNullOrWhiteSpace(record.MatchId))
                {
                    throw new CollectorStateCorruptionException(
                        "A terminal result has an unsupported schema or no Match ID.");
                }

                if (!_terminalMatchIds.Add(record.MatchId))
                {
                    DuplicateTerminalRecordCount++;
                    return;
                }

                if (!_outcomeCounts.ContainsKey(record.Outcome))
                {
                    throw new CollectorStateCorruptionException(
                        "A terminal result has an unknown outcome.");
                }

                if (record.Outcome == MatchValidationOutcome.Accepted)
                {
                    if (record.Observations is not { Count: 5 })
                    {
                        throw new CollectorStateCorruptionException(
                            "An accepted terminal result does not contain five observations.");
                    }

                    replayAcceptedObservations(record.Observations);
                    ObservationCount += record.Observations.Count;
                }
                else if (record.Observations is { Count: > 0 })
                {
                    throw new CollectorStateCorruptionException(
                        "A rejected terminal result unexpectedly contains observations.");
                }

                _outcomeCounts[record.Outcome]++;
            });
    }

    private void AppendTerminal(MatchTerminalRecord record)
    {
        if (_terminalMatchIds.Contains(record.MatchId))
        {
            throw new InvalidOperationException(
                "The Match ID already has a terminal result.");
        }

        var json = JsonSerializer.Serialize(record, _jsonOptions);

        using (var stream = OpenAppendStream(_resultsPath))
        using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
        {
            writer.WriteLine(json);
            writer.Flush();
            stream.Flush(true);
        }

        _terminalMatchIds.Add(record.MatchId);
        _outcomeCounts[record.Outcome]++;

        if (record.Outcome == MatchValidationOutcome.Accepted)
        {
            ObservationCount += record.Observations?.Count ?? 0;
        }
    }

    private void ReadJsonLines<T>(string path, Action<T> acceptRecord)
    {
        if (!File.Exists(path))
        {
            return;
        }

        string? pendingLine = null;
        var pendingLineNumber = 0;
        var lineNumber = 0;

        using var reader = new StreamReader(path, Encoding.UTF8);

        while (reader.ReadLine() is { } line)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (pendingLine is not null)
            {
                ParseRequiredLine(pendingLine, pendingLineNumber, acceptRecord);
            }

            pendingLine = line;
            pendingLineNumber = lineNumber;
        }

        if (pendingLine is null)
        {
            return;
        }

        try
        {
            var record = JsonSerializer.Deserialize<T>(pendingLine, _jsonOptions)
                ?? throw new JsonException("The JSONL record was null.");
            acceptRecord(record);
        }
        catch (JsonException)
        {
            IgnoredIncompleteLastLineCount++;
            TruncateIncompleteLastLine(path);
        }
    }

    private void ParseRequiredLine<T>(
        string line,
        int lineNumber,
        Action<T> acceptRecord)
    {
        try
        {
            var record = JsonSerializer.Deserialize<T>(line, _jsonOptions)
                ?? throw new JsonException("The JSONL record was null.");
            acceptRecord(record);
        }
        catch (JsonException exception)
        {
            throw new CollectorStateCorruptionException(
                $"State corruption at JSONL line {lineNumber}.",
                exception);
        }
    }

    private static FileStream OpenAppendStream(string path) => new(
        path,
        FileMode.Append,
        FileAccess.Write,
        FileShare.Read);

    private static void TruncateIncompleteLastLine(string path)
    {
        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None);
        var position = stream.Length - 1;

        while (position >= 0)
        {
            stream.Position = position;

            if (stream.ReadByte() == '\n')
            {
                stream.SetLength(position + 1);
                return;
            }

            position--;
        }

        stream.SetLength(0);
    }
}

public sealed record CollectorStateRestoreSummary(
    int DiscoveredMatchCount,
    int TerminalResultCount,
    int AcceptedMatchCount,
    int ObservationCount,
    int DuplicateTerminalRecordCount,
    int IgnoredIncompleteLastLineCount);

public sealed class CollectorStateCorruptionException : Exception
{
    public CollectorStateCorruptionException(string message)
        : base(message)
    {
    }

    public CollectorStateCorruptionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

internal sealed record DiscoveredMatchRecord(int SchemaVersion, string MatchId);

internal sealed record MatchTerminalRecord(
    int SchemaVersion,
    string MatchId,
    MatchValidationOutcome Outcome,
    string? Patch,
    IReadOnlyList<MatchupObservation>? Observations);

internal sealed record CollectorCheckpoint(
    int SchemaVersion,
    TierScope TierScope,
    string Region,
    int QueueId,
    string TargetPatch,
    int DiscoveredMatchCount,
    int TerminalMatchCount,
    int AcceptedMatchCount,
    int RejectedMatchCount,
    int ObservationCount,
    int AggregatedMatchupKeyCount,
    int PendingMatchCount,
    DateTimeOffset LastUpdatedUtc);

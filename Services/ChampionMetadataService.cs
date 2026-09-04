using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LoLMatchup.Models.Champions;

namespace LoLMatchup.Services;

public sealed class ChampionMetadataService
{
    private const int SupportedSchemaVersion = 1;
    private readonly string _metadataPath;
    private readonly string _iconDirectory;
    private readonly Lazy<Task<IReadOnlyDictionary<string, ChampionMetadataEntry>>> _metadata;
    private readonly Dictionary<string, ImageSource?> _iconCache =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly object _iconCacheLock = new();

    public ChampionMetadataService()
    {
        var dataDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Champions");
        _metadataPath = Path.Combine(dataDirectory, "champions.ko.json");
        _iconDirectory = Path.Combine(dataDirectory, "Icons");
        _metadata = new Lazy<Task<IReadOnlyDictionary<string, ChampionMetadataEntry>>>(
            LoadMetadataAsync);
    }

    public async Task<ChampionDisplayInfo> GetDisplayInfoAsync(string? championId)
    {
        var normalizedId = championId?.Trim() ?? string.Empty;
        var fallbackName = normalizedId;
        var metadata = await _metadata.Value;

        if (normalizedId.Length == 0
            || !metadata.TryGetValue(normalizedId, out var entry))
        {
            return new ChampionDisplayInfo(normalizedId, fallbackName, null);
        }

        return new ChampionDisplayInfo(
            entry.Id,
            entry.NameKo,
            GetOrLoadIcon(entry.Icon));
    }

    private async Task<IReadOnlyDictionary<string, ChampionMetadataEntry>> LoadMetadataAsync()
    {
        try
        {
            await using var stream = File.OpenRead(_metadataPath);
            var document = await JsonSerializer.DeserializeAsync<ChampionMetadataDocument>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (document?.SchemaVersion != SupportedSchemaVersion
                || document.Champions is null)
            {
                return EmptyIndex();
            }

            var index = new Dictionary<string, ChampionMetadataEntry>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var entry in document.Champions)
            {
                if (string.IsNullOrWhiteSpace(entry.Id)
                    || string.IsNullOrWhiteSpace(entry.NameKo)
                    || !index.TryAdd(entry.Id.Trim(), entry with
                    {
                        Id = entry.Id.Trim(),
                        NameKo = entry.NameKo.Trim()
                    }))
                {
                    return EmptyIndex();
                }
            }

            return index;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException)
        {
            return EmptyIndex();
        }
    }

    private ImageSource? GetOrLoadIcon(string? iconFileName)
    {
        if (string.IsNullOrWhiteSpace(iconFileName)
            || !string.Equals(
                Path.GetFileName(iconFileName),
                iconFileName,
                StringComparison.Ordinal))
        {
            return null;
        }

        lock (_iconCacheLock)
        {
            if (_iconCache.TryGetValue(iconFileName, out var cached))
            {
                return cached;
            }

            var loaded = LoadIcon(Path.Combine(_iconDirectory, iconFileName));
            _iconCache[iconFileName] = loaded;
            return loaded;
        }
    }

    private static ImageSource? LoadIcon(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or FormatException
            or NotSupportedException)
        {
            return null;
        }
    }

    private static IReadOnlyDictionary<string, ChampionMetadataEntry> EmptyIndex() =>
        new Dictionary<string, ChampionMetadataEntry>(StringComparer.OrdinalIgnoreCase);

    private sealed class ChampionMetadataDocument
    {
        public int SchemaVersion { get; init; }
        public List<ChampionMetadataEntry>? Champions { get; init; }
    }

    private sealed record ChampionMetadataEntry
    {
        public string Id { get; init; } = string.Empty;
        public string NameKo { get; init; } = string.Empty;
        public string? Icon { get; init; }
    }
}

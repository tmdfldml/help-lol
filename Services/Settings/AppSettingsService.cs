using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoLMatchup.Models.Settings;

namespace LoLMatchup.Services.Settings;

public sealed class AppSettingsService
{
    private const string ApplicationDirectoryName = "LoLMatchInfo";
    private const string SettingsFileName = "settings.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _settingsPath;

    public AppSettingsService(string? settingsPath = null)
    {
        _settingsPath = settingsPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationDirectoryName,
            SettingsFileName);
    }

    public AppSettings Current { get; private set; } = new();
    public string SettingsPath => _settingsPath;
    public event EventHandler? SettingsChanged;

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return Current = new AppSettings();
            }

            var loaded = JsonSerializer.Deserialize<AppSettings>(
                File.ReadAllText(_settingsPath),
                JsonOptions);
            Current = loaded is { SchemaVersion: AppSettings.CurrentSchemaVersion }
                ? loaded
                : new AppSettings();
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException)
        {
            Debug.WriteLine($"Settings load failed; defaults used: {exception.GetType().Name}");
            Current = new AppSettings();
        }

        return Current;
    }

    public void Update(Func<AppSettings, AppSettings> update)
    {
        var updated = update(Current);
        if (updated == Current)
        {
            return;
        }

        Current = updated;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetToDefaults()
    {
        Current = new AppSettings();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Save()
    {
        var temporaryPath = _settingsPath + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(Current, JsonOptions));
            File.Move(temporaryPath, _settingsPath, overwrite: true);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException)
        {
            Debug.WriteLine($"Settings save failed: {exception.GetType().Name}");
            TryDeleteTemporaryFile(temporaryPath);
        }
    }

    private static void TryDeleteTemporaryFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"Settings temporary file cleanup failed: {exception.GetType().Name}");
        }
    }
}

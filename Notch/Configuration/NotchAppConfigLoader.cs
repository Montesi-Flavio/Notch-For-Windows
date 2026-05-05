using System.IO;
using System.Text.Json;

namespace Notch.Configuration;

public static class NotchAppConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static NotchAppSettings Load(string? path = null)
    {
        path ??= ResolveLoadPath();

        if (!File.Exists(path))
        {
            return new NotchAppSettings();
        }

        try
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<NotchAppSettings>(stream, JsonOptions) ?? new NotchAppSettings();
        }
        catch
        {
            return new NotchAppSettings();
        }
    }

    public static void Save(NotchAppSettings settings, string? path = null)
    {
        path ??= NotchAppConfigPaths.UserConfigPath;

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(path, json);
    }

    private static string ResolveLoadPath()
    {
        if (File.Exists(NotchAppConfigPaths.UserConfigPath))
        {
            return NotchAppConfigPaths.UserConfigPath;
        }

        return File.Exists(NotchAppConfigPaths.DefaultConfigPath)
            ? NotchAppConfigPaths.DefaultConfigPath
            : NotchAppConfigPaths.UserConfigPath;
    }
}

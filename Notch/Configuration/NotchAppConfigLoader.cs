using System.IO;
using System.Text.Json;

namespace Notch.Configuration;

public static class NotchAppConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static NotchAppSettings Load(string? path = null)
    {
        path ??= Path.Combine(AppContext.BaseDirectory, "appsettings.json");

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
}

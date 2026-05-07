using System.IO;

namespace Notch.Configuration;

public sealed class NotchAppSettings
{
    public WindowSettings Window { get; set; } = new();
    public SpotifySettings Spotify { get; set; } = new();
    public FeatureSettings Features { get; set; } = new();
}

public static class NotchAppConfigPaths
{
    public static string UserConfigDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Notch For Windows");

    public static string UserConfigPath => Path.Combine(UserConfigDirectory, "appsettings.json");

    public static string DefaultConfigPath => Path.Combine(AppContext.BaseDirectory, "appsettings.json");
}

public sealed class WindowSettings
{
    public double BaseWidth { get; set; } = 370;
    public double ExpandedWidth { get; set; } = 560;
    public double BaseHeight { get; set; } = 40;
    public double ExpandedHeight { get; set; } = 106;
}

public sealed class SpotifySettings
{
    public string RedirectUri { get; set; } = "http://127.0.0.1:53682/callback";
}

public sealed class FeatureSettings
{
    public bool EnableMusicModule { get; set; } = true;
    public bool EnableBatteryModule { get; set; } = true;
    public bool EnableCameraMirror { get; set; } = false;
    public bool EnableNotesIntegration { get; set; } = false;
}

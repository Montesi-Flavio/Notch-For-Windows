using System.IO;
using Windows.Media.Control;
using Windows.Media;

namespace Notch.Services;

public record VideoInfo(string Title, string SourceApp, bool IsPlaying, MemoryStream? Thumbnail);

public class MediaService
{
    private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;

    public async Task InitializeAsync()
    {
        _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
    }

    public async Task<VideoInfo?> GetCurrentVideoInfoAsync()
    {
        if (_sessionManager is null) return null;

        var session = _sessionManager.GetCurrentSession();
        if (session is null) return null;

        var props = await session.TryGetMediaPropertiesAsync();

        if (props.PlaybackType != MediaPlaybackType.Video)
            return null;

        var playback = session.GetPlaybackInfo();
        var isPlaying = playback?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;

        MemoryStream? thumbnail = null;
        if (props.Thumbnail is not null)
        {
            try
            {
                using var ras = await props.Thumbnail.OpenReadAsync();
                var ms = new MemoryStream((int)ras.Size);
                using var dotnet = ras.AsStreamForRead();
                await dotnet.CopyToAsync(ms);
                ms.Position = 0;
                thumbnail = ms;
            }
            catch { }
        }

        var sourceApp = ParseAppName(session.SourceAppUserModelId);

        return new VideoInfo(props.Title, sourceApp, isPlaying, thumbnail);
    }

    private static string ParseAppName(string appId)
    {
        if (string.IsNullOrEmpty(appId)) return string.Empty;
        var name = appId.Split(['\\', '!'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? appId;
        if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            name = name[..^4];
        return name;
    }
}

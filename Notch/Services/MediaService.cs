using Windows.Media.Control;

namespace Notch.Services;

public class MediaService
{
    private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;

    public async Task InitializeAsync()
    {
        _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
    }

    public async Task<(string Artist, string Title)> GetCurrentTrackInfo()
    {
 
        var session = _sessionManager.GetCurrentSession();
        if (session != null)
        {
            var properties = await session.TryGetMediaPropertiesAsync();
            return (properties.Artist, properties.Title);
        }
        return ("Nessun brano", "In pausa");
    }
}
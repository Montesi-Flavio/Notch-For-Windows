using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Notch.Core;
using Notch.Services;

namespace Notch.Modules;

public partial class SpotifyModule : NotchModuleBase
{
    private readonly SpotifyService _spotify = new();
    private readonly DispatcherTimer _pollTimer;
    private string _lastAlbumArtUrl = string.Empty;

    public override string ModuleName => "Spotify";

    public SpotifyModule()
    {
        InitializeComponent();
        SetActive(false); // nascosto finché non confermiamo che c'è qualcosa in play

        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _pollTimer.Tick += async (_, _) => await RefreshAsync();

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (!_spotify.IsConfigured) return;

        var authorized = await _spotify.AuthorizeAsync();
        if (!authorized) return;

        await RefreshAsync();
        _pollTimer.Start();
    }

    private async Task RefreshAsync()
    {
        var track = await _spotify.GetCurrentlyPlayingAsync();

        if (track is null || !track.IsPlaying)
        {
            SetActive(false);
            return;
        }

        TxtTitle.Text  = track.Title;
        TxtArtist.Text = track.Artist;

        if (track.AlbumArtUrl != _lastAlbumArtUrl)
        {
            _lastAlbumArtUrl = track.AlbumArtUrl;
            ImgAlbumArt.Source = string.IsNullOrEmpty(track.AlbumArtUrl)
                ? null
                : new BitmapImage(new Uri(track.AlbumArtUrl));
        }

        SetActive(true);
    }

    public override void OnModuleActivated()  => _ = RefreshAsync();
    public override void OnModuleDeactivated() { }
}

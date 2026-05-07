using System.Windows;
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

        NotchWindow.ExpandedChanged += OnExpandedChanged;
        UpdateCompactState(NotchWindow.IsExpanded);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (!_spotify.IsConfigured)
        {
            SetStatus("Config mancante: SPOTIFY_CLIENT_ID/SECRET");
            SetActive(true);
            return;
        }

        SetStatus("Autorizzazione in corso...");

        var authorized = await _spotify.AuthorizeAsync();
        if (!authorized)
        {
            SetStatus("Autorizzazione fallita");
            SetActive(true);
            return;
        }

        SetStatus(string.Empty);

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

    private void OnExpandedChanged(bool isExpanded)
    {
        UpdateCompactState(isExpanded);
    }

    private void UpdateCompactState(bool isExpanded)
    {
        if (TextPanel is null || RootPanel is null) return;

        TextPanel.Visibility = isExpanded
            ? Visibility.Visible
            : Visibility.Collapsed;

        RootPanel.Margin = isExpanded
            ? new Thickness(12, 6, 12, 6)
            : new Thickness(6, 4, 6, 4);
    }

    private void SetStatus(string message)
    {
        if (TxtStatus is null) return;

        TxtStatus.Text = message;
        TxtStatus.Visibility = string.IsNullOrWhiteSpace(message)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
}

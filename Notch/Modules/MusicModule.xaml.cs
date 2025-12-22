using Notch.Core;
using Notch.Services;

namespace Notch.Modules;

public partial class MusicModule : NotchModuleBase
{
    private MediaService _mediaService = new MediaService();

    public MusicModule()
    {
        InitializeComponent();
        SetupMedia();
    }

    private async void SetupMedia()
    {
        await _mediaService.InitializeAsync();
        UpdateInfo();
    }

    public override void OnModuleActivated()
    {
        UpdateInfo(); // Aggiorna ogni volta che il notch si espande
    }

    private async void UpdateInfo()
    {
        var (Artist, Title) = await _mediaService.GetCurrentTrackInfo();

        TxtTitle?.Text = Title ?? string.Empty;

        TxtArtist?.Text = Artist ?? string.Empty;
    }

    public override string ModuleName => "Music";
    public override void OnModuleDeactivated() { }
}
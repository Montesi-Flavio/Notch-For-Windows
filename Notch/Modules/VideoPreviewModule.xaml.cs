using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Notch.Core;
using Notch.Services;

namespace Notch.Modules;

public partial class VideoPreviewModule : NotchModuleBase
{
    private readonly MediaService _media = new();
    private readonly DispatcherTimer _pollTimer;

    public override string ModuleName => "VideoPreview";

    public VideoPreviewModule()
    {
        InitializeComponent();
        SetActive(false); // nascosto finché non rilevato un video

        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _pollTimer.Tick += async (_, _) => await RefreshAsync();

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _media.InitializeAsync();
        await RefreshAsync();
        _pollTimer.Start();
    }

    public override void OnModuleActivated()  => _ = RefreshAsync();
    public override void OnModuleDeactivated() { }

    private async Task RefreshAsync()
    {
        var info = await _media.GetCurrentVideoInfoAsync();

        if (info is null)
        {
            SetActive(false);
            return;
        }

        TxtTitle.Text  = info.Title;
        TxtSource.Text = info.SourceApp;

        EllipseStatus.Fill = new SolidColorBrush(info.IsPlaying
            ? Color.FromRgb(0x1D, 0xB9, 0x54)
            : Color.FromRgb(0x3D, 0x3D, 0x3D));

        if (info.Thumbnail is not null)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = info.Thumbnail;
            bitmap.CacheOption  = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            ImgThumbnail.Source = bitmap;
        }
        else
        {
            ImgThumbnail.Source = null;
        }

        SetActive(true);
    }
}

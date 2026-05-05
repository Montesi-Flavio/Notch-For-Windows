using System.Globalization;
using System.Windows;
using Notch.Configuration;

namespace Notch.Views;

public partial class SettingsWindow : Window
{
    private readonly NotchAppSettings _settings;

    public SettingsWindow(NotchAppSettings settings)
    {
        _settings = settings;

        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        BaseWidthBox.Text = _settings.Window.BaseWidth.ToString(CultureInfo.InvariantCulture);
        ExpandedWidthBox.Text = _settings.Window.ExpandedWidth.ToString(CultureInfo.InvariantCulture);
        BaseHeightBox.Text = _settings.Window.BaseHeight.ToString(CultureInfo.InvariantCulture);
        ExpandedHeightBox.Text = _settings.Window.ExpandedHeight.ToString(CultureInfo.InvariantCulture);

        SpotifyRedirectBox.Text = _settings.Spotify.RedirectUri;
        EnableMusicCheck.IsChecked = _settings.Features.EnableMusicModule;
        EnableBatteryCheck.IsChecked = _settings.Features.EnableBatteryModule;
        EnableCameraCheck.IsChecked = _settings.Features.EnableCameraMirror;
        EnableNotesCheck.IsChecked = _settings.Features.EnableNotesIntegration;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!TryParseDouble(BaseWidthBox.Text, out var baseWidth) ||
            !TryParseDouble(ExpandedWidthBox.Text, out var expandedWidth) ||
            !TryParseDouble(BaseHeightBox.Text, out var baseHeight) ||
            !TryParseDouble(ExpandedHeightBox.Text, out var expandedHeight))
        {
            MessageBox.Show(this, "Inserisci valori numerici validi.", "Impostazioni", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _settings.Window.BaseWidth = baseWidth;
        _settings.Window.ExpandedWidth = expandedWidth;
        _settings.Window.BaseHeight = baseHeight;
        _settings.Window.ExpandedHeight = expandedHeight;

        _settings.Spotify.RedirectUri = SpotifyRedirectBox.Text.Trim();
        _settings.Features.EnableMusicModule = EnableMusicCheck.IsChecked == true;
        _settings.Features.EnableBatteryModule = EnableBatteryCheck.IsChecked == true;
        _settings.Features.EnableCameraMirror = EnableCameraCheck.IsChecked == true;
        _settings.Features.EnableNotesIntegration = EnableNotesCheck.IsChecked == true;

        NotchAppConfigLoader.Save(_settings);

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private static bool TryParseDouble(string? value, out double result)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
               || double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result);
    }
}
using System.Windows.Input;
using Notch.Configuration;
using Notch.Services;
using Notch.ViewModels;

namespace Notch.Views;

public partial class MainWindow : NotchWindow 
{
    private readonly NotchAppSettings _settings;

    public MainWindow()
        : this(NotchAppConfigLoader.Load())
    {
    }

    public MainWindow(NotchAppSettings settings)
        : base(settings.Window)
    {
        _settings = settings;
        InitializeComponent();
        DataContext = new NotchViewModel(settings.Features);

        PreviewKeyDown += OnPreviewKeyDown;
        InitializeNotch();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.OemComma && Keyboard.Modifiers == ModifierKeys.Control)
        {
            OpenSettingsWindow();
            e.Handled = true;
        }
    }

    private void OpenSettingsWindow()
    {
        var settingsWindow = new SettingsWindow(_settings)
        {
            Owner = this
        };

        if (settingsWindow.ShowDialog() == true)
        {
            ApplyWindowSettings(_settings.Window);
            DataContext = new NotchViewModel(_settings.Features);
        }
    }
}
using Notch.Configuration;
using Notch.Services;
using Notch.ViewModels;

namespace Notch.Views;

public partial class MainWindow : NotchWindow 
{
    public MainWindow()
        : this(NotchAppConfigLoader.Load())
    {
    }

    public MainWindow(NotchAppSettings settings)
        : base(settings.Window)
    {
        InitializeComponent();
        DataContext = new NotchViewModel(settings.Features);

        InitializeNotch();
    }
}
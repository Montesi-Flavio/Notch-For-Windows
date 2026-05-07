using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Notch.Core;
using Notch.Services;

namespace Notch.Modules;

public partial class ClockModule : NotchModuleBase
{
    private readonly DispatcherTimer _timer;

    public override string ModuleName => "Clock";

    public override ModuleZone Zone => ModuleZone.Right;

    public ClockModule()
    {
        InitializeComponent();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => UpdateTime();

        NotchWindow.ExpandedChanged += OnExpandedChanged;
        UpdateCompactState(NotchWindow.IsExpanded);

        UpdateTime();
        _timer.Start();
        SetActive(true);
    }

    public override void OnModuleActivated() => UpdateTime();

    public override void OnModuleDeactivated() { }

    private void OnExpandedChanged(bool isExpanded)
    {
        UpdateCompactState(isExpanded);
    }

    private void UpdateCompactState(bool isExpanded)
    {
        if (TxtDate is null || RootPanel is null) return;

        TxtDate.Visibility = isExpanded
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Collapsed;

        RootPanel.Margin = isExpanded
            ? new Thickness(12, 6, 12, 6)
            : new Thickness(6, 4, 6, 4);
    }

    private void UpdateTime()
    {
        var now = DateTime.Now;
        TxtTime.Text = now.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
        TxtDate.Text = now.ToString("ddd dd/MM/yyyy", CultureInfo.CurrentCulture);
    }
}

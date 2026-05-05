using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Notch.Core;

namespace Notch.Modules;

public partial class BatteryModule : NotchModuleBase
{
    [DllImport("kernel32.dll")]
    private static extern bool GetSystemPowerStatus(out SystemPowerStatus status);

    [StructLayout(LayoutKind.Sequential)]
    private struct SystemPowerStatus
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte SystemStatusFlag;
        public uint BatteryLifeTime;
        public uint BatteryFullLifeTime;
    }

    private readonly DispatcherTimer _timer;

    public override string ModuleName => "Battery";
    public override ModuleZone Zone => ModuleZone.Right;

    public BatteryModule()
    {
        InitializeComponent();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += (_, _) => Refresh();
        _timer.Start();

        Refresh();
    }

    public override void OnModuleActivated() => Refresh();
    public override void OnModuleDeactivated() { }

    private void Refresh()
    {
        if (!GetSystemPowerStatus(out var ps)) return;

        bool noBattery = (ps.BatteryFlag & 128) != 0;
        if (noBattery) { SetActive(false); return; }

        SetActive(true);

        int level = ps.BatteryLifePercent == 255 ? 0 : ps.BatteryLifePercent;
        bool charging = (ps.BatteryFlag & 8) != 0 || ps.ACLineStatus == 1;

        TxtPercent.Text = $"{level}%";

        Color fillColor = level switch
        {
            < 20 => Color.FromRgb(0xFF, 0x3B, 0x30),
            < 50 => Color.FromRgb(0xFF, 0x9F, 0x0A),
            _    => Colors.White
        };
        if (charging) fillColor = Color.FromRgb(0x30, 0xD1, 0x58);

        BatteryFill.Background = new SolidColorBrush(fillColor);
        TxtPercent.Foreground  = new SolidColorBrush(fillColor);

        FillCol.Width  = new GridLength(level,       GridUnitType.Star);
        EmptyCol.Width = new GridLength(100 - level, GridUnitType.Star);

        TxtBolt.Visibility = charging ? Visibility.Visible : Visibility.Collapsed;
    }
}

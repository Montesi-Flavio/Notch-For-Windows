using Notch.Core;
using System.Windows.Markup;

namespace Notch.Modules;

public partial class BatteryModule : NotchModuleBase
{
    private bool _contentLoaded;

    // Provide a simple stub for InitializeComponent in case the XAML-generated method is not available.
    public void InitializeComponent()
    {
        // No-op: when XAML is present this is generated; keep a stub for compilation without XAML.
    }

    public override void OnModuleActivated()
    {
        throw new NotImplementedException();
    }

    public override void OnModuleDeactivated()
    {
        throw new NotImplementedException();
    }

    public BatteryModule()
    {}

    public override string ModuleName => "Battery";
}
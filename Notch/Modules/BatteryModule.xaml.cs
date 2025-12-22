using Notch.Core;

namespace Notch.Modules;

public partial class BatteryModule : NotchModuleBase
{
    public BatteryModule()
    {
        InitializeComponent();
    }

    public override string ModuleName => "Battery";

    public override void OnModuleActivated()
    {
        // Logica per aggiornare lo stato della batteria quando il modulo viene attivato
    }

    public override void OnModuleDeactivated()
    {
        // Eventuali operazioni da eseguire quando il modulo viene disattivato
    }
}
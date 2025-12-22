using System.Windows.Controls;

namespace Notch.Core;

public abstract class NotchModuleBase : UserControl
{
    // Proprietà comuni a tutti i moduli
    public abstract string ModuleName { get; }
    public abstract void OnModuleActivated(); // Chiamato quando il notch si apre
    public abstract void OnModuleDeactivated(); // Chiamato quando si chiude
}
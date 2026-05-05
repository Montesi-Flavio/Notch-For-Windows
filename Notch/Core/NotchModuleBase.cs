using System.Windows;
using System.Windows.Controls;

namespace Notch.Core;

public enum ModuleZone { Left, Right }

public abstract class NotchModuleBase : UserControl
{
    public abstract string ModuleName { get; }

    // Sovrascrivibile per posizionare il modulo nella zona destra (es. batteria)
    public virtual ModuleZone Zone => ModuleZone.Left;

    public abstract void OnModuleActivated();
    public abstract void OnModuleDeactivated();

    // Chiamare da ogni modulo per mostrare/nascondere sé stesso
    protected void SetActive(bool active)
        => Visibility = active ? Visibility.Visible : Visibility.Collapsed;
}

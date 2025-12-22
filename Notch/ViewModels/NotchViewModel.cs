using System.Collections.ObjectModel;
using Notch.Core;
using Notch.Modules;

namespace Notch.ViewModels;

public class NotchViewModel
{
    // Lista di moduli caricati dinamicamente
    public ObservableCollection<NotchModuleBase> ActiveModules { get; set; }

    public NotchViewModel()
    {
        ActiveModules = new ObservableCollection<NotchModuleBase>();
        LoadModules();
    }

    private void LoadModules()
    {
        // Qui potresti caricare i moduli da plugin esterni o via codice
       
    }
}
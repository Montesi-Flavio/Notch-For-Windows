using System.Collections.ObjectModel;
using System.Diagnostics;
using Notch.Core;
using System.Reflection;
using System.Windows;

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
        var moduleBaseType = typeof(NotchModuleBase);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();

        var moduleTypes = assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // ritorna i tipi caricati correttamente anche se alcuni falliscono
                    return ex.Types.Where(t => t != null)!;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Impossibile leggere i tipi dall'assembly {a.FullName}: {ex.Message}");
                    return Array.Empty<Type>();
                }
            })
            .Where(t => t is not null && moduleBaseType.IsAssignableFrom(t) && !t.IsAbstract)
            .Distinct()
            .ToList();

        foreach (var type in moduleTypes)
        {
            try
            {
                // Richiede costruttore senza parametri
                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    Debug.WriteLine($"Modulo ignorato (manca ctor parameterless): {type.FullName}");
                    continue;
                }

                if (Activator.CreateInstance(type) is NotchModuleBase module)
                {
                    // Assicura l'aggiunta sulla UI thread se disponibile
                    var dispatcher = Application.Current?.Dispatcher;
                    if (dispatcher != null && !dispatcher.CheckAccess())
                    {
                        dispatcher.Invoke(() => ActiveModules.Add(module));
                    }
                    else
                    {
                        ActiveModules.Add(module);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore creando il modulo {type.FullName}: {ex.Message}");
            }
        }
    }
}
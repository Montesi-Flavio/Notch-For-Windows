using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Notch.Configuration;
using Notch.Core;

namespace Notch.ViewModels;

public class NotchViewModel
{
    public ObservableCollection<NotchModuleBase> LeftModules  { get; } = new();
    public ObservableCollection<NotchModuleBase> RightModules { get; } = new();

    private readonly FeatureSettings _features;

    public NotchViewModel()
        : this(new FeatureSettings()) { }

    public NotchViewModel(FeatureSettings? features)
    {
        _features = features ?? new FeatureSettings();
        LoadModules();
    }

    private void LoadModules()
    {
        var baseType = typeof(NotchModuleBase);

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
                catch (Exception ex) { Debug.WriteLine($"Assembly skip: {ex.Message}"); return Array.Empty<Type>(); }
            })
            .Where(t => t is not null && baseType.IsAssignableFrom(t) && !t.IsAbstract)
            .Where(IsModuleEnabled)
            .Distinct();

        foreach (var type in types)
        {
            try
            {
                var ctor = type!.GetConstructor(Type.EmptyTypes);
                if (ctor is null) continue;

                if (Activator.CreateInstance(type) is not NotchModuleBase module) continue;

                var dispatcher = Application.Current?.Dispatcher;
                void Add()
                {
                    if (module.Zone == ModuleZone.Right)
                        RightModules.Add(module);
                    else
                        LeftModules.Add(module);
                }

                if (dispatcher is not null && !dispatcher.CheckAccess())
                    dispatcher.Invoke(Add);
                else
                    Add();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Modulo non caricato {type!.FullName ?? type.Name}: {ex.Message}");
            }
        }
    }

    private bool IsModuleEnabled(Type? type) => type?.Name switch
    {
        "BatteryModule"       => _features.EnableBatteryModule,
        "CameraModule"        => _features.EnableCameraMirror,
        "NotesModule"         => _features.EnableNotesIntegration,
        _                     => true
    };
}

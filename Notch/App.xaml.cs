using System.Configuration;
using System.Data;
using System.Windows;

namespace Notch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Crei la finestra manualmente
        var mainView = new Views.MainWindow();

        // Crei il ViewModel (il "cervello")
        var viewModel = new ViewModels.NotchViewModel();

        // Colleghi i due
        mainView.DataContext = viewModel;

        mainView.Show();
    }
}


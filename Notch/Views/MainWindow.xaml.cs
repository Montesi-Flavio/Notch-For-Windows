using Notch.Services;
using Notch.ViewModels;

namespace Notch.Views;

public partial class MainWindow : NotchWindow 
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new NotchViewModel();

        InitializeNotch();
    }
}
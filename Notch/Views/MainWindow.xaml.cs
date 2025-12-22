using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Notch.Views;

public partial class MainWindow : Window
{
    // Aggiungi questo campo alla classe MainWindow per risolvere CS0103
    private FrameworkElement? ContentArea;

    public MainWindow()
    {
        InitializeComponent();

        // Inizializza ContentArea dopo InitializeComponent
        ContentArea = this.FindName("ContentArea") as FrameworkElement;

        double screenWidth = SystemParameters.PrimaryScreenWidth;
        this.Left = (screenWidth / 2) - (this.Width / 2);
        this.Top = 0;
    }

    private void NotchBorder_MouseEnter(object sender, MouseEventArgs e)
    {
        DoubleAnimation heightAnim = new DoubleAnimation(150, TimeSpan.FromMilliseconds(300));
        heightAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut };

        DoubleAnimation opacityAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));

        NotchBorder?.BeginAnimation(HeightProperty, heightAnim);
        this.BeginAnimation(HeightProperty, heightAnim);
        ContentArea?.BeginAnimation(OpacityProperty, opacityAnim);
    }

    private void NotchBorder_MouseLeave(object sender, MouseEventArgs e)
    {
        DoubleAnimation heightAnim = new DoubleAnimation(40, TimeSpan.FromMilliseconds(300));
        heightAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn };

        DoubleAnimation opacityAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));

        NotchBorder?.BeginAnimation(HeightProperty, heightAnim);
        this.BeginAnimation(HeightProperty, heightAnim);
        ContentArea?.BeginAnimation(OpacityProperty, opacityAnim);
    }
}

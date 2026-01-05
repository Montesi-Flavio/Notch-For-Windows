using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Notch.Services;

public class NotchWindow : Window
{
    protected Border? NotchBorder;
    protected FrameworkElement? ContentArea;

    // Dimensioni CONFIGURABILI
    private const double BaseWidth = 370;      // Larghezza a riposo
    private const double ExpandedWidth = 450;  // Larghezza espansa

    private const double BaseHeight = 40;      // Altezza a riposo
    private const double ExpandedHeight = 150; // Altezza espansa

    public NotchWindow()
    {
        // Impostazioni essenziali per la trasparenza
        this.WindowStyle = WindowStyle.None;
        this.AllowsTransparency = true;
        this.Background = System.Windows.Media.Brushes.Transparent;
        this.Topmost = true;
    }

    protected void InitializeNotch()
    {
        NotchBorder = FindName("NotchBorder") as Border;
        ContentArea = FindName("ContentArea") as FrameworkElement;

        // Posizioniamo la finestra (che è larga 500px invisibili) al centro dello schermo
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        this.Left = (screenWidth / 2) - (this.Width / 2);
        this.Top = 0;

        // Eventi Mouse sul BORDO, non sulla finestra (altrimenti cattura il mouse anche sul trasparente)
        if (NotchBorder != null)
        {
            NotchBorder.MouseEnter += (s, e) => AnimateNotch(true);
            NotchBorder.MouseLeave += (s, e) => AnimateNotch(false);
        }
    }

    private void AnimateNotch(bool isExpanded)
    {
        TimeSpan duration = TimeSpan.FromMilliseconds(300);

        // Easing "Quartic" rende il movimento molto naturale
        var ease = isExpanded
            ? (IEasingFunction)new QuarticEase { EasingMode = EasingMode.EaseOut }
            : new QuarticEase { EasingMode = EasingMode.EaseIn };

        // Definizione Valori Target
        double targetWidth = isExpanded ? ExpandedWidth : BaseWidth;
        double targetHeight = isExpanded ? ExpandedHeight : BaseHeight;
        double targetOpacity = isExpanded ? 1.0 : 0.0;

        // 1. Animiamo Larghezza e Altezza del BORDO CENTRALE
        // Nota: Non tocchiamo la Window, solo il Border interno!
        if (NotchBorder != null)
        {
            var widthAnim = new DoubleAnimation(targetWidth, duration) { EasingFunction = ease };
            var heightAnim = new DoubleAnimation(targetHeight, duration) { EasingFunction = ease };

            NotchBorder.BeginAnimation(FrameworkElement.WidthProperty, widthAnim);
            NotchBorder.BeginAnimation(FrameworkElement.HeightProperty, heightAnim);
        }

        // 2. Animiamo l'Opacità del contenuto
        ContentArea?.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(targetOpacity, duration));
    }
}
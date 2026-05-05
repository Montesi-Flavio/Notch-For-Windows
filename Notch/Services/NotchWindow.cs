using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Notch.Configuration;

namespace Notch.Services;

public class NotchWindow : Window
{
    protected Border? NotchBorder;
    protected FrameworkElement? ContentArea;

    private readonly double _baseWidth;
    private readonly double _expandedWidth;
    private readonly double _baseHeight;
    private readonly double _expandedHeight;

    protected NotchWindow()
        : this(new WindowSettings())
    {
    }

    protected NotchWindow(WindowSettings? settings)
    {
        settings ??= new WindowSettings();

        _baseWidth = settings.BaseWidth;
        _expandedWidth = settings.ExpandedWidth;
        _baseHeight = settings.BaseHeight;
        _expandedHeight = settings.ExpandedHeight;

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

        Width = _expandedWidth + 30;
        Height = _expandedHeight;

        // Posizioniamo la finestra (che � larga 500px invisibili) al centro dello schermo
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        this.Left = (screenWidth / 2) - (this.Width / 2);
        this.Top = 0;

        if (NotchBorder != null)
        {
            NotchBorder.Width = _baseWidth;
            NotchBorder.Height = _baseHeight;
        }

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
        double targetWidth = isExpanded ? _expandedWidth : _baseWidth;
        double targetHeight = isExpanded ? _expandedHeight : _baseHeight;
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

        // 2. Animiamo l'Opacit� del contenuto
        ContentArea?.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(targetOpacity, duration));
    }
}
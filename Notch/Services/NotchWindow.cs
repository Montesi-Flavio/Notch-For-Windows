using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Notch.Configuration;

namespace Notch.Services;

public class NotchWindow : Window
{
    protected Border? _notchBorder;
    protected FrameworkElement? _contentArea;
    private TranslateTransform? _contentSlide;

    private double _baseWidth;
    private double _expandedWidth;
    private double _baseHeight;
    private double _expandedHeight;

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

        this.WindowStyle = WindowStyle.None;
        this.AllowsTransparency = true;
        this.Background = Brushes.Transparent;
        this.Topmost = true;
    }

    protected void InitializeNotch()
    {
        _notchBorder = FindName("NotchBorder") as Border;
        _contentArea = FindName("ContentArea") as FrameworkElement;

        // Prepara il TranslateTransform per l'effetto slide-up del contenuto
        if (_contentArea is not null)
        {
            _contentSlide = new TranslateTransform(0, 8);
            _contentArea.RenderTransform = _contentSlide;
        }

        ApplyWindowSettings(_baseWidth, _expandedWidth, _baseHeight, _expandedHeight);

        if (_notchBorder is not null)
        {
            _notchBorder.MouseEnter += (_, _) => AnimateNotch(true);
            _notchBorder.MouseLeave += (_, _) => AnimateNotch(false);
        }
    }

    private void AnimateNotch(bool expand)
    {
        if (expand)
            AnimateOpen();
        else
            AnimateClose();
    }

    private void AnimateOpen()
    {
        // 1. Border si espande con QuinticEase (rapido + morbido come Dynamic Island)
        var borderEase = new QuinticEase { EasingMode = EasingMode.EaseOut };
        var borderDuration = TimeSpan.FromMilliseconds(380);

        _notchBorder?.BeginAnimation(FrameworkElement.WidthProperty,
            new DoubleAnimation(_expandedWidth, borderDuration) { EasingFunction = borderEase });
        _notchBorder?.BeginAnimation(FrameworkElement.HeightProperty,
            new DoubleAnimation(_expandedHeight, borderDuration) { EasingFunction = borderEase });

        // 2. Contenuto appare 110ms dopo, con slide-up da +8px a 0
        var contentEase = new CubicEase { EasingMode = EasingMode.EaseOut };
        var contentDuration = TimeSpan.FromMilliseconds(260);
        var contentDelay = TimeSpan.FromMilliseconds(110);

        if (_contentArea is not null)
        {
            _contentArea.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(1.0, contentDuration)
                {
                    EasingFunction = contentEase,
                    BeginTime = contentDelay
                });
        }

        _contentSlide?.BeginAnimation(TranslateTransform.YProperty,
            new DoubleAnimation(0, contentDuration)
            {
                EasingFunction = contentEase,
                BeginTime = contentDelay
            });
    }

    private void AnimateClose()
    {
        // 1. Contenuto sparisce subito (rapido, deciso)
        var hideDuration = TimeSpan.FromMilliseconds(100);
        var hideEase = new QuadraticEase { EasingMode = EasingMode.EaseIn };

        if (_contentArea is not null)
        {
            _contentArea.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0.0, hideDuration) { EasingFunction = hideEase });
        }

        _contentSlide?.BeginAnimation(TranslateTransform.YProperty,
            new DoubleAnimation(8, hideDuration) { EasingFunction = hideEase });

        // 2. Border si chiude 80ms dopo (aspetta che il contenuto scompaia)
        var borderEase = new QuarticEase { EasingMode = EasingMode.EaseIn };
        var borderDuration = TimeSpan.FromMilliseconds(220);
        var borderDelay = TimeSpan.FromMilliseconds(80);

        _notchBorder?.BeginAnimation(FrameworkElement.WidthProperty,
            new DoubleAnimation(_baseWidth, borderDuration)
            {
                EasingFunction = borderEase,
                BeginTime = borderDelay
            });
        _notchBorder?.BeginAnimation(FrameworkElement.HeightProperty,
            new DoubleAnimation(_baseHeight, borderDuration)
            {
                EasingFunction = borderEase,
                BeginTime = borderDelay
            });
    }

    protected void ApplyWindowSettings(WindowSettings settings)
    {
        ApplyWindowSettings(settings.BaseWidth, settings.ExpandedWidth, settings.BaseHeight, settings.ExpandedHeight);
    }

    private void ApplyWindowSettings(double baseWidth, double expandedWidth, double baseHeight, double expandedHeight)
    {
        _baseWidth = baseWidth;
        _expandedWidth = expandedWidth;
        _baseHeight = baseHeight;
        _expandedHeight = expandedHeight;

        Width = _expandedWidth + 36;
        Height = _expandedHeight;

        double screenWidth = SystemParameters.PrimaryScreenWidth;
        Left = (screenWidth / 2) - (Width / 2);
        Top = 0;

        if (_notchBorder is not null)
        {
            _notchBorder.Width = _baseWidth;
            _notchBorder.Height = _baseHeight;
        }
    }
}

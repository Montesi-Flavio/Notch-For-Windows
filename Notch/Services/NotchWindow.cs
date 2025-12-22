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

        public NotchWindow()
        {
        }

        protected void InitializeNotch()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            this.Left = (screenWidth / 2) - (this.Width / 2);
            this.Top = 0;

            NotchBorder = FindName("NotchBorder") as Border;
            ContentArea = FindName("ContentArea") as FrameworkElement;

            if (NotchBorder != null)
            {
                NotchBorder.MouseEnter += NotchBorder_MouseEnter;
                NotchBorder.MouseLeave += NotchBorder_MouseLeave;
            }
        }

        protected virtual void NotchBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation heightAnim = new DoubleAnimation(150, TimeSpan.FromMilliseconds(300));
            heightAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut };

            DoubleAnimation opacityAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));

            NotchBorder?.BeginAnimation(HeightProperty, heightAnim);
            this.BeginAnimation(HeightProperty, heightAnim);
            ContentArea?.BeginAnimation(OpacityProperty, opacityAnim);
        }

        protected virtual void NotchBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation heightAnim = new DoubleAnimation(40, TimeSpan.FromMilliseconds(300));
            heightAnim.EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn };

            DoubleAnimation opacityAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));

            NotchBorder?.BeginAnimation(HeightProperty, heightAnim);
            this.BeginAnimation(HeightProperty, heightAnim);
            ContentArea?.BeginAnimation(OpacityProperty, opacityAnim);
        }
    }


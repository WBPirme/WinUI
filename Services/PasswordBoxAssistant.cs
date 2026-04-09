// Helpers/PasswordBoxAssistant.cs
using System.Windows;
using System.Windows.Controls;

namespace OGAS.Services
{
    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAssistant), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false));

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPassword, value);
        }

        public static bool GetBindPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(BindPassword);
        }

        public static void SetBindPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(BindPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdatingPassword, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                if (!(bool)GetUpdatingPassword(passwordBox))
                {
                    passwordBox.Password = e.NewValue as string ?? string.Empty;
                }
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                bool wasBound = (bool)(e.OldValue);
                bool needToBind = (bool)(e.NewValue);

                if (wasBound)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }

                if (needToBind)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }
    }
}

// Views/LoginWindow.xaml.cs
using OGAS.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace OGAS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 允许拖动窗口
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 将焦点移动到密码输入框
                PasswordBox.Focus();
                e.Handled = true; // 防止事件进一步传播
            }
        }

        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 执行登录命令
                if (_viewModel.LoginCommand.CanExecute(null))
                {
                    await _viewModel.LoginCommand.ExecuteAsync(null);
                }
                e.Handled = true; // 防止事件进一步传播
            }
        }
    }
}

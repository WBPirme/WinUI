using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OGAS.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace OGAS.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        // 导航命令
        public IRelayCommand<string> NavigateCommand { get; }
        public IRelayCommand LogoutCommand { get; }

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new RelayCommand<string>(Navigate);

            LogoutCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(Logout);

            // 初始化时导航到 Main 页面
            SelectedPage = "Main"; // 设置初始页面为 Main
            _navigationService.NavigateTo("Main");
        }

        // 当前选中的页面
        private string _selectedPage;
        public string SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (_selectedPage != value)
                {
                    _selectedPage = value;
                    OnPropertyChanged(nameof(SelectedPage)); // 通知 UI 发生了变化
                }
            }
        }

        // 导航逻辑
        private void Navigate(string pageKey)
        {
            if (!string.IsNullOrEmpty(pageKey) && pageKey != SelectedPage)
            {
                SelectedPage = pageKey;
                _navigationService.NavigateTo(pageKey);
            }
        }

        // 登出逻辑
        private void Logout()
        {
            // 实现登出逻辑，例如关闭当前窗口并显示登录窗口
            System.Windows.Application.Current.MainWindow?.Close();
        }
    }
}

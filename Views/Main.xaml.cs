// Views/Main.xaml.cs
using System.Windows.Controls;
using OGAS.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace OGAS.Views
{
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();

            // 从 ServiceProvider 获取 MainViewModel 实例
            var viewModel = App.ServiceProvider.GetService<MainViewModel>();

            if (viewModel != null)
            {
                // 设置 DataContext
                this.DataContext = viewModel;
            }
            else
            {
                // 处理 ViewModel 获取失败的情况
                // 例如，显示错误消息或日志
                MessageBox.Show("无法获取 MainViewModel 实例。");
            }
        }
    }
}

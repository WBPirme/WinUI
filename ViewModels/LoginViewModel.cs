// ViewModels/LoginViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OGAS.Data;
using OGAS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace OGAS.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IDbContextFactory<OGASDbContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;

        public LoginViewModel(IDbContextFactory<OGASDbContext> contextFactory, IServiceProvider serviceProvider)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
        }

        #region 属性

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        #endregion

        #region 命令

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ShowMessage("请输入用户名。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ShowMessage("请输入密码。", "提示", MessageBoxImage.Warning);
                return;
            }

            bool isValid = false;

            try
            {
                using var context = _contextFactory.CreateDbContext();
                isValid = await context.用户.AnyAsync(u => u.编号 == Username && u.密码 == Password);
            }
            catch (Exception ex)
            {
                ShowMessage($"数据库连接失败：{ex.Message}", "错误", MessageBoxImage.Error);
                return;
            }

            if (isValid)
            {
                // 设置 DialogResult 为 true
                // 获取当前窗口并设置 DialogResult
                if (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this) is Window window)
                {
                    window.DialogResult = true;
                }
            }
            else
            {
                ShowMessage("用户名或密码错误，请重试。", "错误", MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Reset()
        {
            Username = string.Empty;
            Password = string.Empty;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="caption">标题</param>
        /// <param name="icon">图标类型</param>
        private void ShowMessage(string message, string caption = "提示", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        #endregion
    }
}

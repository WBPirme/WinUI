// Services/NavigationService.cs
using System;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OGAS.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public Frame MainFrame { get; set; }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo(string pageKey)
        {
            var pageType = Type.GetType($"OGAS.Views.{pageKey}");

            if (pageType != null)
            {
                var page = _serviceProvider.GetRequiredService(pageType) as Page;

                MainFrame?.Navigate(page);
            }
            else
            {
                throw new ArgumentException($"无法找到页面：{pageKey}");
            }
        }
    }
}

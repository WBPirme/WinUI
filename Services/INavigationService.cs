// Services/INavigationService.cs
using System.Windows.Controls;

namespace OGAS.Services
{
    public interface INavigationService
    {
        Frame MainFrame { get; set; } // 添加这一行

        void NavigateTo(string pageKey);
    }
}

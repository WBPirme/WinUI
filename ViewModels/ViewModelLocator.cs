// ViewModels/ViewModelLocator.cs
using Microsoft.Extensions.DependencyInjection;

namespace OGAS.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.ServiceProvider.GetService<MainViewModel>();
    }
}

// Views/Monitor.xaml.cs
using OGAS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace OGAS.Views
{
    public partial class Monitor : Page
    {
        public Monitor(MonitorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

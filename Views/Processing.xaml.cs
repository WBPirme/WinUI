using System.Windows.Controls;
using OGAS.ViewModels;
using OGAS.Data;

namespace OGAS.Views
{
    public partial class Processing : Page
    {
        private readonly ProcessingViewModel _viewModel;

        public Processing()
        {
            InitializeComponent();

            // 创建 OGASDbContextFactory 实例
            var contextFactory = new OGASDbContextFactory();

            _viewModel = new ProcessingViewModel(contextFactory);
            this.DataContext = _viewModel;
        }
    }
}

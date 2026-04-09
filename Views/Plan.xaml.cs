using OGAS.ViewModels;
using System.Windows.Controls;
using OGAS.Services;

namespace OGAS.Views
{
    public partial class Plan : Page
    {
        private readonly PlanViewModel _viewModel;

        public Plan(PlanViewModel viewModel)
        {
            InitializeComponent();

            // 使用构造函数参数直接设置 _viewModel
            _viewModel = viewModel;

            // 设置 DataContext
            this.DataContext = _viewModel;
        }

        private void 计划类型ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 实现您的逻辑，例如更新 ViewModel
            if (_viewModel != null)
            {
                // 根据选择项更新 ViewModel 的某些属性
            }
        }
    }
}

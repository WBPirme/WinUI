// Views/DataCollection.xaml.cs
using System.Windows.Controls;
using OGAS.ViewModels;

namespace OGAS.Views
{
    public partial class DataCollection : Page
    {
        public DataCollection(DataCollectionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

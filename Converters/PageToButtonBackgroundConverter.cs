using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace OGAS.Converters 
{ 
    public class PageToButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
        string currentPage = value as string;
        string targetPage = parameter as string;

            if (currentPage == targetPage)
            {
                return new SolidColorBrush(Color.FromRgb(90, 80, 128)); // #5a5080
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // 不需要反向转换
        }
    }
}
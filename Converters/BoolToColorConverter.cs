// Converters/BoolToColorConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OGAS.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = Brushes.Red;
        public Brush FalseBrush { get; set; } = Brushes.Green;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueBrush : FalseBrush;
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

// Controls/InfoCard.xaml.cs
using System.Windows;
using System.Windows.Controls;

namespace OGAS.Controls
{
    public partial class InfoCard : UserControl
    {
        public InfoCard()
        {
            InitializeComponent();
        }

        // Icon
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

        // Desc
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }

        public static readonly DependencyProperty DescProperty =
            DependencyProperty.Register("Desc", typeof(string), typeof(InfoCard), new PropertyMetadata(string.Empty));

        // Value
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(InfoCard), new PropertyMetadata(0m));

        // Percentage
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(InfoCard), new PropertyMetadata(0.0));

        // IsActive
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(InfoCard), new PropertyMetadata(false));
    }
}

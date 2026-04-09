// ViewModels/ProductionProgressBar.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace OGAS.ViewModels
{
    public class ProductionProgressBar : ObservableObject
    {
        private double _currentProgress;
        public double CurrentProgress
        {
            get => _currentProgress;
            set => SetProperty(ref _currentProgress, value);
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        private string _remainingTime;
        public string RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        // 根据状态返回进度条颜色
        public string ProgressBarColor
        {
            get
            {
                return Status switch
                {
                    "生产中" => "Green",
                    "等待中" => "#FFCC00",
                    _ => "Gray",
                };
            }
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
            OnPropertyChanged(nameof(ProgressBarColor));
        }
    }
}

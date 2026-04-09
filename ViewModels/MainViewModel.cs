// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using OGAS.Data;
using OGAS.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OGAS.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDbContextFactory<OGASDbContext> _contextFactory;

        public MainViewModel(IDbContextFactory<OGASDbContext> contextFactory)
        {
            _contextFactory = contextFactory;

            初始化网络流量图表();
            初始化定时器();

            系统日志字符串 = "系统初始化完成。\n";

            // 异步加载数据
            _ = LoadDataAsync();
        }

        // 图表相关属性
        [ObservableProperty]
        private PlotModel _网络流量图表;

        // 生产计划相关属性
        [ObservableProperty]
        private int _未开始计划数量;
        [ObservableProperty]
        private string _未开始计划百分比;

        [ObservableProperty]
        private int _已开始计划数量;
        [ObservableProperty]
        private string _已开始计划百分比;

        [ObservableProperty]
        private int _未完成计划数量;
        [ObservableProperty]
        private string _未完成计划百分比;

        // 出口收益
        [ObservableProperty]
        private decimal _出口收益;

        // 系统日志
        [ObservableProperty]
        private string _系统日志字符串;

        // 库存及出口相关属性
        [ObservableProperty]
        private int _材料库存总数;

        [ObservableProperty]
        private int _产品库存总数;

        [ObservableProperty]
        private int _出口物品总数;

        [ObservableProperty]
        private double _材料库存占比;

        [ObservableProperty]
        private double _产品库存占比;

        [ObservableProperty]
        private double _出口百分比;

        // 定时器与网络计数器
        private DispatcherTimer _updateTimer;
        private LineSeries _网络流量系列;
        private PerformanceCounter _networkCounter;

        // 初始化网络流量图表
        private void 初始化网络流量图表()
        {
            网络流量图表 = new PlotModel { };

            // 设置时间轴
            var 时间轴 = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "hh:mm:ss",
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            网络流量图表.Axes.Add(时间轴);

            // 设置数值轴
            var 数值轴 = new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            网络流量图表.Axes.Add(数值轴);

            // 添加数据系列
            _网络流量系列 = new LineSeries { };
            网络流量图表.Series.Add(_网络流量系列);

            // 初始化网络性能计数器
            _networkCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec", GetNetworkInterface());
        }

        // 初始化定时器
        private void 初始化定时器()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            _updateTimer.Tick += async (s, e) => await 定时器触发事件处理异步();
            _updateTimer.Start();
        }

        // 定时器触发事件处理
        private async Task 定时器触发事件处理异步()
        {
            更新网络流量数据();
            await Task.CompletedTask;
        }

        // 更新网络流量数据
        private void 更新网络流量数据()
        {
            // 获取当前网络流量数据 (kbps)
            double currentValue = _networkCounter.NextValue() / 1024;
            DateTime now = DateTime.Now;

            var dataPoint = new DataPoint(DateTimeAxis.ToDouble(now), currentValue);
            _网络流量系列.Points.Add(dataPoint);

            // 保持数据点数量不超过60个（1分钟）
            if (_网络流量系列.Points.Count > 60)
            {
                _网络流量系列.Points.RemoveAt(0);
            }

            // 刷新 PlotModel
            网络流量图表.InvalidatePlot(true);
        }

        // 更新系统日志
        private void 更新系统日志(string message)
        {
            系统日志字符串 += $"[{DateTime.Now}] {message}\n";
        }

        // 获取网络接口名称
        private string GetNetworkInterface()
        {
            var category = new PerformanceCounterCategory("Network Interface");
            var instanceNames = category.GetInstanceNames();
            foreach (var name in instanceNames)
            {
                if (!name.Contains("Loopback"))
                {
                    return name;
                }
            }
            return instanceNames[0];
        }

        // 异步加载数据
        private async Task LoadDataAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    // 生产计划统计
                    int totalPlans = await context.生产计划s.CountAsync();
                    int notStartedPlans = await context.生产计划s.CountAsync(p => p.状态 == "未开始");
                    int startedPlans = await context.生产计划s.CountAsync(p => p.状态 == "已取消");
                    int notCompletedPlans = await context.生产计划s.CountAsync(p => p.状态 == "已完成");

                    未开始计划数量 = notStartedPlans;
                    已开始计划数量 = startedPlans;
                    未完成计划数量 = notCompletedPlans;

                    未开始计划百分比 = totalPlans > 0 ? (notStartedPlans / (double)totalPlans).ToString("P0") : "0%";
                    已开始计划百分比 = totalPlans > 0 ? (startedPlans / (double)totalPlans).ToString("P0") : "0%";
                    未完成计划百分比 = totalPlans > 0 ? (notCompletedPlans / (double)totalPlans).ToString("P0") : "0%";

                    // 材料库存与产品库存
                    decimal totalMaterialStock = await context.材料库存s.SumAsync(m => m.数量);
                    decimal totalProductStock = await context.产品库存s.SumAsync(p => p.数量);

                    材料库存总数 = (int)totalMaterialStock;
                    产品库存总数 = (int)totalProductStock;

                    decimal stockSum = totalMaterialStock + totalProductStock;
                    double materialPercentageValue = 0.0;
                    double productPercentageValue = 0.0;

                    if (stockSum > 0)
                    {
                        materialPercentageValue = (double)(totalMaterialStock / stockSum * 100);
                        productPercentageValue = (double)(totalProductStock / stockSum * 100);
                    }

                    // 将 ViewModel 中的 材料库存占比、产品库存占比 定义为 double
                    材料库存占比 = materialPercentageValue;
                    产品库存占比 = productPercentageValue;


                    // 出口订单
                    decimal totalExport = await context.出口订单s.SumAsync(e => e.出口数量);
                    出口物品总数 = (int)totalExport;

                    if (totalProductStock > 0)
                        出口百分比 = (double)(totalExport / totalProductStock * 100);
                    else
                        出口百分比 = 0.0;


                    // 出口收益
                    decimal totalRevenue = await context.出口订单s.SumAsync(e => e.收益);
                    出口收益 = totalRevenue;

                    更新系统日志("数据加载完成");
                }
            }
            catch (Exception ex)
            {
                更新系统日志($"数据加载时发生错误: {ex.Message}");
            }
        }
    }
}

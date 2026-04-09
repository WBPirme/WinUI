// ViewModels/PlanViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OGAS.Data;
using OGAS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OGAS.ViewModels
{
    public partial class PlanViewModel : ObservableObject
    {
        private readonly IDbContextFactory<OGASDbContext> _contextFactory;

        public PlanViewModel(IDbContextFactory<OGASDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await LoadAllDataAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"初始化数据失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        #region 属性

        // 生产计划管理
        [ObservableProperty]
        private ObservableCollection<生产计划> _生产计划集合 = new ObservableCollection<生产计划>();

        [ObservableProperty]
        private 生产计划 _选中生产计划;

        [ObservableProperty]
        private string _计划类型;

        [ObservableProperty]
        private int? _计划项目ID;

        [ObservableProperty]
        private 生产工艺? _选中工艺;

        [ObservableProperty]
        private 加工厂? _选中加工厂;

        [ObservableProperty]
        private int? _计划数量;

        [ObservableProperty]
        private DateTime? _计划开始日期;

        [ObservableProperty]
        private string _计划状态;

        [ObservableProperty]
        private string _计划备注;

        // 所有加工厂列表
        [ObservableProperty]
        private ObservableCollection<加工厂> _所有加工厂列表 = new ObservableCollection<加工厂>();

        // 过滤后的加工厂列表
        [ObservableProperty]
        private ObservableCollection<加工厂> _加工厂列表 = new ObservableCollection<加工厂>();

        #endregion

        #region 数据集合

        [ObservableProperty]
        private ObservableCollection<生产工艺> _工艺列表 = new ObservableCollection<生产工艺>();

        [ObservableProperty]
        private ObservableCollection<产品> _产品列表 = new ObservableCollection<产品>();

        [ObservableProperty]
        private ObservableCollection<材料> _材料列表 = new ObservableCollection<材料>();

        #endregion

        #region 加载数据

        [RelayCommand]
        private async Task LoadAllDataAsync()
        {
            await Task.WhenAll(
                加载工艺列表Async(),
                加载产品列表Async(),
                加载材料列表Async(),
                加载加工厂列表Async(),
                加载生产计划数据Async()
            );
        }

        // 加载工艺列表
        private async Task 加载工艺列表Async()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.生产工艺s.Include(p => p.产品).Include(p => p.材料).Include(p => p.生产设备).ToListAsync();
                工艺列表.Clear();
                foreach (var item in list)
                    工艺列表.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载工艺列表失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        // 加载产品列表
        private async Task 加载产品列表Async()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.产品s.ToListAsync();
                产品列表.Clear();
                foreach (var item in list)
                    产品列表.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载产品列表失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        // 加载材料列表
        private async Task 加载材料列表Async()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.材料s.ToListAsync();
                材料列表.Clear();
                foreach (var item in list)
                    材料列表.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载材料列表失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        // 加载加工厂列表
        private async Task 加载加工厂列表Async()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.加工厂s.ToListAsync();
                _所有加工厂列表.Clear();
                foreach (var item in list)
                    _所有加工厂列表.Add(item);

                // 初始时，显示所有加工厂
                await 更新加工厂列表Async();
            }
            catch (Exception ex)
            {
                ShowMessage($"加载加工厂列表失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        // 加载生产计划数据
        private async Task 加载生产计划数据Async()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.生产计划s
                    .Include(p => p.生产工艺)
                    .Include(p => p.加工厂)
                    .ToListAsync();
                生产计划集合.Clear();
                foreach (var plan in list)
                    生产计划集合.Add(plan);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载生产计划数据失败: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        #endregion

        #region 生产计划

        // 生产计划管理命令

        [RelayCommand]
        private async Task 保存生产计划Async()
        {
            // 验证
            if (string.IsNullOrEmpty(计划类型))
            {
                ShowMessage("请选择计划类型。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (选中工艺 == null)
            {
                ShowMessage("请选择生产工艺。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (选中加工厂 == null)
            {
                ShowMessage("请选择加工厂。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (!计划数量.HasValue || 计划数量 <= 0)
            {
                ShowMessage("请输入有效的计划数量（大于0）。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (!计划开始日期.HasValue)
            {
                ShowMessage("请选择计划开始日期。", "提示", MessageBoxImage.Warning);
                return;
            }

            using var context = _contextFactory.CreateDbContext();

            // 验证所选加工厂是否具备所需生产设备且设备数量 >=1
            var factory = await context.加工厂s
                .FirstOrDefaultAsync(f => f.加工厂ID == 选中加工厂.加工厂ID);

            if (factory == null)
            {
                ShowMessage("所选加工厂不存在。", "错误", MessageBoxImage.Error);
                return;
            }

            if (factory.生产设备ID != 选中工艺.生产设备ID || (factory.设备数量 ?? 0) < 1)
            {
                ShowMessage("所选加工厂不具备执行所选生产工艺所需的设备。", "验证失败", MessageBoxImage.Warning);
                return;
            }

            // 创建新的生产计划
            var newPlan = new 生产计划
            {
                计划类型 = 计划类型,
                工艺ID = 选中工艺.工艺ID,
                加工厂ID = 选中加工厂.加工厂ID,
                计划数量 = 计划数量.Value,
                计划开始日期 = 计划开始日期.Value,
                状态 = string.IsNullOrWhiteSpace(计划状态) ? "未开始" : 计划状态,
                备注 = string.IsNullOrWhiteSpace(计划备注) ? null : 计划备注
            };

            await ExecuteDatabaseOperationAsync(async () =>
            {
                context.生产计划s.Add(newPlan);
                await context.SaveChangesAsync();

                // 重新加载新保存的生产计划，包括导航属性
                var savedPlan = await context.生产计划s
                    .Include(p => p.生产工艺)
                    .Include(p => p.加工厂)
                    .FirstOrDefaultAsync(p => p.计划ID == newPlan.计划ID);

                if (savedPlan != null)
                {
                    生产计划集合.Add(savedPlan);
                }

                await 重置生产计划Async();
                ShowMessage("生产计划保存成功。");
            });

        }

        [RelayCommand]
        private async Task 更新生产计划Async()
        {
            if (选中生产计划 == null)
            {
                ShowMessage("请先选择要更新的生产计划。", "提示", MessageBoxImage.Information);
                return;
            }

            // 验证
            if (string.IsNullOrEmpty(计划类型))
            {
                ShowMessage("请选择计划类型。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (选中工艺 == null)
            {
                ShowMessage("请选择生产工艺。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (选中加工厂 == null)
            {
                ShowMessage("请选择加工厂。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (!计划数量.HasValue || 计划数量 <= 0)
            {
                ShowMessage("请输入有效的计划数量（大于0）。", "提示", MessageBoxImage.Warning);
                return;
            }

            if (!计划开始日期.HasValue)
            {
                ShowMessage("请输入有效的计划开始日期。", "提示", MessageBoxImage.Warning);
                return;
            }

            using var context = _contextFactory.CreateDbContext();

            // 验证所选加工厂是否具备所需生产设备且设备数量 >=1
            var factory = await context.加工厂s
                .FirstOrDefaultAsync(f => f.加工厂ID == 选中加工厂.加工厂ID);

            if (factory == null)
            {
                ShowMessage("所选加工厂不存在。", "错误", MessageBoxImage.Error);
                return;
            }

            if (factory.生产设备ID != 选中工艺.生产设备ID || (factory.设备数量 ?? 0) < 1)
            {
                ShowMessage("所选加工厂不具备执行所选生产工艺所需的设备。", "验证失败", MessageBoxImage.Warning);
                return;
            }

            // 更新选中的生产计划
            选中生产计划.计划类型 = 计划类型;
            选中生产计划.工艺ID = 选中工艺!.工艺ID;
            选中生产计划.加工厂ID = 选中加工厂!.加工厂ID;
            选中生产计划.计划数量 = 计划数量.Value;
            选中生产计划.计划开始日期 = 计划开始日期.Value;
            选中生产计划.状态 = string.IsNullOrWhiteSpace(计划状态) ? "未开始" : 计划状态;
            选中生产计划.备注 = string.IsNullOrWhiteSpace(计划备注) ? null : 计划备注;

            await ExecuteDatabaseOperationAsync(async () =>
            {
                context.生产计划s.Update(选中生产计划);
                await context.SaveChangesAsync();
                await 重置生产计划Async();
                ShowMessage("生产计划更新成功。");
            });
        }


        [RelayCommand]
        private async Task 删除生产计划Async()
        {
            if (选中生产计划 == null)
            {
                ShowMessage("请先选择要删除的生产计划。", "提示", MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"确定要删除生产计划 '{选中生产计划.计划ID}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await ExecuteDatabaseOperationAsync(async () =>
                {
                    using var context = _contextFactory.CreateDbContext();
                    context.生产计划s.Remove(选中生产计划);
                    await context.SaveChangesAsync();
                    生产计划集合.Remove(选中生产计划);
                    await 重置生产计划Async();
                    ShowMessage("生产计划删除成功。");
                });
            }
        }

        [RelayCommand]
        private async Task 重置生产计划Async()
        {
            计划类型 = null;
            计划项目ID = null;
            选中工艺 = null; // 重置工艺选择
            选中加工厂 = null; // 重置加工厂选择
            计划数量 = null;
            计划开始日期 = DateTime.Now;
            计划状态 = null;
            计划备注 = null;
            选中生产计划 = null;
            await 更新加工厂列表Async(); // 更新加工厂列表
            await 加载工艺列表Async();
            await 加载生产计划数据Async();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="caption">标题</param>
        /// <param name="icon">图标类型</param>
        private void ShowMessage(string message, string caption = "提示", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        /// <summary>
        /// 异步执行数据库操作并处理异常
        /// </summary>
        /// <param name="operation">要执行的异步操作</param>
        private async Task ExecuteDatabaseOperationAsync(Func<Task> operation)
        {
            try
            {
                await operation();
            }
            catch (DbUpdateException ex)
            {
                ShowMessage($"数据库操作失败: {ex.InnerException?.Message ?? ex.Message}", "错误", MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage($"发生未知错误: {ex.Message}", "错误", MessageBoxImage.Error);
            }
        }

        #endregion

        #region 部分方法

        partial void On选中工艺Changed(生产工艺 value)
        {
            // 刷新加工厂列表以应用过滤
            _ = 更新加工厂列表Async();

            // 如果当前选中的加工厂不在新的加工厂列表中，清空选择
            if (选中加工厂 != null && !加工厂列表.Contains(选中加工厂))
            {
                选中加工厂 = null;
            }
        }

        #endregion

        #region 过滤方法

        private async Task 更新加工厂列表Async()
        {
            加工厂列表.Clear();
            if (选中工艺 == null)
            {
                // 如果未选择生产工艺，显示所有加工厂
                foreach (var factory in _所有加工厂列表)
                    加工厂列表.Add(factory);
            }
            else
            {
                // 获取所选生产工艺所需的生产设备ID
                var requiredEquipmentId = 选中工艺.生产设备ID;

                // 筛选具备所需生产设备且设备数量 >=1 的加工厂
                var validFactories = _所有加工厂列表
                    .Where(f => f.生产设备ID == requiredEquipmentId && (f.设备数量 ?? 0) >= 1)
                    .ToList();

                foreach (var factory in validFactories)
                    加工厂列表.Add(factory);
            }
        }

        #endregion
    }
}

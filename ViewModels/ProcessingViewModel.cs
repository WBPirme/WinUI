using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using OGAS.Data;
using OGAS.Models;
using System.Windows.Markup;

namespace OGAS.ViewModels
{
    public partial class ProcessingViewModel : ObservableObject
    {
        private readonly OGASDbContextFactory _contextFactory;
        private CancellationTokenSource _cancellationTokenSource;

        private Dictionary<int, bool> _factoryBusyStatus = new Dictionary<int, bool>();

        // 初始化工厂忙碌状态
        private async Task InitializeFactoryStatusAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var factories = await context.加工厂s.ToListAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Factories.Clear();
                foreach (var factory in factories)
                {
                    Factories.Add(factory);
                    _factoryBusyStatus[factory.加工厂ID] = false; // 初始化为未忙碌
                }
            });
        }

        public ProcessingViewModel(OGASDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            ProductionProgressBars = new ObservableCollection<ProductionProgressBar>();
            InitializeAsync();
        }

        // ObservableCollection 属性
        [ObservableProperty]
        private ObservableCollection<生产计划> _生产计划集合 = new();

        [ObservableProperty]
        private ObservableCollection<加工厂> _factories = new();

        [ObservableProperty]
        private ObservableCollection<生产工艺> _processes = new();

        [ObservableProperty]
        private ObservableCollection<生产设备> _equipments = new();

        [ObservableProperty]
        private ObservableCollection<产品库存> _产品库存集合 = new();

        [ObservableProperty]
        private ObservableCollection<产品> _products = new();

        [ObservableProperty]
        private bool _isProcessing;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private decimal _totalProductionTime;

        // ObservableCollection 用于存储多个生产进度条
        [ObservableProperty]
        private ObservableCollection<ProductionProgressBar> _productionProgressBars;

        // 自动生产开关
        [ObservableProperty]
        private bool _isAutoProductionEnabled;

        // 暂停标志
        private bool _isPaused;
        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        // 自动生产开关变化时触发的方法
        partial void OnIsAutoProductionEnabledChanged(bool value)
        {
            Console.WriteLine($"Auto Production Enabled: {value}");
            if (value)  // 如果勾选框被选中
            {
                // 初始化 CancellationTokenSource
                _cancellationTokenSource = new CancellationTokenSource();
                // 自动开始生产任务计算
                _ = StartAutoProductionAsync(_cancellationTokenSource.Token);
            }
            else
            {
                // 取消生产任务
                _cancellationTokenSource?.Cancel();
                // 停止生产或重置处理状态（如果需要）
                IsProcessing = false;
                StatusMessage = "自动生产已停止";
            }
        }

        private async Task InitializeAsync()
        {
            StatusMessage = "准备就绪";
            IsProcessing = false;

            // 异步加载数据
            await LoadProductionPlansAsync();
            await LoadProductsAsync();
            await LoadProductStocksAsync();
            await InitializeFactoryStatusAsync();
        }

        private async Task LoadProductionPlansAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var productionPlans = await context.生产计划s
                .Where(p => p.状态 != "已完成")
                .Include(p => p.生产工艺)
                    .ThenInclude(proc => proc.产品)
                .Include(p => p.加工厂)
                .ToListAsync();

            // 切换到 UI 线程更新集合
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                生产计划集合.Clear();
                foreach (var plan in productionPlans)
                {
                    生产计划集合.Add(plan);
                }
            });
        }

        private async Task LoadProductsAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var products = await context.产品s.ToListAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            });
        }

        private async Task LoadProductStocksAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var productStocks = await context.产品库存s
                .Include(ps => ps.产品)
                .ToListAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                产品库存集合.Clear();
                foreach (var stock in productStocks)
                {
                    产品库存集合.Add(stock);
                }
            });
        }

        // ProcessingViewModel.cs (修改 StartAutoProductionAsync 方法)
        private async Task StartAutoProductionAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("自动生产任务启动");
                IsProcessing = true;
                StatusMessage = "正在计算生产任务...";

                // 获取所有未完成的生产计划，按优先级或其他逻辑排序
                var productionPlans = 生产计划集合.ToList();

                foreach (var productionPlan in productionPlans)
                {
                    // 检查材料是否充足
                    await CheckMaterialsAndCalculateProductionTime(productionPlan.计划ID);

                    // 获取加工厂ID
                    int factoryId = productionPlan.加工厂ID;

                    // 检查加工厂是否忙碌
                    if (_factoryBusyStatus.ContainsKey(factoryId) && _factoryBusyStatus[factoryId])
                    {
                        // 加工厂忙碌，生产计划进入等待状态
                        await HandleWaitingProductionAsync(productionPlan, factoryId, cancellationToken);
                    }
                    else
                    {
                        // 加工厂空闲，开始生产
                        _factoryBusyStatus[factoryId] = true;
                        var productionTask = Task.Run(async () =>
                        {
                            var productionResult = await CalculateProductionAsync(productionPlan, factoryId, cancellationToken);

                            // 标记生产计划为已完成
                            productionPlan.状态 = "已完成";
                            await CompleteProductionAsync(productionPlan.计划ID, (decimal)productionResult.总加工时间, factoryId, productionPlan.工艺ID, cancellationToken);

                            // 标记加工厂为未忙碌
                            _factoryBusyStatus[factoryId] = false;

                            // 检查是否有等待中的生产计划
                            await StartNextWaitingProductionAsync(factoryId, cancellationToken);
                        }, cancellationToken);

                        // 不等待任务完成，继续分配下一个生产计划
                    }
                }

                StatusMessage = "所有生产任务已分配";
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "自动生产已取消";
                Console.WriteLine("自动生产已取消");
            }
            catch (Exception ex)
            {
                StatusMessage = $"发生错误: {ex.Message}";
                Console.WriteLine($"异常信息：{ex}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        //等待生产
        private async Task HandleWaitingProductionAsync(生产计划 productionPlan, int factoryId, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            // 计算等待时间：假设等待时间为 预计生产完成时间
            // 这里可以根据需求调整等待时间的计算方式
            var estimatedProcessingTime = await EstimateProcessingTimeAsync(productionPlan, factoryId, context);

            // 总等待时间 = 预计生产完成时间 + 当前生产任务的剩余时间
            // 这里简化为使用预计生产时间
            decimal totalWaitTimeInSeconds = estimatedProcessingTime;

            // 创建等待中的生产进度条
            var progressBar = new ProductionProgressBar
            {
                ProductName = $"等待: {productionPlan.生产工艺?.产品?.名称}",
                CurrentProgress = 1.0, // 全满
                RemainingTime = TimeSpan.FromSeconds((double)totalWaitTimeInSeconds).ToString(@"hh\:mm\:ss"),
                Status = "等待中"
            };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ProductionProgressBars.Add(progressBar);
                Debug.WriteLine($"添加等待进度条: {progressBar.ProductName}");
            });

            // 记录等待中的生产计划
            // 可以使用一个队列来管理等待中的生产计划
            // 这里简化为直接处理
        }

        private async Task StartNextWaitingProductionAsync(int factoryId, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            // 获取下一个等待中的生产计划
            var nextProductionPlan = await context.生产计划s
                .Where(p => p.加工厂ID == factoryId && p.状态 == "等待")
                .FirstOrDefaultAsync(cancellationToken);

            if (nextProductionPlan != null)
            {
                // 标记加工厂为忙碌
                _factoryBusyStatus[factoryId] = true;

                // 更新生产计划状态为生产中
                nextProductionPlan.状态 = "生产中";
                await context.SaveChangesAsync(cancellationToken);

                // 创建绿色进度条
                var productionResult = await CalculateProductionAsync(nextProductionPlan, factoryId, cancellationToken);

                var progressBar = new ProductionProgressBar
                {
                    ProductName = $"产品名称: {nextProductionPlan.生产工艺?.产品?.名称}",
                    CurrentProgress = 0,
                    StartTime = DateTime.Now,
                    RemainingTime = TimeSpan.FromSeconds((double)productionResult.总加工时间).ToString(@"hh\:mm\:ss"),
                    Status = "生产中"
                };

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProductionProgressBars.Add(progressBar);
                    Debug.WriteLine($"添加生产进度条: {progressBar.ProductName}");
                });

                // 开始生产任务
                var productionTask = Task.Run(async () =>
                {
                    await UpdateProgressBarAsync(progressBar, (double)productionResult.总加工时间, nextProductionPlan, cancellationToken);

                    // 标记生产计划为已完成
                    nextProductionPlan.状态 = "已完成";
                    await CompleteProductionAsync(nextProductionPlan.计划ID, (decimal)productionResult.总加工时间, factoryId, nextProductionPlan.工艺ID, cancellationToken);

                    // 标记加工厂为未忙碌
                    _factoryBusyStatus[factoryId] = false;

                    // 从进度条集合中移除已完成的进度条
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ProductionProgressBars.Remove(progressBar);
                        Debug.WriteLine($"移除已完成的进度条: {progressBar.ProductName}");
                    });

                    // 检查是否有下一个等待中的生产计划
                    await StartNextWaitingProductionAsync(factoryId, cancellationToken);
                }, cancellationToken);
            }
        }

        private async Task<decimal> EstimateProcessingTimeAsync(生产计划 productionPlan, int factoryId, OGASDbContext context)
        {
            // 这里简化为根据生产计划计算预计加工时间
            decimal processTimeInSeconds = productionPlan.生产工艺?.标准加工时间 ?? throw new Exception("生产工艺未找到");
            var factory = await context.加工厂s.FirstOrDefaultAsync(f => f.加工厂ID == factoryId);
            if (factory == null)
            {
                throw new Exception("加工厂未找到");
            }

            var equipment = await context.生产设备s.FirstOrDefaultAsync(e => e.生产设备ID == factory.生产设备ID);
            if (equipment == null)
            {
                throw new Exception("生产设备未找到");
            }

            decimal equipmentCapacity = equipment.容量;
            decimal equipmentEfficiency = equipment.效率;

            decimal equipmentOutput = equipmentCapacity * equipmentEfficiency;
            decimal totalOutput = (decimal)(equipmentOutput * factory.设备数量);
            decimal totalProcessingTimeInSeconds = (processTimeInSeconds * productionPlan.计划数量) / totalOutput;

            decimal factoryEfficiency = factory.加工效率;
            totalOutput *= factoryEfficiency;

            if (totalProcessingTimeInSeconds <= 0)
            {
                throw new Exception("总加工时间计算结果无效");
            }

            return totalProcessingTimeInSeconds;
        }


        // 计算生产任务结果
        // ProcessingViewModel.cs (修改 CalculateProductionAsync 方法)
        public async Task<生产任务结果> CalculateProductionAsync(生产计划 productionPlan, int factoryId, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            // 获取工艺的标准加工时间（以秒为单位）
            decimal processTimeInSeconds = productionPlan.生产工艺?.标准加工时间 ?? throw new Exception("生产工艺未找到");

            // 获取生产设备ID和设备数量
            var factory = await context.加工厂s.FirstOrDefaultAsync(f => f.加工厂ID == factoryId, cancellationToken);
            if (factory == null)
            {
                throw new Exception("加工厂未找到");
            }

            var equipment = await context.生产设备s.FirstOrDefaultAsync(e => e.生产设备ID == factory.生产设备ID, cancellationToken);
            if (equipment == null)
            {
                throw new Exception("生产设备未找到");
            }

            decimal equipmentCapacity = equipment.容量;
            decimal equipmentEfficiency = equipment.效率;

            decimal equipmentOutput = equipmentCapacity * equipmentEfficiency;
            decimal totalOutput = (decimal)(equipmentOutput * factory.设备数量);

            decimal totalProcessingTimeInSeconds = (processTimeInSeconds * productionPlan.计划数量) / totalOutput;

            decimal factoryEfficiency = factory.加工效率;
            totalOutput *= factoryEfficiency;

            if (totalProcessingTimeInSeconds <= 0)
            {
                throw new Exception("总加工时间计算结果无效");
            }

            // 创建并添加生产进度条
            var progressBar = new ProductionProgressBar
            {
                ProductName = $"产品名称: {productionPlan.生产工艺?.产品?.名称}",
                CurrentProgress = 0,
                StartTime = DateTime.Now,
                RemainingTime = TimeSpan.FromSeconds((double)totalProcessingTimeInSeconds).ToString(@"hh\:mm\:ss"),
                Status = "生产中"
            };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ProductionProgressBars.Add(progressBar);
                Debug.WriteLine($"添加进度条: {progressBar.ProductName}");
            });

            // 更新进度条
            var productionResult = new 生产任务结果
            {
                产出数量 = totalOutput,
                总加工时间 = (double)totalProcessingTimeInSeconds // 以秒为单位
            };

            await UpdateProgressBarAsync(progressBar, (double)totalProcessingTimeInSeconds, productionPlan, cancellationToken);

            // 返回计算结果
            return productionResult;
        }


        // 选择加工厂
        public async Task<加工厂> SelectFactoryForProductionAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            // 查找一个有设备的加工厂
            var factory = await context.加工厂s
                .FirstOrDefaultAsync(f => f.设备数量 > 0);

            if (factory == null)
            {
                throw new Exception("没有可用的加工厂");
            }

            return factory;
        }

        // 更新进度条的方法
        public async Task UpdateProgressBarAsync(ProductionProgressBar progressBar, double totalProcessingTimeInSeconds, 生产计划 productionPlan, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"开始更新进度条: {progressBar.ProductName}, 总加工时间: {totalProcessingTimeInSeconds} 秒");
            // 使用 Stopwatch 进行精确计时
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (!cancellationToken.IsCancellationRequested)
            {
                // 检查是否暂停
                if (IsPaused)
                {
                    Debug.WriteLine($"生产任务已暂停: {progressBar.ProductName}");
                    await Task.Delay(500, cancellationToken); // 短暂等待后继续检查
                    continue;
                }

                // 计算已耗费的时间
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                // 计算当前进度比例
                double progress = elapsedSeconds / totalProcessingTimeInSeconds;

                // 计算剩余时间
                double remainingSeconds = Math.Max(totalProcessingTimeInSeconds - elapsedSeconds, 0);

                // 更新进度条和剩余时间
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.CurrentProgress = Math.Min(progress, 1.0); // 确保不超过1.0
                    progressBar.RemainingTime = TimeSpan.FromSeconds(remainingSeconds).ToString(@"hh\:mm\:ss");
                    Debug.WriteLine($"进度更新: {progressBar.ProductName}, 当前进度: {progressBar.CurrentProgress:P2}, 剩余时间: {progressBar.RemainingTime}");
                });

                // 如果进度达到或超过100%，结束循环
                if (progress >= 1.0)
                {
                    break;
                }

                // 每100毫秒更新一次
                try
                {
                    await Task.Delay(100, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // 任务被取消，退出循环
                    break;
                }
            }

            stopwatch.Stop();

            if (!cancellationToken.IsCancellationRequested)
            {
                // 生产完成，确保进度条为满并设置剩余时间为0
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.CurrentProgress = 1.0;
                    progressBar.RemainingTime = "00:00:00";
                    Debug.WriteLine($"生产完成: {progressBar.ProductName}, 进度条已满");
                });

                // 调用完成生产的方法
                await CompleteProductionAsync(productionPlan.计划ID, (decimal)totalProcessingTimeInSeconds, productionPlan.加工厂ID, productionPlan.工艺ID, cancellationToken);

                // 从生产进度条集合中移除已完成的进度条
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProductionProgressBars.Remove(progressBar);
                    Debug.WriteLine($"移除已完成的进度条: {progressBar.ProductName}");
                });
            }
        }

        // 创建生产记录并保存
        public async Task CompleteProductionAsync(long productionPlanId, decimal totalProcessingTimeInSeconds, int factoryId, long processId, CancellationToken cancellationToken)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // 获取生产计划信息
                var productionPlan = await context.生产计划s
                    .Include(p => p.生产工艺)
                    .Include(p => p.生产工艺.产品) // 确保加载产品信息
                    .FirstOrDefaultAsync(p => p.计划ID == productionPlanId, cancellationToken)
                    ?? throw new Exception("生产计划未找到");

                var totalOutput = productionPlan.计划数量;

                // 更新库存：减少材料库存（如果有材料ID）
                if (productionPlan.生产工艺.材料ID.HasValue)
                {
                    var materialId = productionPlan.生产工艺.材料ID.Value;
                    var materialStock = await context.材料库存s
                        .FirstOrDefaultAsync(s => s.材料ID == materialId, cancellationToken);

                    if (materialStock == null || materialStock.数量 < totalOutput)
                    {
                        throw new Exception("材料库存不足");
                    }

                    materialStock.数量 -= totalOutput;
                    materialStock.最后更新时间 = DateTime.Now;
                }

                // 更新库存：增加产出产品
                var outputProductId = productionPlan.生产工艺.产品ID;
                var outputProductStock = await context.产品库存s
                    .FirstOrDefaultAsync(s => s.产品ID == outputProductId, cancellationToken);

                if (outputProductStock != null)
                {
                    outputProductStock.数量 += totalOutput;
                    outputProductStock.最后更新时间 = DateTime.Now;
                    Debug.WriteLine($"更新产品库存: 产品ID {outputProductId}, 新数量: {outputProductStock.数量}");
                }
                else
                {
                    context.产品库存s.Add(new 产品库存
                    {
                        产品ID = outputProductId,
                        产品类型 = productionPlan.生产工艺.产品.产品类别, // 确保此值不为 NULL
                        计量单位 = productionPlan.生产工艺.产品.计量单位, // 确保此值不为 NULL
                        数量 = totalOutput,
                        最后更新时间 = DateTime.Now
                    });
                    Debug.WriteLine($"新增产品库存: 产品ID {outputProductId}, 数量: {totalOutput}");
                }

                // **检索工厂信息**
                var factory = await context.加工厂s
                    .FirstOrDefaultAsync(f => f.加工厂ID == factoryId, cancellationToken);

                if (factory == null)
                {
                    throw new Exception("指定的加工厂不存在");
                }

                // **确保生产设备ID存在于生产设备表中**
                var productionEquipment = await context.生产设备s
                    .FirstOrDefaultAsync(e => e.生产设备ID == factory.生产设备ID, cancellationToken);

                if (productionEquipment == null)
                {
                    throw new Exception("指定的生产设备不存在");
                }

                // 创建生产记录
                var productionRecord = new 生产记录
                {
                    计划ID = productionPlan.计划ID,
                    记录类型 = "生产",
                    加工厂ID = factoryId,
                    工艺ID = processId,
                    生产设备ID = (int)factory.生产设备ID,
                    实际开始日期 = productionPlan.计划开始日期,
                    实际结束日期 = DateTime.Now,
                    生产数量 = totalOutput,
                    生产时间 = totalProcessingTimeInSeconds,
                };

                context.生产记录s.Add(productionRecord);
                Debug.WriteLine($"创建生产记录: 生产计划ID {productionPlan.计划ID}, 生产数量: {totalOutput}");

                // 更新生产计划的状态为已完成
                productionPlan.状态 = "已完成";
                Debug.WriteLine($"更新生产计划状态为已完成: 生产计划ID {productionPlan.计划ID}");

                // 保存数据库中的所有变更
                await context.SaveChangesAsync(cancellationToken);
                Debug.WriteLine($"保存数据库变更成功: 生产计划ID {productionPlan.计划ID}");

                // 从生产计划集合中移除已完成的计划
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    生产计划集合.Remove(productionPlan);
                });

                Debug.WriteLine($"从生产计划集合中移除已完成的计划: 生产计划ID {productionPlan.计划ID}");
            }
            catch (DbUpdateException dbEx)
            {
                // 使用 MessageBox 显示异常信息
                string errorMessage = $"数据库更新异常: {dbEx.Message}\n" +
                                      $"内部异常: {dbEx.InnerException?.Message}";
                MessageBox.Show(errorMessage, "数据库更新错误", MessageBoxButton.OK, MessageBoxImage.Error);

                // 或者使用 Debug 输出
                Debug.WriteLine($"数据库更新异常: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Debug.WriteLine($"内部异常: {dbEx.InnerException.Message}");
                }

                throw; // 重新抛出异常以便上层处理
            }
            catch (Exception ex)
            {
                // 使用 MessageBox 显示异常信息
                string errorMessage = $"完成生产任务时发生错误: {ex.Message}";
                MessageBox.Show(errorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

                // 或者使用 Debug 输出
                Debug.WriteLine($"完成生产任务时发生错误: {ex.Message}");
                throw;
            }
        }

        // 检查库存
        public async Task CheckMaterialsAndCalculateProductionTime(long planId)
        {
            using var context = _contextFactory.CreateDbContext();
            var productionPlan = await context.生产计划s
                .Include(p => p.生产工艺)
                .FirstOrDefaultAsync(p => p.计划ID == planId)
                ?? throw new Exception("生产计划未找到");

            var totalOutput = productionPlan.计划数量;

            // 检查材料库存（如果有材料ID）
            if (productionPlan.生产工艺.材料ID.HasValue)
            {
                var materialId = productionPlan.生产工艺.材料ID.Value;
                var materialStock = await context.材料库存s
                    .FirstOrDefaultAsync(s => s.材料ID == materialId, cancellationToken: default);

                if (materialStock == null || materialStock.数量 < totalOutput)
                {
                    throw new Exception("材料库存不足");
                }
            }
        }

        // 生产任务结果
        public class 生产任务结果
        {
            public decimal 产出数量 { get; set; }
            public double 总加工时间 { get; set; } // 以秒为单位
        }

        // 暂停生产命令
        [RelayCommand]
        private void PauseProduction()
        {
            IsPaused = true;
            StatusMessage = "生产已暂停";
            Debug.WriteLine("生产任务已暂停");
        }

        // 恢复生产命令
        [RelayCommand]
        private void ResumeProduction()
        {
            IsPaused = false;
            StatusMessage = "生产已恢复";
            Debug.WriteLine("生产任务已恢复");
        }
    }
}

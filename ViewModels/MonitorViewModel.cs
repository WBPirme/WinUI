// ViewModels/MonitorViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OGAS.Data;
using OGAS.Models;
using OGAS.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OGAS.ViewModels
{
    public partial class MonitorViewModel : ObservableObject
    {
        private readonly IDbContextFactory<OGASDbContext> _contextFactory;
        private readonly INavigationService _navigationService;

        // 构造函数
        public MonitorViewModel(IDbContextFactory<OGASDbContext> contextFactory, INavigationService navigationService)
        {
            _contextFactory = contextFactory;
            _navigationService = navigationService;
            _ = 加载数据(); // 异步加载数据
        }

        #region Observable Properties

        // 产品库存集合
        [ObservableProperty]
        private ObservableCollection<产品库存> _productInventories;

        // 材料库存集合
        [ObservableProperty]
        private ObservableCollection<材料库存> _materialInventories;

        // 选中的产品库存项
        [ObservableProperty]
        private 产品库存 _selectedProductInventory;

        // 选中的材料库存项
        [ObservableProperty]
        private 材料库存 _selectedMaterialInventory;

        // 库存表单绑定属性
        [ObservableProperty]
        private string _productCategory;

        [ObservableProperty]
        private string _materialCategoryType;

        [ObservableProperty]
        private string _quantity;

        [ObservableProperty]
        private string _unit;

        [ObservableProperty]
        private string _remarks;

        // 名称相关
        [ObservableProperty]
        private ObservableCollection<NameOption> _nameOptions;

        [ObservableProperty]
        private int? _selectedName;

        // CheckBox 绑定属性
        [ObservableProperty]
        private bool _isSetMaterialSelected;

        [ObservableProperty]
        private bool _isSetProductSelected;

        partial void OnIsSetMaterialSelectedChanged(bool value)
        {
            if (value)
            {
                IsSetProductSelected = false;
                _ = LoadNameOptionsAsync();
            }
            else
            {
                NameOptions = new ObservableCollection<NameOption>();
            }
        }

        partial void OnIsSetProductSelectedChanged(bool value)
        {
            if (value)
            {
                IsSetMaterialSelected = false;
                _ = LoadNameOptionsAsync();
            }
            else
            {
                NameOptions = new ObservableCollection<NameOption>();
            }
        }

        #endregion

        #region 出口订单相关属性

        // 出口订单集合
        [ObservableProperty]
        private ObservableCollection<出口订单> _exportOrders;

        // 产品集合，用于选择产品
        [ObservableProperty]
        private ObservableCollection<产品> _products;

        // 选中的出口订单
        [ObservableProperty]
        private 出口订单 _selectedExportOrder;

        // 选中的产品ID
        [ObservableProperty]
        private int _selectedProductID;

        // 出口数量
        [ObservableProperty]
        private decimal _exportQuantity;

        // 收益，自动计算
        [ObservableProperty]
        private decimal _revenue;

        // 订单日期，自动获取
        [ObservableProperty]
        private DateTime _orderDate = DateTime.Now;

        #endregion

        #region 数据加载

        // 异步加载所有数据
        private async Task 加载数据()
        {
            await Task.WhenAll(
                LoadProductInventoriesAsync(),
                LoadMaterialInventoriesAsync(),
                LoadNameOptionsAsync(),
                LoadExportOrdersAsync()
            );
        }

        private async Task LoadProductInventoriesAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var products = await context.产品库存s
                        .Include(p => p.产品) // 包含产品导航属性
                        .ToListAsync();
                    ProductInventories = new ObservableCollection<产品库存>(products);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载产品库存时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadMaterialInventoriesAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var materials = await context.材料库存s
                        .Include(m => m.材料) // 包含材料导航属性
                        .ToListAsync();
                    MaterialInventories = new ObservableCollection<材料库存>(materials);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载材料库存时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadNameOptionsAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    if (IsSetMaterialSelected)
                    {
                        var materials = await context.材料s.ToListAsync();
                        NameOptions = new ObservableCollection<NameOption>(
                            materials.Select(m => new NameOption { ID = m.材料ID, Name = m.名称 })
                        );
                    }
                    else if (IsSetProductSelected)
                    {
                        var products = await context.产品s.ToListAsync();
                        NameOptions = new ObservableCollection<NameOption>(
                            products.Select(p => new NameOption { ID = p.产品ID, Name = p.名称 })
                        );
                    }
                    else
                    {
                        NameOptions = new ObservableCollection<NameOption>();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载名称选项时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadExportOrdersAsync()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var orders = await context.出口订单s
                        .Include(o => o.产品)
                        .OrderByDescending(o => o.订单日期)
                        .ToListAsync();
                    ExportOrders = new ObservableCollection<出口订单>(orders);

                    var products = await context.产品s.ToListAsync();
                    Products = new ObservableCollection<产品>(products);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载出口订单时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Commands

        // 保存命令
        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedName == null)
            {
                MessageBox.Show("请选择一个名称。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductCategory) ||
                string.IsNullOrWhiteSpace(MaterialCategoryType) ||
                string.IsNullOrWhiteSpace(Quantity) ||
                string.IsNullOrWhiteSpace(Unit))
            {
                MessageBox.Show("请填写所有必填字段。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(Quantity, out decimal parsedQuantity) || parsedQuantity < 0)
            {
                MessageBox.Show("数量必须为有效的非负数。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = _contextFactory.CreateDbContext())
            {
                if (IsSetMaterialSelected)
                {
                    // 添加材料库存
                    var newMaterialInventory = new 材料库存
                    {
                        材料ID = SelectedName,
                        材料类型 = MaterialCategoryType,
                        数量 = parsedQuantity,
                        计量单位 = Unit,
                        备注 = Remarks,
                        最后更新时间 = DateTime.Now
                    };

                    context.材料库存s.Add(newMaterialInventory);
                }
                else if (IsSetProductSelected)
                {
                    // 添加产品库存
                    var newProductInventory = new 产品库存
                    {
                        产品ID = SelectedName,
                        产品类型 = ProductCategory,
                        数量 = parsedQuantity,
                        计量单位 = Unit,
                        备注 = Remarks,
                        最后更新时间 = DateTime.Now
                    };

                    context.产品库存s.Add(newProductInventory);
                }
                else
                {
                    MessageBox.Show("请选择设置材料或设置产品。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await context.SaveChangesAsync();
            }

            await 加载数据();
            ResetFields();
        }

        // 更新命令
        [RelayCommand]
        private async Task UpdateAsync()
        {
            if (IsSetMaterialSelected && SelectedMaterialInventory == null ||
                IsSetProductSelected && SelectedProductInventory == null)
            {
                MessageBox.Show("请先选择要更新的库存项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (SelectedName == null)
            {
                MessageBox.Show("请选择一个名称。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductCategory) ||
                string.IsNullOrWhiteSpace(MaterialCategoryType) ||
                string.IsNullOrWhiteSpace(Quantity) ||
                string.IsNullOrWhiteSpace(Unit))
            {
                MessageBox.Show("请填写所有必填字段。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(Quantity, out decimal parsedQuantity) || parsedQuantity < 0)
            {
                MessageBox.Show("数量必须为有效的非负数。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = _contextFactory.CreateDbContext())
            {
                if (IsSetMaterialSelected)
                {
                    var materialInventory = await context.材料库存s
                        .FirstOrDefaultAsync(m => m.材料库存ID == SelectedMaterialInventory.材料库存ID);

                    if (materialInventory != null)
                    {
                        materialInventory.材料ID = SelectedName;
                        materialInventory.材料类型 = MaterialCategoryType;
                        materialInventory.数量 = parsedQuantity;
                        materialInventory.计量单位 = Unit;
                        materialInventory.备注 = Remarks;
                        materialInventory.最后更新时间 = DateTime.Now;

                        context.材料库存s.Update(materialInventory);
                    }
                }
                else if (IsSetProductSelected)
                {
                    var productInventory = await context.产品库存s
                        .FirstOrDefaultAsync(p => p.产品库存ID == SelectedProductInventory.产品库存ID);

                    if (productInventory != null)
                    {
                        productInventory.产品ID = SelectedName;
                        productInventory.产品类型 = ProductCategory;
                        productInventory.数量 = parsedQuantity;
                        productInventory.计量单位 = Unit;
                        productInventory.备注 = Remarks;
                        productInventory.最后更新时间 = DateTime.Now;

                        context.产品库存s.Update(productInventory);
                    }
                }

                await context.SaveChangesAsync();
            }

            await 加载数据();
            ResetFields();
        }

        // 删除命令
        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (IsSetMaterialSelected && SelectedMaterialInventory == null ||
                IsSetProductSelected && SelectedProductInventory == null)
            {
                MessageBox.Show("请先选择要删除的库存项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("确定要删除选中的库存项吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            using (var context = _contextFactory.CreateDbContext())
            {
                if (IsSetMaterialSelected)
                {
                    var materialInventory = await context.材料库存s.FindAsync(SelectedMaterialInventory.材料库存ID);
                    if (materialInventory != null)
                    {
                        context.材料库存s.Remove(materialInventory);
                    }
                }
                else if (IsSetProductSelected)
                {
                    var productInventory = await context.产品库存s.FindAsync(SelectedProductInventory.产品库存ID);
                    if (productInventory != null)
                    {
                        context.产品库存s.Remove(productInventory);
                    }
                }

                await context.SaveChangesAsync();
            }

            await 加载数据();
            ResetFields();
        }

        // 重置命令
        [RelayCommand]
        private void Reset()
        {
            ResetFields();
        }

        private void ResetFields()
        {
            SelectedName = null;
            ProductCategory = string.Empty;
            MaterialCategoryType = string.Empty;
            Quantity = string.Empty;
            Unit = string.Empty;
            Remarks = string.Empty;
            SelectedProductInventory = null;
            SelectedMaterialInventory = null;
        }

        #endregion

        #region 出口订单命令

        // 创建出口订单命令
        [RelayCommand]
        private async Task CreateExportOrderAsync()
        {
            if (!ValidateExportOrder(out decimal calculatedRevenue)) return;

            var newOrder = new 出口订单
            {
                产品ID = SelectedProductID,
                出口数量 = ExportQuantity,
                收益 = calculatedRevenue,
                订单日期 = OrderDate
            };

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    // 检查库存是否足够
                    var productInventory = await context.产品库存s
                        .FirstOrDefaultAsync(pi => pi.产品ID == SelectedProductID);

                    if (productInventory == null)
                    {
                        ShowError("未找到所选产品的库存信息。");
                        return;
                    }

                    if (productInventory.数量 < ExportQuantity)
                    {
                        ShowError("库存数量不足以完成此次出口。");
                        return;
                    }

                    // 减少库存
                    productInventory.数量 -= ExportQuantity;
                    context.产品库存s.Update(productInventory);

                    // 添加出口订单
                    context.出口订单s.Add(newOrder);

                    await context.SaveChangesAsync();
                }

                await LoadExportOrdersAsync();
                ResetExportOrderFields();
                MessageBox.Show("出口订单已成功创建。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建出口订单时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 重置出口订单输入字段命令
        [RelayCommand]
        private void ResetExportOrder()
        {
            ResetExportOrderFields();
        }

        // 重置出口订单输入字段的方法
        private void ResetExportOrderFields()
        {
            SelectedProductID = 0;
            ExportQuantity = 0;
            Revenue = 0;
            OrderDate = DateTime.Now;
            SelectedExportOrder = null;
        }

        #endregion

        #region 出口订单辅助方法

        // 验证出口订单输入
        private bool ValidateExportOrder(out decimal calculatedRevenue)
        {
            calculatedRevenue = 0;

            if (SelectedProductID <= 0)
            {
                ShowError("请选择一个产品。");
                return false;
            }

            if (ExportQuantity <= 0)
            {
                ShowError("出口数量必须大于 0。");
                return false;
            }

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var product = context.产品s
                        .FirstOrDefault(p => p.产品ID == SelectedProductID);

                    if (product == null)
                    {
                        ShowError("未找到所选产品。");
                        return false;
                    }

                    calculatedRevenue = ExportQuantity * product.每单位价格;
                }
            }
            catch
            {
                ShowError("计算收益时出错。");
                return false;
            }

            return true;
        }

        // 显示错误消息
        private void ShowError(string message)
        {
            MessageBox.Show(message, "验证错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region 收益计算

        // 当出口数量变化时计算收益
        partial void OnExportQuantityChanged(decimal value)
        {
            CalculateRevenue();
        }

        // 当选中的产品ID变化时计算收益
        partial void OnSelectedProductIDChanged(int value)
        {
            CalculateRevenue();
        }

        // 计算收益 = 出口数量 * 产品单价
        private async void CalculateRevenue()
        {
            if (SelectedProductID > 0 && ExportQuantity > 0)
            {
                try
                {
                    using (var context = _contextFactory.CreateDbContext())
                    {
                        var product = await context.产品s
                            .FirstOrDefaultAsync(p => p.产品ID == SelectedProductID);

                        if (product != null)
                        {
                            Revenue = ExportQuantity * product.每单位价格;
                        }
                        else
                        {
                            Revenue = 0;
                        }
                    }
                }
                catch
                {
                    Revenue = 0;
                }
            }
            else
            {
                Revenue = 0;
            }
        }

        #endregion

        #region 选框

        // 当“设置材料”被勾选时调用
        private void CheckBox_SetMaterial_Checked(object sender, RoutedEventArgs e)
        {
            if (IsSetMaterialSelected)
            {
                IsSetProductSelected = false;
                _ = LoadNameOptionsAsync();
            }
        }

        // 当“设置产品”被勾选时调用
        private void CheckBox_SetProduct_Checked(object sender, RoutedEventArgs e)
        {
            if (IsSetProductSelected)
            {
                IsSetMaterialSelected = false;
                _ = LoadNameOptionsAsync();
            }
        }

        #endregion
    }

    // 辅助类，用于 ComboBox 的名称选项
    public class NameOption
    {
        public int? ID { get; set; }
        public string Name { get; set; }
    }
}

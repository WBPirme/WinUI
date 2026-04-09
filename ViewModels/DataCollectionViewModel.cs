// ViewModels/DataCollectionViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OGAS.Data;
using OGAS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Input;

namespace OGAS.ViewModels
{
    public partial class DataCollectionViewModel : ObservableObject
    {
        private readonly IDbContextFactory<OGASDbContext> _contextFactory;

        public DataCollectionViewModel(IDbContextFactory<OGASDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            InitializeAsync();

        }

        private async void InitializeAsync()
        {
            try
            {
                await 加载数据();
            }
            catch (Exception ex)
            {
                ShowMessage($"初始化数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region 通用属性

        [ObservableProperty]
        private ObservableCollection<产品> products = new();

        [ObservableProperty]
        private ObservableCollection<材料> materials = new();

        [ObservableProperty]
        private ObservableCollection<加工厂> factories = new();

        [ObservableProperty]
        private ObservableCollection<生产工艺> processes = new();

        [ObservableProperty]
        private ObservableCollection<生产设备> equipments = new();

        #endregion

        #region 选中项属性

        [ObservableProperty]
        private 产品 selectedProduct;

        [ObservableProperty]
        private 材料 selectedMaterial;

        [ObservableProperty]
        private 加工厂 selectedFactory;

        [ObservableProperty]
        private 生产工艺 selectedProcess;

        [ObservableProperty]
        private 生产设备 selectedEquipment;

        [ObservableProperty]
        private bool _is设置材料选中 = true; // 默认选中
        [ObservableProperty]
        private bool _is设置产品选中 = true; // 默认选中

        #endregion

        #region 产品管理属性

        [ObservableProperty]
        private string productName;

        [ObservableProperty]
        private string productCategory;

        [ObservableProperty]
        private string productDescription;

        [ObservableProperty]
        private string productUnit;

        [ObservableProperty]
        private string productPrice;

        #endregion

        #region 材料管理属性

        [ObservableProperty]
        private string materialName;

        [ObservableProperty]
        private string materialCategory;

        [ObservableProperty]
        private string materialDescription;

        [ObservableProperty]
        private string materialUnit;

        [ObservableProperty]
        private string materialPrice;

        #endregion

        #region 加工厂管理属性

        [ObservableProperty]
        private string factoryName;

        [ObservableProperty]
        private string factoryLocation;

        [ObservableProperty]
        private string factoryDescription;

        [ObservableProperty]
        private string factoryContact;

        [ObservableProperty]
        private string factoryEfficiency;

        [ObservableProperty]
        private string factoryEquipmentCount;

        [ObservableProperty]
        private 生产设备 factorySelectedEquipment;

        #endregion

        #region 生产工艺管理属性

        [ObservableProperty]
        private string processName;

        [ObservableProperty]
        private string processRemarks;

        [ObservableProperty]
        private string processQualityStandard;

        [ObservableProperty]
        private string processSafetyGuidelines;

        [ObservableProperty]
        private string processStandardTime;

        [ObservableProperty]
        private 产品 processSelectedProduct;

        [ObservableProperty]
        private 材料 processSelectedMaterial;

        [ObservableProperty]
        private 生产设备 processSelectedEquipment;

        [ObservableProperty]
        private ObservableCollection<产品> processProducts = new();

        [ObservableProperty]
        private ObservableCollection<材料> processMaterials = new();

        [ObservableProperty]
        private ObservableCollection<生产设备> processEquipments = new();

        #endregion

        #region 生产设备管理属性

        [ObservableProperty]
        private string equipmentName;

        [ObservableProperty]
        private string equipmentType;

        [ObservableProperty]
        private string equipmentDescription;

        [ObservableProperty]
        private string equipmentCapacity;

        [ObservableProperty]
        private string equipmentEfficiency;

        #endregion

        #region 加载数据命令

        [RelayCommand]
        private async Task 加载数据()
        {
            await Task.WhenAll(
                加载产品(),
                加载材料(),
                加载加工厂(),
                加载生产工艺(),
                加载生产设备(),
                加载生产工艺相关()
            );
        }

        private async Task 加载产品()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.产品s.ToListAsync();
                Products.Clear();
                foreach (var item in list)
                    Products.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载产品列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 加载材料()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.材料s.ToListAsync();
                Materials.Clear();
                foreach (var item in list)
                    Materials.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载材料列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 加载加工厂()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.加工厂s.Include(f => f.生产设备).ToListAsync();
                Factories.Clear();
                foreach (var item in list)
                    Factories.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载加工厂列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 加载生产工艺()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.生产工艺s
                    .Include(p => p.产品)
                    .Include(p => p.材料)
                    .Include(p => p.生产设备)
                    .ToListAsync();
                Processes.Clear();
                foreach (var item in list)
                    Processes.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载生产工艺列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 加载生产设备()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var list = await context.生产设备s.ToListAsync();
                Equipments.Clear();
                foreach (var item in list)
                    Equipments.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载生产设备列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 加载生产工艺相关()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var products = await context.产品s.ToListAsync();
                ProcessProducts.Clear();
                foreach (var item in products)
                    ProcessProducts.Add(item);

                var materials = await context.材料s.ToListAsync();
                ProcessMaterials.Clear();
                foreach (var item in materials)
                    ProcessMaterials.Add(item);

                var equipments = await context.生产设备s.ToListAsync();
                ProcessEquipments.Clear();
                foreach (var item in equipments)
                    ProcessEquipments.Add(item);
            }
            catch (Exception ex)
            {
                ShowMessage($"加载生产工艺相关数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 保存功能
        [RelayCommand]
        private async Task 保存Async()
        {
            if (Is设置产品选中 && Is设置材料选中)
            {
                if (string.IsNullOrWhiteSpace(ProductCategory) || string.IsNullOrWhiteSpace(MaterialCategory))
                {
                    MessageBox.Show("当产品和材料都被选中时，产品类别和材料类别不能为空。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newProduct = new 产品
                {
                    名称 = ProductName,
                    产品类别 = ProductCategory,
                    描述 = ProductDescription,
                    计量单位 = ProductUnit,
                    每单位价格 = decimal.TryParse(ProductPrice, out decimal price) ? price : 0m
                };

                var newMaterial = new 材料
                {
                    名称 = ProductName,  // 产品和材料名称相同
                    材料类别 = MaterialCategory,  // 类别不同
                    描述 = ProductDescription,
                    计量单位 = ProductUnit,
                    每单位价格 = decimal.TryParse(ProductPrice, out decimal materialPrice) ? materialPrice : 0m
                };

                // 校验输入
                if (string.IsNullOrWhiteSpace(newProduct.名称) || newProduct.每单位价格 == 0m)
                {
                    MessageBox.Show("名称和每单位价格为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    using (var context = _contextFactory.CreateDbContext())
                    {
                        var productExists = await context.产品s.AnyAsync(p => p.名称 == newProduct.名称);
                        if (productExists)
                        {
                            MessageBox.Show("产品名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var materialExists = await context.材料s.AnyAsync(m => m.名称 == newMaterial.名称);
                        if (materialExists)
                        {
                            MessageBox.Show("材料名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // 保存到数据库
                        context.产品s.Add(newProduct);
                        context.材料s.Add(newMaterial);
                        await context.SaveChangesAsync();

                        // 更新UI
                        Products.Add(newProduct);
                        Materials.Add(newMaterial);

                        // 清空输入框
                        重置Async();

                        MessageBox.Show("产品和材料保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (Is设置产品选中)
            {
                await 保存产品Async();
            }
            else if (Is设置材料选中)
            {
                await 保存材料Async();
            }
        }

        private async Task 保存产品Async()
        {
            var newProduct = new 产品
            {
                名称 = ProductName,
                产品类别 = ProductCategory,
                描述 = ProductDescription,
                计量单位 = ProductUnit,
                每单位价格 = decimal.TryParse(ProductPrice, out decimal price) ? price : 0m
            };

            if (string.IsNullOrWhiteSpace(newProduct.名称) || newProduct.每单位价格 == 0m)
            {
                MessageBox.Show("名称和每单位价格为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var productExists = await context.产品s.AnyAsync(p => p.名称 == newProduct.名称);
                    if (productExists)
                    {
                        MessageBox.Show("产品名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    context.产品s.Add(newProduct);
                    await context.SaveChangesAsync();

                    Products.Add(newProduct);
                    重置Async();

                    MessageBox.Show("产品保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存产品失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 保存材料Async()
        {
            var newMaterial = new 材料
            {
                名称 = ProductName,
                材料类别 = MaterialCategory,
                描述 = ProductDescription,
                计量单位 = ProductUnit,
                每单位价格 = decimal.TryParse(ProductPrice, out decimal materialPrice) ? materialPrice : 0m
            };

            if (string.IsNullOrWhiteSpace(newMaterial.名称) || newMaterial.每单位价格 == 0m)
            {
                MessageBox.Show("名称和每单位价格为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var materialExists = await context.材料s.AnyAsync(m => m.名称 == newMaterial.名称);
                    if (materialExists)
                    {
                        MessageBox.Show("材料名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    context.材料s.Add(newMaterial);
                    await context.SaveChangesAsync();

                    Materials.Add(newMaterial);
                    重置Async();

                    MessageBox.Show("材料保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存材料失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 更新功能
        [RelayCommand]
        private async Task 更新Async()
        {
            if (SelectedProduct == null && SelectedMaterial == null)
            {
                MessageBox.Show("请先选择要更新的产品或材料。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Is设置产品选中 && Is设置材料选中)
            {
                if (string.IsNullOrWhiteSpace(ProductCategory) || string.IsNullOrWhiteSpace(MaterialCategory))
                {
                    MessageBox.Show("当产品和材料都被选中时，产品类别和材料类别不能为空。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // 更新产品和材料
                SelectedProduct.名称 = ProductName;
                SelectedProduct.产品类别 = ProductCategory;
                SelectedProduct.描述 = ProductDescription;
                SelectedProduct.计量单位 = ProductUnit;
                SelectedProduct.每单位价格 = decimal.TryParse(ProductPrice, out decimal price) ? price : 0m;

                SelectedMaterial.名称 = ProductName;
                SelectedMaterial.材料类别 = MaterialCategory;
                SelectedMaterial.描述 = ProductDescription;
                SelectedMaterial.计量单位 = ProductUnit;
                SelectedMaterial.每单位价格 = decimal.TryParse(ProductPrice, out decimal materialPrice) ? materialPrice : 0m;

                try
                {
                    using (var context = _contextFactory.CreateDbContext())
                    {
                        context.产品s.Update(SelectedProduct);
                        context.材料s.Update(SelectedMaterial);
                        await context.SaveChangesAsync();
                        await 重置Async();
                        MessageBox.Show("产品和材料更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"更新失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (Is设置产品选中)
            {
                await 更新产品Async();
            }
            else if (Is设置材料选中)
            {
                await 更新材料Async();
            }
        }

        private async Task 更新产品Async()
        {
            if (SelectedProduct == null) return;

            SelectedProduct.名称 = ProductName;
            SelectedProduct.产品类别 = ProductCategory;
            SelectedProduct.描述 = ProductDescription;
            SelectedProduct.计量单位 = ProductUnit;
            SelectedProduct.每单位价格 = decimal.TryParse(ProductPrice, out decimal price) ? price : 0m;

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    context.产品s.Update(SelectedProduct);
                    await context.SaveChangesAsync();
                    await 重置Async();
                    MessageBox.Show("产品更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新产品失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task 更新材料Async()
        {
            if (SelectedMaterial == null) return;

            SelectedMaterial.名称 = ProductName;
            SelectedMaterial.材料类别 = MaterialCategory;
            SelectedMaterial.描述 = ProductDescription;
            SelectedMaterial.计量单位 = ProductUnit;
            SelectedMaterial.每单位价格 = decimal.TryParse(ProductPrice, out decimal materialPrice) ? materialPrice : 0m;

            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    context.材料s.Update(SelectedMaterial);
                    await context.SaveChangesAsync();
                    await 重置Async();
                    MessageBox.Show("材料更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新材料失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 删除功能
        [RelayCommand]
        private async Task 删除Async()
        {
            if (SelectedProduct == null && SelectedMaterial == null)
            {
                MessageBox.Show("请先选择要删除的产品或材料。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("确定删除选中的产品或材料吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // 开始事务，确保操作的原子性
                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    // 如果选择了产品，检查是否被引用
                    if (SelectedProduct != null)
                    {
                        bool isProductReferenced = await IsProductReferencedAsync(SelectedProduct.产品ID);
                        if (isProductReferenced)
                        {
                            MessageBox.Show("无法删除选中的产品，因为存在相关的出口订单或产品库存。请先删除相关引用。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // 删除产品
                        // 确保实体被上下文追踪
                        if (context.Entry(SelectedProduct).State == EntityState.Detached)
                        {
                            context.产品s.Attach(SelectedProduct);
                        }
                        context.产品s.Remove(SelectedProduct);
                    }

                    // 如果选择了材料，检查是否被引用
                    if (SelectedMaterial != null)
                    {
                        bool isMaterialReferenced = await IsMaterialReferencedAsync(SelectedMaterial.材料ID);
                        if (isMaterialReferenced)
                        {
                            MessageBox.Show("无法删除选中的材料，因为存在相关的出口订单或材料库存。请先删除相关引用。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // 删除材料
                        // 确保实体被上下文追踪
                        if (context.Entry(SelectedMaterial).State == EntityState.Detached)
                        {
                            context.材料s.Attach(SelectedMaterial);
                        }
                        context.材料s.Remove(SelectedMaterial);
                    }

                    // 保存更改
                    await context.SaveChangesAsync();

                    // 提交事务
                    await transaction.CommitAsync();

                    // 从本地集合中移除
                    if (SelectedProduct != null)
                    {
                        Products.Remove(SelectedProduct);
                    }

                    if (SelectedMaterial != null)
                    {
                        Materials.Remove(SelectedMaterial);
                    }

                    MessageBox.Show("删除成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    // 出现异常时回滚事务
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                // 处理数据库更新异常，显示内部异常信息
                string errorMessage = $"删除失败: {dbEx.Message}";
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"\n详细信息: {dbEx.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "数据库错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> IsProductReferencedAsync(int productId)
        {
            using var context = _contextFactory.CreateDbContext();

            // 检查出口订单是否引用了该产品
            bool isReferencedInExportOrders = await context.出口订单s
                .AnyAsync(o => o.产品ID == productId);

            // 检查产品库存是否引用了该产品
            bool isReferencedInProductInventories = await context.产品库存s
                .AnyAsync(pi => pi.产品ID == productId);

            return isReferencedInExportOrders || isReferencedInProductInventories;
        }

        private async Task<bool> IsMaterialReferencedAsync(int materialId)
        {
            using var context = _contextFactory.CreateDbContext();

            // 检查材料库存是否引用了该材料
            bool isReferencedInMaterialInventories = await context.材料库存s
                .AnyAsync(mi => mi.材料ID == materialId);

            return isReferencedInMaterialInventories;
        }

        #endregion

        #region 重置功能
        [RelayCommand]
        private async Task 重置Async()
        {
            ProductName = string.Empty;
            ProductCategory = string.Empty;
            ProductDescription = string.Empty;
            ProductUnit = string.Empty;
            ProductPrice = string.Empty;
            MaterialCategory = string.Empty;
            SelectedProduct = null;
            SelectedMaterial = null;
            await 加载产品();
            await 加载材料();
        }

        #endregion

        #region 加工厂管理命令

        [RelayCommand]
        private async Task 保存加工厂Async()
        {
            var newFactory = new 加工厂
            {
                名称 = FactoryName,
                位置 = FactoryLocation,
                联系人信息 = FactoryContact,
                加工效率 = decimal.TryParse(FactoryEfficiency, out decimal efficiency) ? efficiency : 0,
                设备数量 = int.TryParse(FactoryEquipmentCount, out int qty) ? qty : (int?)null,
                生产设备ID = FactorySelectedEquipment?.生产设备ID ?? 0
            };

            // 验证输入
            if (string.IsNullOrWhiteSpace(newFactory.名称))
            {
                ShowMessage("名称为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (newFactory.设备数量 <= 0)
            {
                ShowMessage("设备数量必须大于 0。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newFactory.加工效率 <= 0.00m || newFactory.加工效率 > 1.00m)
            {
                ShowMessage("加工效率必须在 0 到 1 之间。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.加工厂s.AnyAsync(f => f.名称 == newFactory.名称);
                if (exists)
                {
                    ShowMessage("加工厂名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.加工厂s.Add(newFactory);
                await context.SaveChangesAsync();
                Factories.Add(newFactory);
                await 重置加工厂Async();
                ShowMessage("加工厂保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"保存加工厂失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 更新加工厂Async()
        {
            if (SelectedFactory == null)
            {
                ShowMessage("请先选择要更新的加工厂。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 更新属性
            SelectedFactory.名称 = FactoryName;
            SelectedFactory.位置 = FactoryLocation;
            SelectedFactory.联系人信息 = FactoryContact;
            SelectedFactory.加工效率 = decimal.TryParse(FactoryEfficiency, out decimal efficiency) ? efficiency : 0;
            SelectedFactory.设备数量 = int.TryParse(FactoryEquipmentCount, out int qty) ? qty : (int?)null;
            SelectedFactory.生产设备ID = FactorySelectedEquipment?.生产设备ID ?? 0;

            // 验证输入
            if (string.IsNullOrWhiteSpace(SelectedFactory.名称))
            {
                ShowMessage("名称为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedFactory.设备数量 <= 0)
            {
                ShowMessage("设备数量必须大于 0。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedFactory.加工效率 <= 0.00m || SelectedFactory.加工效率 > 1.00m)
            {
                ShowMessage("加工效率必须在 0 到 1 之间。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.加工厂s.AnyAsync(f => f.名称 == SelectedFactory.名称 && f.加工厂ID != SelectedFactory.加工厂ID);
                if (exists)
                {
                    ShowMessage("加工厂名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.加工厂s.Update(SelectedFactory);
                await context.SaveChangesAsync();
                await 重置加工厂Async();
                ShowMessage("加工厂更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"更新加工厂失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 删除加工厂Async()
        {
            if (SelectedFactory == null)
            {
                ShowMessage("请先选择要删除的加工厂。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"确定要删除加工厂 '{SelectedFactory.名称}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // 开始事务，确保操作的原子性
                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    // 检查加工厂是否被引用
                    bool isFactoryReferenced = await IsFactoryReferencedAsync(SelectedFactory.加工厂ID);
                    if (isFactoryReferenced)
                    {
                        string references = await GetFactoryReferencesAsync(SelectedFactory.加工厂ID);
                        ShowMessage($"无法删除选中的加工厂，因为存在以下引用：{references}。请先删除相关引用。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 删除加工厂
                    // 确保实体被上下文追踪
                    var factoryEntry = context.加工厂s.Local.FirstOrDefault(f => f.加工厂ID == SelectedFactory.加工厂ID);
                    if (factoryEntry == null)
                    {
                        context.加工厂s.Attach(SelectedFactory);
                    }

                    context.加工厂s.Remove(SelectedFactory);

                    // 保存更改
                    await context.SaveChangesAsync();

                    // 提交事务
                    await transaction.CommitAsync();

                    // 从本地集合中移除
                    Factories.Remove(SelectedFactory);
                    await 重置加工厂Async();
                    ShowMessage("加工厂删除成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    // 出现异常时回滚事务
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                // 处理数据库更新异常，显示内部异常信息
                string errorMessage = $"删除失败: {dbEx.Message}";
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"\n详细信息: {dbEx.InnerException.Message}";
                }
                ShowMessage(errorMessage, "数据库错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                ShowMessage($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 重置加工厂Async()
        {
            FactoryName = string.Empty;
            FactoryLocation = string.Empty;
            FactoryDescription = string.Empty;
            FactoryContact = string.Empty;
            FactoryEfficiency = string.Empty;
            FactoryEquipmentCount = string.Empty;
            FactorySelectedEquipment = null;
            SelectedFactory = null;
            await 更新选框Async();
            await 加载加工厂();
        }

        #endregion

        #region 生产工艺管理命令

        [RelayCommand]
        private async Task 保存生产工艺Async()
        {
            var newProcess = new 生产工艺
            {
                工艺名称 = ProcessName,
                备注 = ProcessRemarks,
                质量标准 = ProcessQualityStandard,
                安全指引 = ProcessSafetyGuidelines,
                标准加工时间 = int.TryParse(ProcessStandardTime, out int time) && time > 0 ? time : 0,
                产品ID = ProcessSelectedProduct?.产品ID ?? 0,
                材料ID = ProcessSelectedMaterial?.材料ID ?? 0,
                生产设备ID = ProcessSelectedEquipment?.生产设备ID ?? 0
            };

            // 验证输入
            if (string.IsNullOrWhiteSpace(newProcess.工艺名称) || newProcess.标准加工时间 <= 0)
            {
                ShowMessage("工艺名称和标准加工时间为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.生产工艺s.AnyAsync(p => p.工艺名称 == newProcess.工艺名称);
                if (exists)
                {
                    ShowMessage("生产工艺名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.生产工艺s.Add(newProcess);
                await context.SaveChangesAsync();
                Processes.Add(newProcess);
                await 重置生产工艺Async();
                ShowMessage("生产工艺保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"保存生产工艺失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 更新生产工艺Async()
        {
            if (SelectedProcess == null)
            {
                ShowMessage("请先选择要更新的生产工艺。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 更新属性
            SelectedProcess.工艺名称 = ProcessName;
            SelectedProcess.备注 = ProcessRemarks;
            SelectedProcess.质量标准 = ProcessQualityStandard;
            SelectedProcess.安全指引 = ProcessSafetyGuidelines;
            SelectedProcess.标准加工时间 = int.TryParse(ProcessStandardTime, out int time) && time > 0 ? time : 0;
            SelectedProcess.产品ID = ProcessSelectedProduct?.产品ID ?? 0;
            SelectedProcess.材料ID = ProcessSelectedMaterial?.材料ID ?? 0;
            SelectedProcess.生产设备ID = ProcessSelectedEquipment?.生产设备ID ?? 0;

            // 验证输入
            if (string.IsNullOrWhiteSpace(SelectedProcess.工艺名称) || SelectedProcess.标准加工时间 <= 0)
            {
                ShowMessage("工艺名称和标准加工时间为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.生产工艺s.AnyAsync(p => p.工艺名称 == SelectedProcess.工艺名称 && p.工艺ID != SelectedProcess.工艺ID);
                if (exists)
                {
                    ShowMessage("生产工艺名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.生产工艺s.Update(SelectedProcess);
                await context.SaveChangesAsync();
                await 重置生产工艺Async();
                ShowMessage("生产工艺更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"更新生产工艺失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 删除生产工艺Async()
        {
            if (SelectedProcess == null)
            {
                ShowMessage("请先选择要删除的生产工艺。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"确定要删除生产工艺 '{SelectedProcess.工艺名称}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // 开始事务，确保操作的原子性
                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    // 检查生产工艺是否被引用
                    bool isProcessReferenced = await IsProductionProcessReferencedAsync((int)SelectedProcess.工艺ID);
                    if (isProcessReferenced)
                    {
                        string references = await GetProductionProcessReferencesAsync((int)SelectedProcess.工艺ID);
                        ShowMessage($"无法删除选中的生产工艺，因为存在以下引用：{references}。请先删除相关引用。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 删除生产工艺
                    // 确保实体被上下文追踪
                    var processEntry = context.生产工艺s.Local.FirstOrDefault(p => p.工艺ID == SelectedProcess.工艺ID);
                    if (processEntry == null)
                    {
                        context.生产工艺s.Attach(SelectedProcess);
                    }

                    context.生产工艺s.Remove(SelectedProcess);

                    // 保存更改
                    await context.SaveChangesAsync();

                    // 提交事务
                    await transaction.CommitAsync();

                    // 从本地集合中移除
                    Processes.Remove(SelectedProcess);
                    await 重置生产工艺Async();
                    ShowMessage("生产工艺删除成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    // 出现异常时回滚事务
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                // 处理数据库更新异常，显示内部异常信息
                string errorMessage = $"删除失败: {dbEx.Message}";
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"\n详细信息: {dbEx.InnerException.Message}";
                }
                ShowMessage(errorMessage, "数据库错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                ShowMessage($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 重置生产工艺Async()
        {
            ProcessName = string.Empty;
            ProcessRemarks = string.Empty;
            ProcessQualityStandard = string.Empty;
            ProcessSafetyGuidelines = string.Empty;
            ProcessStandardTime = string.Empty;
            ProcessSelectedProduct = null;
            ProcessSelectedMaterial = null;
            ProcessSelectedEquipment = null;
            SelectedProcess = null;
            await 更新选框Async();
            await 加载生产工艺();
        }

        #endregion

        #region 生产设备管理命令

        [RelayCommand]
        private async Task 保存生产设备Async()
        {
            var newEquipment = new 生产设备
            {
                名称 = EquipmentName,
                描述 = EquipmentDescription,
                容量 = decimal.TryParse(EquipmentCapacity, out decimal capacity) ? capacity : 0,
                效率 = decimal.TryParse(EquipmentEfficiency, out decimal efficiency) ? efficiency : 0
            };

            // 验证输入
            if (string.IsNullOrWhiteSpace(newEquipment.名称))
            {
                ShowMessage("名称为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newEquipment.容量 <= 0)
            {
                ShowMessage("容量必须大于 0。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newEquipment.效率 < 0m || newEquipment.效率 > 1m)
            {
                ShowMessage("效率必须在 0 到 1 之间。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.生产设备s.AnyAsync(e => e.名称 == newEquipment.名称);
                if (exists)
                {
                    ShowMessage("生产设备名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.生产设备s.Add(newEquipment);
                await context.SaveChangesAsync();
                Equipments.Add(newEquipment);
                await 重置生产设备Async();
                ShowMessage("生产设备保存成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"保存生产设备失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 更新生产设备Async()
        {
            if (SelectedEquipment == null)
            {
                ShowMessage("请先选择要更新的生产设备。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 更新属性
            SelectedEquipment.名称 = EquipmentName;
            SelectedEquipment.描述 = EquipmentDescription;
            SelectedEquipment.容量 = decimal.TryParse(EquipmentCapacity, out decimal capacity) ? capacity : 0;
            SelectedEquipment.效率 = decimal.TryParse(EquipmentEfficiency, out decimal efficiency) ? efficiency : 0;

            // 验证输入
            if (string.IsNullOrWhiteSpace(SelectedEquipment.名称))
            {
                ShowMessage("名称为必填项。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedEquipment.容量 <= 0)
            {
                ShowMessage("容量必须大于 0。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedEquipment.效率 < 0m || SelectedEquipment.效率 > 1m)
            {
                ShowMessage("效率必须在 0 到 1 之间。", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = _contextFactory.CreateDbContext();
                bool exists = await context.生产设备s.AnyAsync(e => e.名称 == SelectedEquipment.名称 && e.生产设备ID != SelectedEquipment.生产设备ID);
                if (exists)
                {
                    ShowMessage("生产设备名称已存在。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.生产设备s.Update(SelectedEquipment);
                await context.SaveChangesAsync();
                await 重置生产设备Async();
                ShowMessage("生产设备更新成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"更新生产设备失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 删除生产设备Async()
        {
            if (SelectedEquipment == null)
            {
                ShowMessage("请先选择要删除的生产设备。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"确定要删除生产设备 '{SelectedEquipment.名称}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // 开始事务，确保操作的原子性
                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    // 检查生产设备是否被引用
                    bool isEquipmentReferenced = await IsEquipmentReferencedAsync(SelectedEquipment.生产设备ID);
                    if (isEquipmentReferenced)
                    {
                        string references = await GetEquipmentReferencesAsync(SelectedEquipment.生产设备ID);
                        ShowMessage($"无法删除选中的生产设备，因为存在以下引用：{references}。请先删除相关引用。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 删除生产设备
                    // 确保实体被上下文追踪
                    var equipmentEntry = context.生产设备s.Local.FirstOrDefault(e => e.生产设备ID == SelectedEquipment.生产设备ID);
                    if (equipmentEntry == null)
                    {
                        context.生产设备s.Attach(SelectedEquipment);
                    }

                    context.生产设备s.Remove(SelectedEquipment);

                    // 保存更改
                    await context.SaveChangesAsync();

                    // 提交事务
                    await transaction.CommitAsync();

                    // 从本地集合中移除
                    Equipments.Remove(SelectedEquipment);
                    await 重置生产设备Async();
                    ShowMessage("生产设备删除成功。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    // 出现异常时回滚事务
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                // 处理数据库更新异常，显示内部异常信息
                string errorMessage = $"删除失败: {dbEx.Message}";
                if (dbEx.InnerException != null)
                {
                    errorMessage += $"\n详细信息: {dbEx.InnerException.Message}";
                }
                ShowMessage(errorMessage, "数据库错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                ShowMessage($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task 重置生产设备Async()
        {
            EquipmentName = string.Empty;
            EquipmentType = string.Empty;
            EquipmentDescription = string.Empty;
            EquipmentCapacity = string.Empty;
            EquipmentEfficiency = string.Empty;
            SelectedEquipment = null;
            await 更新选框Async();
            await 加载生产设备();
        }

        #endregion

        #region 引用检查和引用表名获取方法

        // 检查加工厂是否被引用
        private async Task<bool> IsFactoryReferencedAsync(int factoryId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "生产计划", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产计划s.AnyAsync(p => p.加工厂ID == id);
            }
        }
        // 如果未来有其他表引用加工厂，可以在这里添加
    };

            return await IsEntityReferencedAsync(factoryId, referenceChecks);
        }

        // 获取加工厂被引用的表名
        private async Task<string> GetFactoryReferencesAsync(int factoryId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "生产计划", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产计划s.AnyAsync(p => p.加工厂ID == id);
            }
        }
        // 如果未来有其他表引用加工厂，可以在这里添加
    };

            var references = await GetEntityReferencesAsync(factoryId, referenceChecks);
            return string.Join("、", references);
        }

        // 检查生产工艺是否被引用
        private async Task<bool> IsProductionProcessReferencedAsync(int processId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "生产计划", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产计划s.AnyAsync(p => p.工艺ID == id);
            }
        }
        // 添加其他可能引用生产工艺的表
    };

            return await IsEntityReferencedAsync(processId, referenceChecks);
        }

        // 获取生产工艺被引用的表名
        private async Task<string> GetProductionProcessReferencesAsync(int processId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "生产计划", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产计划s.AnyAsync(p => p.工艺ID == id);
            }
        }
        // 添加其他可能引用生产工艺的表
    };

            var references = await GetEntityReferencesAsync(processId, referenceChecks);
            return string.Join("、", references);
        }

        // 检查生产设备是否被引用
        private async Task<bool> IsEquipmentReferencedAsync(int equipmentId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "加工厂", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.加工厂s.AnyAsync(o => o.生产设备ID == id);
            }
        },
        { "生产工艺", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产工艺s.AnyAsync(p => p.生产设备ID == id);
            }
        }
        // 添加其他可能引用生产设备的表
    };

            return await IsEntityReferencedAsync(equipmentId, referenceChecks);
        }

        // 获取生产设备被引用的表名
        private async Task<string> GetEquipmentReferencesAsync(int equipmentId)
        {
            var referenceChecks = new Dictionary<string, Func<int, Task<bool>>>
    {
        { "加工厂", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.加工厂s.AnyAsync(o => o.生产设备ID == id);
            }
        },
        { "生产工艺", async id =>
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.生产工艺s.AnyAsync(p => p.生产设备ID == id);
            }
        }
        // 添加其他可能引用生产设备的表
    };

            var references = await GetEntityReferencesAsync(equipmentId, referenceChecks);
            return string.Join("、", references);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="caption">标题</param>
        /// <param name="buttons">按钮类型</param>
        /// <param name="icon">图标类型</param>
        private void ShowMessage(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageBox.Show(message, caption, buttons, icon);
        }

        private async Task 更新选框Async()
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    // 更新生产设备（FactorySelectedEquipment）
                    var factoryEquipments = await context.生产设备s.AsNoTracking().ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Equipments.Clear();
                        foreach (var equipment in factoryEquipments)
                        {
                            Equipments.Add(equipment);
                        }
                    });

                    // 更新产品（ProcessProducts）
                    var products = await context.产品s.AsNoTracking().ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessProducts.Clear();
                        foreach (var product in products)
                        {
                            ProcessProducts.Add(product);
                        }
                    });

                    // 更新材料（ProcessMaterials）
                    var materials = await context.材料s.AsNoTracking().ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessMaterials.Clear();
                        foreach (var material in materials)
                        {
                            ProcessMaterials.Add(material);
                        }
                    });

                    // 更新生产设备（ProcessEquipments）
                    var processEquipments = await context.生产设备s.AsNoTracking().ToListAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessEquipments.Clear();
                        foreach (var equipment in processEquipments)
                        {
                            ProcessEquipments.Add(equipment);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"刷新 ComboBox 数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 判断实体是否被引用
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <param name="referenceChecks">引用检查的字典，键为表名，值为检查函数</param>
        /// <returns>如果被引用则返回 true，否则返回 false</returns>
        private async Task<bool> IsEntityReferencedAsync(int entityId, Dictionary<string, Func<int, Task<bool>>> referenceChecks)
        {
            foreach (var checkFunc in referenceChecks.Values)
            {
                if (await checkFunc(entityId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实体被哪些表引用
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <param name="referenceChecks">引用检查的字典，键为表名，值为检查函数</param>
        /// <returns>引用的表名列表</returns>
        private async Task<List<string>> GetEntityReferencesAsync(int entityId, Dictionary<string, Func<int, Task<bool>>> referenceChecks)
        {
            var references = new List<string>();

            foreach (var kvp in referenceChecks)
            {
                var tableName = kvp.Key;
                var checkFunc = kvp.Value;

                if (await checkFunc(entityId))
                {
                    references.Add(tableName);
                }
            }

            return references;
        }

        #endregion
    }
}

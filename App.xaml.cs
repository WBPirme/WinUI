// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using OGAS.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using NLog;
using OGAS.Services;
using Microsoft.Extensions.Hosting;
using OGAS.ViewModels;
using OGAS.Views;
using OGAS.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.Configuration;
using System.IO;
using OGAS.Converters;
using Microsoft.Extensions.Logging;
namespace OGAS
{
    public partial class App : Application
    {
        private readonly IHost _host;
        

        // 将 ServiceProvider 声明为静态属性，以便全局访问
        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            // 配置 Host 和依赖注入
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // 添加 appsettings.json 配置文件
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 DbContext
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    // 配置 DbContext 使用动态连接字符串
                    services.AddDbContextFactory<OGASDbContext>(options =>
                        options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

                    // 注册 ViewModels
                    services.AddTransient<ProcessingViewModel>();
                    services.AddSingleton<PlanViewModel>();
                    services.AddTransient<DataCollectionViewModel>();
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MonitorViewModel>();

                    // 注册 Views
                    services.AddTransient<OGAS.Views.Main>();
                    services.AddTransient<OGAS.Views.DataCollection>();
                    services.AddTransient<OGAS.Views.Plan>();
                    services.AddSingleton<OGAS.Views.Processing>();
                    services.AddTransient<OGAS.Views.Monitor>();
                    services.AddTransient<LoginWindow>();

                    // 注册主窗口（假设您有 MainWindow）
                    services.AddTransient<OGAS.Views.MainWindow>();
                    services.AddTransient<OGAS.Views.LoginWindow>();

                    // 注册 ViewModelLocator
                    services.AddSingleton<ViewModelLocator>();

                    // 注册转换器（如果需要依赖注入）

                    // 注册导航服务
                    services.AddSingleton<INavigationService, OGAS.Services.NavigationService>();

                    services.AddHostedService<NoOpHostedService>();

                })
                .Build();
            ServiceProvider = _host.Services;
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"未处理的异常: {e.Exception}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // 配置全局异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            await _host.StartAsync();

            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            var loginViewModel = _host.Services.GetRequiredService<LoginViewModel>();
            loginWindow.DataContext = loginViewModel;

            bool? loginResult = loginWindow.ShowDialog();

            // 添加日志或调试信息
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation($"Login result: {loginResult}");

            if (loginResult == true)
            {
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
                mainWindow.DataContext = mainWindowViewModel;

                var navigationService = _host.Services.GetRequiredService<INavigationService>();
                navigationService.MainFrame = mainWindow.MainFrame;

                // 使用类型导航
                navigationService.NavigateTo("Main");

                // 将 MainWindow 设置为主窗口
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                logger.LogInformation("MainWindow 已显示。");

                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                logger.LogInformation("登录失败或取消，应用程序即将关闭。");
                Shutdown();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

        // 定义 CurrentDomain_UnhandledException 处理方法
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"发生未处理异常：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Shutdown();
        }
    }
}

using FileMonitorService.JsonService;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WPFApplication.ViewModels;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IJsonMonitor? _jsonMonitor;

        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new ServiceCollection();

            //Transients
            services.AddTransient<MainWindow>();

            //Singletons
            services.AddSingleton<IJsonMonitor, JsonMonitor>(service =>
            {
                string pathToFile = (string)base.FindResource("DataFilePath");
                return new JsonMonitor(pathToFile);
            });
            services.AddSingleton<MainWindowViewModel>();
            
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            _jsonMonitor = ServiceProvider.GetRequiredService<IJsonMonitor>();
            _jsonMonitor.StartMonitoring();

            var mainWindowVM = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindowVM.UpdateData();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _jsonMonitor?.StopMonitoring();
            _jsonMonitor?.Dispose();
            ServiceProvider.Dispose();
            base.OnExit(e);
        }
    }
}
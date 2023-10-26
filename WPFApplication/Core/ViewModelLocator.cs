using Microsoft.Extensions.DependencyInjection;
using WPFApplication.ViewModels;

namespace WPFApplication.Core
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowVM => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
    }
}
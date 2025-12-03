using Client.Common;
using Client.ViewModels.DialogContent;
using Client.Views.DialogContent;
using MaterialDesignThemes.Wpf;

namespace Client.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public async Task<ButtonResult> ShowAsync(string title, string message)
        {
            MessageBoxViewModel vm = new()
            {
                Title = title,
                Message = message
            };
            // 不会违反 MVVM，在 WPF/Prism/MVVM 架构里，这种由 Service 层创建 View + ViewModel 并注入的方式非常常见。
            // Service 层属于 UI 层的一部分，本来就允许直接创建 View。
            // 只要 View 与 ViewModel 之间没有相互依赖，就是合格的 MVVM。
            MessageBoxView view = new()
            {
                DataContext = vm
            };
            // DialogHost.Show() 的返回值 = 调用 DialogHost.Close() 时传入的对象
            object? result = await DialogHost.Show(view, AppConstants.MessageBoxDialog);

            if (result is null)
            {
                return ButtonResult.None;
            }
            return (ButtonResult)result;
        }
    }
}
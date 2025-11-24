using System.Windows;

namespace Client.Events
{
    public static class LoadingHelper
    {
        private static IEventAggregator? _eventAggregator;

        // 初始化一次，通常在 App.xaml.cs 或主 ViewModel 中调用
        public static void Initialize(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public static async Task RunWithLoadingAsync(Func<Task> func)
        {
            if (_eventAggregator is null)
                throw new InvalidOperationException($"{nameof(LoadingHelper)} 未初始化，请先调用 Initialize()");

            _eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Visible);
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Collapsed);
            }
        }
    }
}
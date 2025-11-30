using Client.Events;
using System.Windows;

namespace Client.Services
{
    public class LoadingService : ILoadingService
    {
        private IEventAggregator? _eventAggregator;

        public LoadingService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task RunWithLoadingAsync(Func<Task> func, bool isLogin = false)
        {
            if (_eventAggregator is null)
                return;

            _eventAggregator.GetEvent<LoadingOpenEvent>().Publish((true, isLogin));
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
                _eventAggregator.GetEvent<LoadingOpenEvent>().Publish((false, isLogin));
            }
        }
    }
}
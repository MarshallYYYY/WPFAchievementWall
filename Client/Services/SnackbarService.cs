using Client.Events;

namespace Client.Services
{
    public class SnackbarService : ISnackbarService
    {
        private readonly IEventAggregator eventAggregator;

        public SnackbarService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }
        public void SendMessage(string msg)
        {
            eventAggregator.GetEvent<SnackbarMessageEvent>().Publish(msg);
        }
    }
}
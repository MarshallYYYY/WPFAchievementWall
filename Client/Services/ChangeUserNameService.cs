using Client.Common;
using Client.Events;

namespace Client.Services
{
    public class ChangeUserNameService : IChangeUserNameService
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IUserSession userSession;

        public ChangeUserNameService(
            IEventAggregator eventAggregator,
            IUserSession userSession)
        {
            this.eventAggregator = eventAggregator;
            this.userSession = userSession;
        }

        public void ChangeUserNname(string userName)
        {
            eventAggregator.GetEvent<ChangeUserNameEvent>().Publish(userSession.CurrentUser.UserName);
        }
    }
}
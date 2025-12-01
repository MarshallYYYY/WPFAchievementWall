using Client.Common;
using Client.Services;

namespace Client.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        public IndexViewModel(IUserSession userSession)
        {
            this.userSession = userSession;
            UserName = userSession.CurrentUser.UserName;
            CurrentTime = DateTime.Now;
        }
        private readonly IUserSession userSession;

        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }
        private DateTime currentTime;
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { SetProperty(ref currentTime, value); }
        }
    }
}
using Client.Common;
using Client.Services.WebApi;

namespace Client.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        public IndexViewModel(IUserSession userSession, AchievementService achievementService)
        {
            this.userSession = userSession;
            this.achievementService = achievementService;
            UserName = userSession.CurrentUser.UserName;
            CurrentTime = DateTime.Now;
        }
        #region 欢迎当前用户、显示当前时间
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
        #endregion

        private readonly AchievementService achievementService;
        // TODO：软件打开后进入主页，随机显示一个成就。
        private void ShowRandomAchievement()
        {

        }
    }
}
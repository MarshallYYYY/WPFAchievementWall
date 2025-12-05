using Client.Common;
using Client.Services.WebApi;

namespace Client.ViewModels
{
    public class IndexViewModel : BindableBase, INavigationAware
    {
        public IndexViewModel(IUserSession userSession, IAchievementService achievementService)
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

        private readonly IAchievementService achievementService;
        // TODO：软件打开后进入主页，随机显示一个成就。
        private void ShowRandomAchievement()
        {

        }

        #region INavigationAware：刷新时间
        // 每次点击进主页的时候，刷新当前时间，防止跨越多天使用软件时，仍显示登录后的时间信息。
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CurrentTime = DateTime.Now;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
        #endregion
    }
}
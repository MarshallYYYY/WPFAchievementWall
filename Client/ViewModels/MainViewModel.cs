using Client.Common;
using Client.Events;
using Client.Models;
using Client.Services;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Client.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        public MainViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IUserSession userSession)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.userSession = userSession;

            eventAggregator.GetEvent<LoadingOpenEvent>().Subscribe(LoadingSubscribe);
            eventAggregator.GetEvent<SnackbarMessageEvent>().Subscribe(ShowMessage);
            eventAggregator.GetEvent<ChangeUserNameEvent>().Subscribe(ChangeUserName);

            NavigateCommand = new DelegateCommand<Menu>(Navigate);

            MsgQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        }

        #region 初始化配置
        private readonly IUserSession userSession;

        /// <summary>
        /// 当用户在登录窗口成功登录后，App.xaml.cs 调用该函数
        /// </summary>
        public void Configure()
        {
            InitMenus();
            UserName = userSession.CurrentUser.UserName;
            //Navigate(Menus.First(menu => menu.Title == "成就展示"));
            Navigate(Menus.First(menu => menu.Title == "主页"));
        }
        #endregion

        #region 上方工具栏

        private string mainViewTitle = "个人成就记录墙";
        //private string mainViewTitle = "个人成就记录墙 - 主页";

        public string MainViewTitle
        {
            get { return mainViewTitle; }
            set { SetProperty(ref mainViewTitle, value); }
        }

        private bool menuToggleButtonIsChecked = false;
        /// <summary>
        /// 左侧菜单栏的显示与否
        /// </summary>
        public bool MenuToggleButtonIsChecked
        {
            get { return menuToggleButtonIsChecked; }
            set { SetProperty(ref menuToggleButtonIsChecked, value); }
        }

        #endregion 上方工具栏

        #region 左侧菜单栏
        // TDOO：找个位置设置左侧菜单栏的用户名
        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        //  { get; set; } 是必要的！！！
        public ObservableCollection<Menu> Menus { get; set; } = [];

        private void InitMenus()
        {
            Menus.Add(new Menu() { Icon = "🏠", Title = "主页", ViewName = "IndexView" });
            Menus.Add(new Menu() { Icon = "🏆", Title = "成就展示", ViewName = "AchievementDisplayView" });
            Menus.Add(new Menu() { Icon = "🎯", Title = "目标管理", ViewName = "GoalsManagementView" });
            Menus.Add(new Menu() { Icon = "📈", Title = "数据统计", ViewName = "DataStatisticsView" });
            Menus.Add(new Menu() { Icon = "⚙️", Title = "设置", ViewName = "SettingsView" });
        }
        private readonly IRegionManager regionManager;
        public DelegateCommand<Menu> NavigateCommand { get; private set; }

        private void Navigate(Menu menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName))
                return;

            regionManager.Regions[AppConstants.MainViewRegion].RequestNavigate(menu.ViewName);
            MainViewTitle = $"个人成就记录墙 - {menu.Title}";
            MenuToggleButtonIsChecked = false;
        }
        private void ChangeUserName(string userName)
        {
            UserName = userName;
        }
        #endregion 左侧菜单栏

        #region 加载窗口
        private readonly IEventAggregator eventAggregator;

        private void LoadingSubscribe((bool isOpen, bool isLogin) tuple)
        {
            // MainViewModel 只监听自己的标识
            if (tuple.isLogin is false)
                IsOpenDialogContent = tuple.isOpen;
        }
        private bool isOpenDialogContent = false;
        public bool IsOpenDialogContent
        {
            get { return isOpenDialogContent; }
            set { SetProperty(ref isOpenDialogContent, value); }
        }

        #endregion 加载窗口

        #region Snackbar消息队列
        public SnackbarMessageQueue MsgQueue { get; }
        private void ShowMessage(string msg)
        {
            MsgQueue.Enqueue(msg);
        }
        #endregion
    }
}
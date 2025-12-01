using Client.Common;
using Client.Events;
using Client.Models;
using Client.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Client.ViewModels
{
    public class MainViewModel : BindableBase,IConfigureService
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
            NavigateCommand = new DelegateCommand<Menu>(Navigate);

            TestCommand = new DelegateCommand(TestButton);
            //TestCommand = new(() => IsOpenDialogContent = !IsOpenDialogContent);
        }

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

        private readonly IUserSession userSession;
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

            regionManager.Regions[PrismRegionName.MainViewRegion].RequestNavigate(menu.ViewName);
            MainViewTitle = $"个人成就记录墙 - {menu.Title}";
            MenuToggleButtonIsChecked = false;
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

        #region Test

        public DelegateCommand TestCommand { get; private set; }

        private void TestButton()
        {
            if (Debugger.IsAttached)
            {
                // 简单提示开发者
                Debug.WriteLine("注意：重启后需要手动重新附加调试器");

                // 或者使用更明显的方式
                if (MessageBox.Show(
                    "重启后将断开与调试器的连接。继续吗？",
                    "调试提示",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            // 获取当前应用程序的路径和文件名

            //旧版.NET Framework
            //string applicationPath = Process.GetCurrentProcess().MainModule!.FileName;
            string applicationPath = Environment.ProcessPath!;

            // 启动新的应用程序实例
            Process.Start(applicationPath);

            // 关闭当前应用程序
            Application.Current.Shutdown();
        }
        #endregion Test
    }
}
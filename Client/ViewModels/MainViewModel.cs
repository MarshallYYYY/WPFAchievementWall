using Client.Common;
using Client.Events;
using Client.Models;
using Client.Services;
using Client.ViewModels.MainViewModels;
using Models;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            UserService userService, AchievementService achievementService, GoalService goalService)
        {
            InitMenus();
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Subscribe(
                visibility => LoadingVisibility = visibility);
            NavigateCommand = new DelegateCommand<Menu>(Navigate);

            //Navigate(Menus.First(menu => menu.Title == "成就展示"));

            this.userService = userService;
            this.achievementService = achievementService;
            this.goalService = goalService;
            //Test(userService, achievementService, goalService);

            TestCommand = new DelegateCommand(TestButton);
        }

        //  { get; set; } 是必要的！！！
        public ObservableCollection<Menu> Menus { get; set; } = [];
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        public DelegateCommand<Menu> NavigateCommand { get; private set; }

        private string mainViewTitle = "个人成就记录墙";
        public string MainViewTitle
        {
            get { return mainViewTitle; }
            set { SetProperty(ref mainViewTitle, value); }
        }
        private bool menuToggleButtonIsChecked = false;
        public bool MenuToggleButtonIsChecked
        {
            get { return menuToggleButtonIsChecked; }
            set { SetProperty(ref menuToggleButtonIsChecked, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        /// <summary>
        /// 加载部分的显示与否
        /// </summary>
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                SetProperty(ref loadingVisibility, value);
                if (loadingVisibility is Visibility.Collapsed)
                    RegionVisibility = Visibility.Visible;
                else if (loadingVisibility is Visibility.Visible)
                    RegionVisibility = Visibility.Collapsed;
            }
        }
        private Visibility regionVisibility;
        public Visibility RegionVisibility
        {
            get { return regionVisibility; }
            set { SetProperty(ref regionVisibility, value); }
        }


        private void InitMenus()
        {
            Menus.Add(new Menu() { Icon = "📊", Title = "成就展示", ViewName = "AchievementDisplayView" });
            Menus.Add(new Menu() { Icon = "🎯", Title = "目标管理", ViewName = "GoalsManagementView" });
            Menus.Add(new Menu() { Icon = "📈", Title = "数据统计", ViewName = "DataStatisticsView" });
            Menus.Add(new Menu() { Icon = "⚙️", Title = "设置", ViewName = "SettingsView" });
        }
        private void Navigate(Menu menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName))
                return;

            regionManager.Regions[PrismRegionName.MainViewRegion].RequestNavigate(menu.ViewName);
            MainViewTitle = $"个人成就记录墙 - {menu.Title}";
            MenuToggleButtonIsChecked = false;
        }

        #region Test
        public DelegateCommand TestCommand { get; private set; }

        private void TestButton()
        {
            if (loadingVisibility is Visibility.Collapsed)
                LoadingVisibility = Visibility.Visible;
            else if (loadingVisibility is Visibility.Visible)
                LoadingVisibility = Visibility.Collapsed;
        }

        private readonly UserService userService;
        private readonly AchievementService achievementService;
        private readonly GoalService goalService;
        private async void Test(UserService userService, AchievementService achievementService, GoalService goalService)
        {
            List<User> users = await userService.GetUsersAsync();
            string msg = "";
            foreach (User user in users)
            {
                msg += $"{user.Id}  {user.UserName}  {user.Password}  {user.AvatarPath}  {user.CreateTime}\n";
            }
            MessageBox.Show(msg);

            List<Achievement> achievements = await achievementService.GetAchievementsAsync();
            msg = "";
            foreach (Achievement achievement in achievements)
            {
                msg += $"{achievement.Id}  {achievement.Category}\n";
            }
            MessageBox.Show(msg);

            List<Goal> goals = await goalService.GetGoalsAsync();
            msg = "";
            goals.ForEach(goal => msg += $"{goal.Id} {goal.Title} {goal.Content} {goal.TargetDate} {goal.AchieveDate}\n");
            MessageBox.Show(msg);
        }
        #endregion
    }
}
using Client.Common;
using Client.Models;
using Client.ViewModels.MainViewModels;
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
        public MainViewModel(IRegionManager regionManager)
        {
            InitMenus();
            this.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<Menu>(Navigate);
            
            //Navigate(Menus.First(menu => menu.Title == "成就展示"));
        }
        //  { get; set; } 是必要的！！！
        public ObservableCollection<Menu> Menus { get; set; } = [];
        private void InitMenus()
        {
            Menus.Add(new Menu() { Icon = "📊", Title = "成就展示", ViewName = "AchievementDisplayView" });
            Menus.Add(new Menu() { Icon = "🎯", Title = "目标管理", ViewName = "GoalsManagementView" });
            Menus.Add(new Menu() { Icon = "📈", Title = "数据统计", ViewName = "DataStatisticsView" });
            Menus.Add(new Menu() { Icon = "⚙️", Title = "设置", ViewName = "SettingsView" });
        }
        private string mainViewTitle = "个人成就记录墙";
        public string MainViewTitle
        {
            get { return mainViewTitle; }
            set { SetProperty(ref mainViewTitle, value); }
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
        private bool menuToggleButtonIsChecked = false;
        public bool MenuToggleButtonIsChecked
        {
            get { return menuToggleButtonIsChecked; }
            set { SetProperty(ref menuToggleButtonIsChecked, value); }
        }
    }
}
using Client.Common;
using Client.Models;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Client.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IRegionManager regionManager)
        {
            InitMenus();
            NavigateCommand = new DelegateCommand<Menu>(Navigate);
            this.regionManager = regionManager;
        }

        //  { get; set; } 是必须的！！！
        public ObservableCollection<Menu> Menus { get; set; } = [];
        private void InitMenus()
        {
            Menus.Add(new Menu() { Icon = "📊", Title = "成就展示", ViewName = "AchievementDisplayView" });
            Menus.Add(new Menu() { Icon = "🎯", Title = "目标管理", ViewName = "ToDoView" });
            Menus.Add(new Menu() { Icon = "📈", Title = "数据统计", ViewName = "MemoView" });
            Menus.Add(new Menu() { Icon = "⚙️", Title = "设置", ViewName = "SettingsView" });
        }
        private readonly IRegionManager regionManager;
        public DelegateCommand<Menu> NavigateCommand { get; private set; }
        private void Navigate(Menu menu)
        {
            if (menu == null || string.IsNullOrEmpty(menu.ViewName))
                return;

            regionManager.Regions[PrismRegionName.MainViewRegion].RequestNavigate(menu.ViewName);
            //Title = $"MyToDo - {menu.Title}";
        }
        private bool menuToggleButtonIsChecked = false;
        public bool MenuToggleButtonIsChecked
        {
            get { return menuToggleButtonIsChecked; }
            set { SetProperty(ref menuToggleButtonIsChecked, value); }
        }
    }
}

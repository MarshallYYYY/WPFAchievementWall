using Client.Models;
using Client.Services;
using Client.ViewModels;
using Client.ViewModels.AchievementDisplayViewModels;
using Client.ViewModels.MainViewModels;
using Client.Views;
using Client.Views.AchievementDisplayViews;
using Client.Views.MainViews;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AchievementDisplayView, AchievementDisplayViewModel>();
            containerRegistry.RegisterForNavigation<GoalsManagementView, GoalsManagementViewModel>();
            containerRegistry.RegisterForNavigation<DataStatisticsView, DataStatisticsViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            
            // 注册 API 服务，注意不能用 https！
            string baseUrl = "http://localhost:5045";

            // 注册为单例服务
            containerRegistry.RegisterSingleton<UserService>(() => new UserService(baseUrl));
            containerRegistry.RegisterSingleton<AchievementService>(() => new AchievementService(baseUrl));
            containerRegistry.RegisterSingleton<GoalService>(() => new GoalService(baseUrl));
        }
    }
}
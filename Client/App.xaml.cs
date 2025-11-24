using Client.Events;
using Client.Services;
using Client.ViewModels;
using Client.Views;
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
            // 容器已经准备好，可以 Resolve 出 IEventAggregator 实例
            // 解析 IEventAggregator 实例
            var eventAggregator = Container.Resolve<IEventAggregator>();

            // 初始化静态 LoadingHelper
            LoadingHelper.Initialize(eventAggregator);

            return Container.Resolve<MainView>();
        }

        // 只能注册类型，没有实例可用
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
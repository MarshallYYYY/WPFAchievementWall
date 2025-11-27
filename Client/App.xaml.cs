using Client.Events;
using Client.Services;
using Client.ViewModels;
using Client.Views;
using System.Windows;

namespace Client
{
    public partial class App : PrismApplication
    {
        // 只能注册类型，没有实例可用，也不能 Resolve
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

            containerRegistry.RegisterForNavigation<AchievementDisplayView, AchievementDisplayViewModel>();
            containerRegistry.RegisterForNavigation<GoalsManagementView, GoalsManagementViewModel>();
            containerRegistry.RegisterForNavigation<DataStatisticsView, DataStatisticsViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            
            #region WebApi Service
            // 注册 API 服务，注意不能用 https！
            const string baseUrl = "http://localhost:5045";

            // 注册为单例服务

            // 当服务构造函数需要外部参数或容器无法自动创建的类型时，必须用 lambda / 工厂方法注册
            containerRegistry.RegisterSingleton<UserService>(() => new UserService(baseUrl));
            // 构造函数没有参数或者构造函数依赖的类型都已经注册在容器里，可以用下面的写法，
            // 这个方法容器会自动通过 反射 找到构造函数，自动 Resolve 所需的依赖。
            //containerRegistry.RegisterSingleton<XXX>();

            containerRegistry.RegisterSingleton<AchievementService>(() => new AchievementService(baseUrl));
            containerRegistry.RegisterSingleton<GoalService>(() => new GoalService(baseUrl)); 
            #endregion
        }
        // 只要你在 CreateShell 中返回某个 Window，Prism 会自动把这个 Window 注册到容器里作为单例。
        protected override Window CreateShell()
        {
            IEventAggregator eventAggregator = Container.Resolve<IEventAggregator>();
            // 初始化静态 LoadingHelper
            LoadingHelper.Initialize(eventAggregator);
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            IDialogService dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(LoginView), null, result =>
            {
                if (result.Result is not ButtonResult.OK)
                    Current.Shutdown();
            });

            //没有马上 base.OnInitialized()，所以 Shell(MainView) 还不会显示，
            //登录成功后 MainView 才开始生命周期（Loaded、Activated 等）
            base.OnInitialized();
        }
    }
}
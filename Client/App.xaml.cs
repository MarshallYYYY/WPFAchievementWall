using Client.Common;
using Client.Services;
using Client.Services.WebApi;
using Client.ViewModels;
using Client.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Windows;

namespace Client
{
    public partial class App : PrismApplication
    {
        // 只能注册类型，没有实例可用，也不能 Resolve
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
            containerRegistry.RegisterSingleton<IUserSession, UserSession>();

            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<AchievementDisplayView, AchievementDisplayViewModel>();
            containerRegistry.RegisterForNavigation<GoalsManagementView, GoalsManagementViewModel>();
            containerRegistry.RegisterForNavigation<DataStatisticsView, DataStatisticsViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            #region WebApi Service
            // 注册 API 服务，注意不能用 https！
            const string baseUrl = "http://localhost:5045";

            // 注册为单例服务

            // 当服务构造函数需要外部参数或容器无法自动创建的类型时，必须用 lambda / 工厂方法注册
            containerRegistry.RegisterSingleton<IUserService>(() => new UserService(baseUrl));
            // 构造函数没有参数或者构造函数依赖的类型都已经注册在容器里，可以用下面的写法，
            // 这个方法容器会自动通过 反射 找到构造函数，自动 Resolve 所需的依赖。
            //containerRegistry.RegisterSingleton<XXX>();

            containerRegistry.RegisterSingleton<IAchievementService>(() => new AchievementService(baseUrl));
            containerRegistry.RegisterSingleton<IGoalService>(() => new GoalService(baseUrl));
            #endregion

            containerRegistry.RegisterSingleton<ILoadingService, LoadingService>();
            containerRegistry.RegisterSingleton<IMessageBoxService, MessageBoxService>();
            containerRegistry.RegisterSingleton<ISnackbarService, SnackbarService>();
            containerRegistry.RegisterSingleton<IChangeUserNameService, ChangeUserNameService>();
        }
        // 只要你在 CreateShell 中返回某个 Window，Prism 会自动把这个 Window 注册到容器里作为单例。
        protected override Window CreateShell()
        {
            // MainViewModel 的构造函数在 MainView 被 Container.Resolve 时就已经执行了。
            // Prism 会在解析 MainView 时：✔ 自动解析 ViewModel，并设置为 DataContext
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            IDialogService dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(LoginView), null, result =>
            {
                if (result.Result is not ButtonResult.OK)
                {
                    Current.Shutdown();
                    return;
                }

                // Prism 会自动把 MainViewModel 作为 MainView 的 DataContext。

                // 写法一
                //IConfigureService? service = Current.MainWindow.DataContext as IConfigureService;
                //if (service != null)
                //    service.Configure();

                // 写法二
                //IConfigureService? service = Current.MainWindow.DataContext as IConfigureService;
                //service?.Configure();

                // 写法三
                if (Current.MainWindow.DataContext is IConfigureService service)
                    service.Configure();

                //没有马上 base.OnInitialized()，所以 Shell(MainView) 还不会显示，
                //登录成功后 MainView 才开始生命周期（Loaded、Activated 等）
                base.OnInitialized();
            });
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LiveCharts.Configure(config =>
            config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));
        }
    }
}
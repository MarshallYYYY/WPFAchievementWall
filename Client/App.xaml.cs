using Client.Models;
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
        }
    }
}
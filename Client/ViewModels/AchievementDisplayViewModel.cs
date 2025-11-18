using Client.Events;
using Client.Models;
using Client.Services;
using Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.ViewModels
{
    public class AchievementDisplayViewModel : BindableBase
    {
        public AchievementDisplayViewModel(
            AchievementService achievementService,
            IEventAggregator eventAggregator)
        {
            OpenDetailsCommand = new DelegateCommand<Achievement>(OpenDetails);
            CloseDetailsCommand = new DelegateCommand(CloseDetails);
            this.achievementService = achievementService;
            this.eventAggregator = eventAggregator;

            InitAllAchievement(achievementService);

            // 这个不成功：
            //Task.Run(async () => await InitAllAchievement(achievementService));

            // 直接调用异步初始化方法，C# 7.0 丢弃运算符
            // 我明确知道这是一个异步方法，我故意不等待它完成，让它在后台运行，我不关心它的返回结果
            //_ = InitAllAchievement(achievementService);

        }
        public ObservableCollection<YearAchievements> AllAchievement { get; set; } = [];
        private Visibility detailsVisibility = Visibility.Hidden;
        public Visibility DetailsVisibility
        {
            get { return detailsVisibility; }
            set { SetProperty(ref detailsVisibility, value); }
        }
        private Achievement? selectedAchievement;
        public Achievement? SelectedAchievement
        {
            get { return selectedAchievement; }
            set { SetProperty(ref selectedAchievement, value); }
        }
        public DelegateCommand<Achievement> OpenDetailsCommand { get; private set; }
        public DelegateCommand CloseDetailsCommand { get; private set; }
        private readonly AchievementService achievementService;
        private readonly IEventAggregator eventAggregator;


        private async void InitAllAchievement(AchievementService service)
        {
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Visible);
            AllAchievement.Clear();
            List<Achievement> allAchievement = await service.GetAchievementsAsync();
            List<YearAchievements> allYearGroup = allAchievement
                .GroupBy(achievement => achievement.AchieveDate.Year.ToString())
                // 按年份降序排列
                .OrderByDescending(yearGroup => yearGroup.Key)
                //  Select(转换为YearAchievements)
                .Select(yearGroup => new YearAchievements
                {
                    Year = yearGroup.Key,
                    Achievements = new ObservableCollection<Achievement>(
                        yearGroup.OrderByDescending(achievement => achievement.AchieveDate).ToList())
                })
                .ToList();
            allYearGroup.ForEach(yearGroup => AllAchievement.Add(yearGroup));
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Collapsed);
        }
        private void OpenDetails(Achievement achievement)
        {
            SelectedAchievement = achievement;
            if (DetailsVisibility == Visibility.Hidden)
                DetailsVisibility = Visibility.Visible;
        }
        private void CloseDetails()
        {
            if (DetailsVisibility == Visibility.Visible)
                DetailsVisibility = Visibility.Hidden;
        }
    }
}
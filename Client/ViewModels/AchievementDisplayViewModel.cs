using Client.Events;
using Client.Models;
using Client.Services;
using Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Client.ViewModels
{
    public class AchievementDisplayViewModel : BindableBase
    {
        public AchievementDisplayViewModel(
            AchievementService achievementService,
            IEventAggregator eventAggregator)
        {
            OpenCloseDetailsCommand = new DelegateCommand<Achievement>(OpenCloseDetails);
            service = achievementService;
            this.eventAggregator = eventAggregator;

            // 直接调用异步初始化方法，C# 7.0 丢弃运算符
            // 我明确知道这是一个异步方法，我故意不等待它完成，让它在后台运行，我不关心它的返回结果
            _ = InitAllAchievement();

            AddCommand = new DelegateCommand(AddAchievement);
            UpdateCommand = new DelegateCommand(UpdateAchievement);
            DeleteCommand = new DelegateCommand(DeleteAchievement);
        }
        public ObservableCollection<YearAchievements> AllAchievement { get; set; } = [];
        private Visibility detailsVisibility = Visibility.Collapsed;
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
        public DelegateCommand<Achievement> OpenCloseDetailsCommand { get; private set; }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        /// <summary>
        /// Achievement API Service
        /// </summary>
        private readonly AchievementService service;
        private readonly IEventAggregator eventAggregator;


        private async Task InitAllAchievement()
        {
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Visible);
            AllAchievement.Clear();
            // 获取所有的成就
            List<Achievement> allAchievement = await service.GetAchievementsAsync();
            // 将所有的成就按照年份分组
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
            // 按照年份依次添加到 AllAchievement 中
            allYearGroup.ForEach(yearGroup => AllAchievement.Add(yearGroup));
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Collapsed);
        }
        private void OpenCloseDetails(Achievement achievement)
        {
            if (DetailsVisibility == Visibility.Collapsed)
            {
                DetailsVisibility = Visibility.Visible;
                SelectedAchievement = achievement;
            }
            else if (DetailsVisibility == Visibility.Visible)
            {
                if (achievement == SelectedAchievement)
                {
                    DetailsVisibility = Visibility.Collapsed;
                    SelectedAchievement = null;
                }
                else
                    SelectedAchievement = achievement;
            }
        }
        private void AddAchievement()
        {

        }
        private void UpdateAchievement()
        {

        }
        private async void DeleteAchievement()
        {
            if (selectedAchievement is null)
                return;
            MessageBoxResult boxResult = MessageBox.Show(
                "是否删除？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (boxResult is MessageBoxResult.No)
                return;
            bool result = await service.DeleteAchievementAsync(selectedAchievement.Id);
            if (result is true)
            {
                DetailsVisibility = Visibility.Collapsed;
                SelectedAchievement = null;
                _ = InitAllAchievement();
            }
        }
    }
}
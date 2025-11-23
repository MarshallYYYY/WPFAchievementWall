using Client.Events;
using Client.Models;
using Client.Services;
using DryIoc;
using MaterialDesignThemes.Wpf;
using Models;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace Client.ViewModels
{
    public partial class AchievementDisplayViewModel : BindableBase
    {
        public AchievementDisplayViewModel(
            AchievementService achievementService,
            IEventAggregator eventAggregator)
        {
            FilterCommand = new DelegateCommand(Filter);
            ClearCommand = new(ClearSearchBar);
            OpenCloseDetailsCommand = new DelegateCommand<Achievement>(OpenCloseDetails);
            service = achievementService;
            this.eventAggregator = eventAggregator;

            // 直接调用异步初始化方法，C# 7.0 丢弃运算符
            // 我明确知道这是一个异步方法，我故意不等待它完成，让它在后台运行，我不关心它的返回结果
            _ = InitData();

            AddCommand = new DelegateCommand<object>(AddAchievement);
            EditCommand = new DelegateCommand<object>(EditAchievement);
            DeleteCommand = new DelegateCommand(DeleteAchievement);
        }
        private readonly List<YearAchievements> localAllAchievement = [];
        public ObservableCollection<YearAchievements> AllAchievement { get; set; } = [];
        private string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string content = "";
        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }
        private DateTime achieveDate = DateTime.Now;
        public DateTime AchieveDate
        {
            get { return achieveDate; }
            set { SetProperty(ref achieveDate, value); }
        }
        public ObservableCollection<string> YearComboBoxSource { get; private set; } = [];
        public ObservableCollection<int> LevelComboBoxSource { get; } = [0, 1, 2, 3, 4, 5];
        public ObservableCollection<string> CategoryComboBoxSource { get; } =
            ["默认", "生活经历", "学习成长", "健康运动", "职业发展"];
        private string? yearComboBoxSelectedItem = null;
        public string? YearComboBoxSelectedItem
        {
            get { return yearComboBoxSelectedItem; }
            set { SetProperty(ref yearComboBoxSelectedItem, value); }
        }
        private int? levelComboBoxSelectedItem = null;
        public int? LevelComboBoxSelectedItem
        {
            get { return levelComboBoxSelectedItem; }
            set { SetProperty(ref levelComboBoxSelectedItem, value); }
        }
        private string? categoryComboBoxSelectedItem = null;
        public string? CategoryComboBoxSelectedItem
        {
            get { return categoryComboBoxSelectedItem; }
            set { SetProperty(ref categoryComboBoxSelectedItem, value); }
        }

        private Visibility detailsVisibility = Visibility.Collapsed;
        public Visibility DetailsVisibility
        {
            get { return detailsVisibility; }
            set { SetProperty(ref detailsVisibility, value); }
        }
        private Achievement? selectedAchievement = null;
        public Achievement? SelectedAchievement
        {
            get { return selectedAchievement; }
            set { SetProperty(ref selectedAchievement, value); }
        }
        private string titleAddEdit = "新增";
        public string TitleAddEdit
        {
            get { return titleAddEdit; }
            set { SetProperty(ref titleAddEdit, value); }
        }
        public DelegateCommand FilterCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        /// <summary>
        /// AchievementView：打开关闭详情成就页面（AchievementDetailsView）
        /// </summary>
        public DelegateCommand<Achievement> OpenCloseDetailsCommand { get; private set; }
        /// <summary>
        /// AchievementDisplayTopView：新增成就按钮
        /// </summary>
        public DelegateCommand<object> AddCommand { get; private set; }
        /// <summary>
        /// AchievementDetailsView：编辑成就按钮
        /// </summary>
        public DelegateCommand<object> EditCommand { get; private set; }
        /// <summary>
        /// AchievementDetailsView：删除成就按钮
        /// </summary>
        public DelegateCommand DeleteCommand { get; private set; }
        /// <summary>
        /// Achievement API Service
        /// </summary>
        private readonly AchievementService service;
        private readonly IEventAggregator eventAggregator;

        private async Task InitData()
        {
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Visible);
            await InitLocalAllAchievement();
            SetAllAchievement(localAllAchievement);
            InitYearComboBoxSource();
            eventAggregator.GetEvent<LoadingVisibilityEvent>().Publish(Visibility.Collapsed);
        }
        private async Task InitLocalAllAchievement()
        {
            localAllAchievement.Clear();
            // 获取所有的成就
            List<Achievement> allAchievement = await service.GetAchievementsAsync();
            // 将所有的成就按照年份分组
            List<YearAchievements> allYearGroup = allAchievement
                .GroupBy(achievement => achievement.AchieveDate.Year.ToString())
                // 按年份降序排列
                // yearGroup.Key 是刚刚分组的 Key（年份字符串）
                .OrderByDescending(yearGroup => yearGroup.Key)
                //  Select(转换为YearAchievements)
                .Select(yearGroup => new YearAchievements
                {
                    Year = yearGroup.Key,
                    // 调用构造函数来进行赋值
                    Achievements = new List<Achievement>(
                        yearGroup.OrderByDescending(achievement => achievement.AchieveDate).ToList())
                })
                .ToList();
            // 按照年份依次添加到 AllAchievement 中
            allYearGroup.ForEach(yearGroup => localAllAchievement.Add(yearGroup));
        }
        private void SetAllAchievement(List<YearAchievements> allAchievement)
        {
            AllAchievement.Clear();
            foreach (YearAchievements item in allAchievement)
                AllAchievement.Add(item);
        }
        private void InitYearComboBoxSource()
        {
            YearComboBoxSource.Clear();
            foreach (YearAchievements item in localAllAchievement)
            {
                if (item.Year is not null)
                {
                    // 因为 localAllAchievement 已经按照年份排好序了，
                    // 所以 YearComboBoxSource 直接添加元素即可，无需排序。
                    YearComboBoxSource.Add(item.Year);
                }
            }
        }
        private void Filter()
        {
            SelectedAchievement = null;
            DetailsVisibility = Visibility.Collapsed;

            AllAchievement.Clear();

            List<YearAchievements>? allAchievement = localAllAchievement;
            if (YearComboBoxSelectedItem is not null)
                allAchievement = allAchievement.Where(yearGroup => yearGroup.Year == YearComboBoxSelectedItem).ToList();

            // 其实当 搜索栏 选择了某个年份时，allAchievement 中只有一个元素，也就是下面的循环只执行一次；
            // 但是若 搜索栏 中未选择年份时，则对所有年份逐个运行检查。
            foreach (YearAchievements yearGroup in allAchievement)
            {
                IQueryable<Achievement> query = (yearGroup.Achievements ?? []).AsQueryable();

                if (Title is not "")
                    query = query.Where(achievement => (achievement.Title ?? "").Contains(Title));
                if (Content is not "")
                    query = query.Where(achievement => (achievement.Content ?? string.Empty).Contains(Content));
                if (LevelComboBoxSelectedItem is not null)
                    query = query.Where(achievement => achievement.Level == LevelComboBoxSelectedItem);
                if (CategoryComboBoxSelectedItem is not null)
                    query = query.Where(achievement => achievement.Category == CategoryComboBoxSelectedItem);

                if (query.Any() is false)
                    continue;
                AllAchievement.Add(new YearAchievements
                {
                    Year = yearGroup.Year,
                    //Achievements = query.ToList()
                    // 集合表达式
                    Achievements = [.. query]
                });
            }
        }
        private void ClearSearchBar()
        {
            SelectedAchievement = null;
            DetailsVisibility = Visibility.Collapsed;

            YearComboBoxSelectedItem = null;
            Title = "";
            Content = "";
            LevelComboBoxSelectedItem = null;
            CategoryComboBoxSelectedItem = null;

            SetAllAchievement(localAllAchievement);
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
        private void AddAchievement(object parameter)
        {
            TitleAddEdit = "新增";
            DialogHost.OpenDialogCommand.Execute(parameter, null);
        }
        private void EditAchievement(object parameter)
        {
            TitleAddEdit = "编辑";
            DialogHost.OpenDialogCommand.Execute(parameter, null);
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
                _ = InitData();
            }
        }
    }
}
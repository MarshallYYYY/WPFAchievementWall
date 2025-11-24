using Client.Events;
using Client.Models;
using Client.Services;
using MaterialDesignThemes.Wpf;
using Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;


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

            // C# 7.0 丢弃运算符：直接调用异步初始化方法
            // 我明确知道这是一个异步方法，我故意不等待它完成，让它在后台运行，我不关心它的返回结果

            // 下面是两种写法，在功能和行为上 等价
            // ①：lambda 方式一般在你想传入额外参数或多行操作时才需要
            //_ = LoadingHelper.RunWithLoadingAsync(async () => await InitData());
            // ②：
            _ = LoadingHelper.RunWithLoadingAsync(InitData);

            // ③：下面这种写法也可以，但会多一个无用的变量（语义不清晰），所以不推荐使用。
            //Task t = LoadingHelper.RunWithLoadingAsync(InitData);

            OpenAddCommand = new DelegateCommand(OpenAdd);
            OpenEditCommand = new DelegateCommand(OpenEdit);
            DeleteCommand = new DelegateCommand(DeleteAchievement);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new(() => AddEditVisibility = Visibility.Collapsed);
        }

        #region 成就展示（首层页面）

        /// <summary>
        /// Achievement API Service
        /// </summary>
        private readonly AchievementService service;

        /// <summary>
        /// 从 Web API 获取的所有成就，按年份分组存储；
        /// 只有在 初始化的查询 以及 删除、新增、编辑成就 后才会变更该属性的值，
        /// 因为这些操作都需要向 Web API 请求数据。
        /// </summary>
        private readonly List<YearAchievements> localAllAchievement = [];
        /// <summary>
        /// 用于绑定到界面的所有成就，按年份分组存储；
        /// 搜索栏的筛选按钮会变更该属性的值。
        /// </summary>
        public ObservableCollection<YearAchievements> AllAchievement { get; set; } = [];

        private Visibility detailsVisibility = Visibility.Collapsed;

        public Visibility DetailsVisibility
        {
            get { return detailsVisibility; }
            set { SetProperty(ref detailsVisibility, value); }
        }

        private async Task InitData()
        {
            await InitLocalAllAchievement();
            SetAllAchievement(localAllAchievement);
            InitSearchBarYearComboBoxSource();
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

        private void InitSearchBarYearComboBoxSource()
        {
            SearchBarYearComboBoxSource.Clear();
            foreach (YearAchievements item in localAllAchievement)
            {
                if (item.Year is not null)
                {
                    // 因为 localAllAchievement 已经按照年份排好序了，
                    // 所以 SearchBarYearComboBoxSource 直接添加元素即可，无需排序。
                    SearchBarYearComboBoxSource.Add(item.Year);
                }
            }
        }

        #endregion 成就展示（首层页面）

        #region 单个成就（左侧界面的一部分）

        /// <summary>
        /// AchievementView：打开关闭详情成就页面（AchievementDetailsView）
        /// </summary>
        public DelegateCommand<Achievement> OpenCloseDetailsCommand { get; private set; }

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

        #endregion 单个成就（左侧界面的一部分）

        #region 上方的搜索栏 SearchBar

        #region 数据
        public ObservableCollection<string> SearchBarYearComboBoxSource { get; private set; } = [];
        private string? searchBarYear = null;

        public string? SearchBarYear
        {
            get { return searchBarYear; }
            set { SetProperty(ref searchBarYear, value); }
        }

        private string searchBarTitle = "";

        public string SearchBarTitle
        {
            get { return searchBarTitle; }
            set { SetProperty(ref searchBarTitle, value); }
        }

        private string searchBarContent = "";

        public string SearchBarContent
        {
            get { return searchBarContent; }
            set { SetProperty(ref searchBarContent, value); }
        }

        public ObservableCollection<int> LevelComboBoxSource { get; } = [0, 1, 2, 3, 4, 5];
        private int? searchBarLevel = null;

        public int? SearchBarLevel
        {
            get { return searchBarLevel; }
            set { SetProperty(ref searchBarLevel, value); }
        }

        public ObservableCollection<string> CategoryComboBoxSource { get; } =
            ["默认", "生活经历", "学习成长", "健康运动", "职业发展"];

        private string? searchBarCategory = null;

        public string? SearchBarCategory
        {
            get { return searchBarCategory; }
            set { SetProperty(ref searchBarCategory, value); }
        }
        #endregion

        public DelegateCommand FilterCommand { get; private set; }

        private void Filter()
        {
            SelectedAchievement = null;
            DetailsVisibility = Visibility.Collapsed;

            AllAchievement.Clear();

            IEnumerable<YearAchievements> allAchievement = localAllAchievement;
            if (SearchBarYear is not null)
                allAchievement = allAchievement.Where(yearGroup => yearGroup.Year == SearchBarYear);

            // 其实当 搜索栏 选择了某个年份时，allAchievement 中只有一个元素，也就是下面的循环只执行一次；
            // 但是若 搜索栏 中未选择年份时，则对所有年份逐个运行检查。
            foreach (YearAchievements yearGroup in allAchievement)
            {
                IQueryable<Achievement> query = (yearGroup.Achievements ?? []).AsQueryable();

                if (SearchBarTitle is not "")
                    query = query.Where(achievement => (achievement.Title ?? "").Contains(SearchBarTitle));
                if (SearchBarContent is not "")
                    query = query.Where(achievement => (achievement.Content ?? string.Empty).Contains(SearchBarContent));
                if (SearchBarLevel is not null)
                    query = query.Where(achievement => achievement.Level == SearchBarLevel);
                if (SearchBarCategory is not null)
                    query = query.Where(achievement => achievement.Category == SearchBarCategory);

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

        public DelegateCommand ClearCommand { get; private set; }

        private void ClearSearchBar()
        {
            SelectedAchievement = null;
            DetailsVisibility = Visibility.Collapsed;

            SearchBarYear = null;
            SearchBarTitle = "";
            SearchBarContent = "";
            SearchBarLevel = null;
            SearchBarCategory = null;

            SetAllAchievement(localAllAchievement);
        }
        #endregion 上方的搜索栏 SearchBar

        #region 成就详情（右侧界面）

        private Achievement? selectedAchievement = null;

        public Achievement? SelectedAchievement
        {
            get { return selectedAchievement; }
            set { SetProperty(ref selectedAchievement, value); }
        }

        /// <summary>
        /// AchievementDetailsView：删除成就按钮
        /// </summary>
        public DelegateCommand DeleteCommand { get; private set; }

        private void DeleteAchievement()
        {
            if (selectedAchievement is null)
                return;
            MessageBoxResult boxResult = MessageBox.Show(
                "是否删除？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (boxResult is MessageBoxResult.No)
                return;

            _ = LoadingHelper.RunWithLoadingAsync(async () =>
            {
                bool result = await service.DeleteAchievementAsync(selectedAchievement.Id);
                if (result is true)
                {
                    DetailsVisibility = Visibility.Collapsed;
                    SelectedAchievement = null;
                    await InitData();
                    // 下面这个不能使用！！！
                    // fire-and-forget，可能未完成就关闭 Loading，异常可能被吞掉
                    //_ = InitData();
                }
            });
        }

        #endregion 成就详情（右侧界面）

        #region 新增、编辑成就功能
        private Visibility addEditVisibility = Visibility.Collapsed;
        public Visibility AddEditVisibility
        {
            get { return addEditVisibility; }
            set { SetProperty(ref addEditVisibility, value); }
        }
        private string titleAddEdit = "新增";
        public string TitleAddEdit
        {
            get { return titleAddEdit; }
            set { SetProperty(ref titleAddEdit, value); }
        }
        /// <summary>
        /// AchievementDisplayTopView：新增成就按钮
        /// </summary>
        public DelegateCommand OpenAddCommand { get; private set; }
        /// <summary>
        /// AchievementDetailsView：编辑成就按钮
        /// </summary>
        public DelegateCommand OpenEditCommand { get; private set; }
        private void OpenAdd()
        {
            TitleAddEdit = "新增";
            AchieveDate = DateTime.Now;
            Title = "";
            Content = "";
            Level = 0;
            Category = "默认";

            AddEditVisibility = Visibility.Visible;
        }

        private void OpenEdit()
        {
            TitleAddEdit = "编辑";
            if (selectedAchievement is null)
                return;
            AchieveDate = selectedAchievement.AchieveDate;
            Title = selectedAchievement.Title ?? "";
            Content = selectedAchievement.Content ?? "";
            Level = selectedAchievement.Level;
            Category = selectedAchievement.Category ?? "";

            AddEditVisibility = Visibility.Visible;
        }

        #region 数据
        private DateTime achieveDate;
        public DateTime AchieveDate
        {
            get { return achieveDate; }
            set { SetProperty(ref achieveDate, value); }
        }
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
        private int level;
        public int Level
        {
            get { return level; }
            set { SetProperty(ref level, value); }
        }
        private string category = "默认";
        public string Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }
        #endregion

        public DelegateCommand SaveCommand { get; private set; }
        private void Save()
        {
            switch (titleAddEdit)
            {
                case "新增":
                    AddAchievement();
                    break;
                case "编辑":
                    EditAchievement();
                    break;
                default:
                    break;
            }
        }
        private void AddAchievement()
        {
            Achievement achievement = new()
            {
                AchieveDate = achieveDate,
                Title = title,
                Content = content,
                Level = level,
                Category = category,
                ImagePath = "ImgPath"
            };

            _ = LoadingHelper.RunWithLoadingAsync(async () =>
            {
                Achievement achievementFromApi = await service.CreateAchievementAsync(achievement);
                if (achievementFromApi is not null)
                {
                    AddEditVisibility = Visibility.Collapsed;
                    DetailsVisibility = Visibility.Collapsed;
                    await InitData();
                }
            });
        }
        private void EditAchievement()
        {
            if (selectedAchievement is null)
                return;
            selectedAchievement.AchieveDate = AchieveDate;
            selectedAchievement.Title = Title;
            selectedAchievement.Content = Content;
            selectedAchievement.Level = Level;
            selectedAchievement.Category = Category;

            _ = LoadingHelper.RunWithLoadingAsync(async () =>
            {
                bool result = await service.UpdateAchievementAsync(selectedAchievement);
                if (result is true)
                {
                    AddEditVisibility = Visibility.Collapsed;
                    DetailsVisibility = Visibility.Collapsed;
                    await InitData();
                }
            });
        }
        public DelegateCommand CancelCommand { get; private set; }
        #endregion 新增、编辑成就页面
    }
}
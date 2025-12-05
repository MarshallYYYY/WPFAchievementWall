using Client.Common;
using Client.Models;
using Client.Services;
using Client.Services.WebApi;
using MaterialDesignThemes.Wpf;
using Models;
using System.Collections.ObjectModel;
using System.Windows;


namespace Client.ViewModels
{
    public partial class AchievementDisplayViewModel : BindableBase
    {
        public AchievementDisplayViewModel(
            IAchievementService achievementService,
            ILoadingService loadingService,
            IUserSession userSession,
            IMessageBoxService messageBoxService,
            ISnackbarService snackbarService)
        {
            FilterCommand = new DelegateCommand(Filter);
            ClearCommand = new(ClearSearchBar);

            OpenCloseDetailsCommand = new DelegateCommand<Achievement>(OpenCloseDetails);

            service = achievementService;
            this.loadingService = loadingService;
            this.userSession = userSession;
            this.messageBoxService = messageBoxService;
            this.snackbarService = snackbarService;

            // C# 7.0 丢弃运算符：直接调用异步初始化方法
            // 我明确知道这是一个异步方法，我故意不等待它完成，让它在后台运行，我不关心它的返回结果

            // 下面是两种写法，在功能和行为上 等价
            // ①：lambda 方式一般在你想传入额外参数或多行操作时才需要
            //_ = loadingService.RunWithLoadingAsync(async () => await InitData());
            // ②：
            _ = loadingService.RunWithLoadingAsync(InitData);

            // ③：下面这种写法也可以，但会多一个无用的变量（语义不清晰），所以不推荐使用。
            //Task t = loadingService.RunWithLoadingAsync(InitData);

            OpenAddCommand = new DelegateCommand(OpenAdd);
            OpenEditCommand = new DelegateCommand(OpenEdit);
            DeleteCommand = new DelegateCommand(DeleteAchievement);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new(() => IsShowAddEdit = 0);
        }

        #region 服务、会话
        /// <summary>
        /// Achievement API Service
        /// </summary>
        private readonly IAchievementService service;
        private readonly ILoadingService loadingService;
        private readonly IUserSession userSession;
        private readonly IMessageBoxService messageBoxService;
        private readonly ISnackbarService snackbarService;
        #endregion

        #region 成就展示（首层页面）

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
            ApiResult apiResult = await service.SetUserAchievementsGroupedByYearAsync(
                userSession.CurrentUser.Id, localAllAchievement);
            if (apiResult.IsSuccess is false)
            {
                snackbarService.SendMessage(apiResult.ErrorMessage!);
            }
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
        // ComboBoxSource相关的都改为可空了，下面的两个 Level 和 Category 的源相关的也是可空。
        public ObservableCollection<int?> SearchBarYearComboBoxSource { get; private set; } = [];
        private int? searchBarYear = null;
        public int? SearchBarYear
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

        public ObservableCollection<int> LevelComboBoxSource { get; } = [1, 2, 3, 4, 5];
        private int? searchBarLevel = null;
        public int? SearchBarLevel
        {
            get { return searchBarLevel; }
            set { SetProperty(ref searchBarLevel, value); }
        }

        //["默认", "生活经历", "学习成长", "健康运动", "职业发展"];
        public ObservableCollection<string> CategoryComboBoxSource { get; } =
        [
            AchievementCategory.Default,
            AchievementCategory.Life,
            AchievementCategory.Learning,
            AchievementCategory.Health,
            AchievementCategory.Career,
        ];
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
            if (searchBarYear is not null)
            {
                // 如果限制了某个年份，那么 allAchievement 中就只有一个元素
                allAchievement = allAchievement.Where(yearGroup => yearGroup.Year == SearchBarYear);
            }

            // 其实当 搜索栏 选择了某个年份时，allAchievement 中只有一个元素，也就是下面的循环只执行一次；
            // 但是若 搜索栏 中未选择年份时，则对所有年份逐个运行检查。
            foreach (YearAchievements yearGroup in allAchievement)
            {
                IQueryable<Achievement> query = (yearGroup.Achievements).AsQueryable();

                if (SearchBarTitle is not "")
                    query = query.Where(achievement => (achievement.Title).Contains(SearchBarTitle));
                if (SearchBarContent is not "")
                    query = query.Where(
                        achievement => (achievement.Content).Contains(SearchBarContent));
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

        private async void DeleteAchievement()
        {
            if (selectedAchievement is null)
                return;
            ButtonResult boxResult = await messageBoxService.ShowAsync("警告", "是否删除？");
            if (boxResult == ButtonResult.Cancel)
                return;

            _ = loadingService.RunWithLoadingAsync(async () =>
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

        private int isShowAddEdit = 0;
        /// <summary>
        /// Transitioner的SelectedIndex，0：不显示AddEdit，1：显示AddEdit。
        /// </summary>
        public int IsShowAddEdit
        {
            get { return isShowAddEdit; }
            set { SetProperty(ref isShowAddEdit, value); }
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
            Level = 1;
            Category = AchievementCategory.Default;

            IsShowAddEdit = 1;
        }

        private void OpenEdit()
        {
            TitleAddEdit = "编辑";
            if (selectedAchievement is null)
                return;
            AchieveDate = selectedAchievement.AchieveDate ?? DateTime.Now;
            Title = selectedAchievement.Title;
            Content = selectedAchievement.Content;
            Level = selectedAchievement.Level;
            Category = selectedAchievement.Category;

            IsShowAddEdit = 1;
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
        private int level = 1;
        public int Level
        {
            get { return level; }
            set { SetProperty(ref level, value); }
        }
        private string category = AchievementCategory.Default;
        public string Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }
        #endregion

        public DelegateCommand SaveCommand { get; private set; }
        private void Save()
        {
            if (ValidateAchievement(out string errorMessage) is false)
            {
                snackbarService.SendMessage(errorMessage);
                return;
            }
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
        private bool ValidateAchievement(out string errorMessage)
        {
            // 1. 日期不能为空或默认值
            if (achieveDate == default)
            {
                errorMessage = "日期不能为空，请选择一个有效日期。";
                return false;
            }

            // 2. 日期不能大于现在（未来时间不允许）
            if (achieveDate > DateTime.Now)
            {
                errorMessage = "日期不能晚于当前时间。";
                return false;
            }

            // 3. 标题不能为空
            // string.IsNullOrEmpty 只能判断下面的前两种情况
            // 判断字符串是否为：1.null 2.""（空字符串） 3." "（空格） 4."\n" "\t" 等空白字符
            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage = "标题不能为空。";
                return false;
            }

            // 4. 内容不能为空
            if (string.IsNullOrWhiteSpace(content))
            {
                errorMessage = "内容不能为空。";
                return false;
            }

            //5.Level 必须在合法区间（1–5）
            if (level < 1 || level > 5)
            {
                errorMessage = "重要程度星级必须在 1 到 5 之间。";
                return false;
            }

            // 6. 分类不能为空
            if (string.IsNullOrWhiteSpace(category))
            {
                errorMessage = "类别不能为空。";
                return false;
            }

            // 7. 图片路径不能为空
            //if (string.IsNullOrWhiteSpace(imagePath))
            //{
            //    errorMessage = "图片路径不能为空，请上传图片。";
            //    return false;
            //}

            // 全部通过
            errorMessage = string.Empty;
            return true;
        }

        private void AddAchievement()
        {
            Achievement achievement = new()
            {
                UserId = userSession.CurrentUser.Id,
                AchieveDate = achieveDate,
                Title = title,
                Content = content,
                Level = level,
                Category = category,
                ImagePath = "ImgPath"
            };

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                ApiResult<Achievement> apiResult = await service.CreateAchievementAsync(achievement);
                if (apiResult.IsSuccess is false)
                {
                    snackbarService.SendMessage(apiResult.ErrorMessage!);
                    return;
                }

                //Achievement newAchievement = apiResult.Data!;
                IsShowAddEdit = 0;
                // 下面两个函数中都有 SetAllAchievement(localAllAchievement);，
                // 所以在 ClearSearchBar(); 中的那一次调用属于多余调用
                await InitData();
                ClearSearchBar();
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

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                ApiResult apiResult = await service.UpdateAchievementAsync(selectedAchievement);
                if (apiResult.IsSuccess is true)
                {
                    IsShowAddEdit = 0;
                    await InitData();
                    ClearSearchBar();
                }
                else
                {
                    snackbarService.SendMessage(apiResult.ErrorMessage!);
                }
            });
        }
        public DelegateCommand CancelCommand { get; private set; }
        #endregion 新增、编辑成就页面
    }
}
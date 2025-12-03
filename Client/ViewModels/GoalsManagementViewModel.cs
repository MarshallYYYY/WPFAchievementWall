using Client.Common;
using Client.Services;
using Client.Services.WebApi;
using Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.ViewModels
{
    public class GoalsManagementViewModel : BindableBase
    {
        public GoalsManagementViewModel(
            GoalService goalService,
            ILoadingService loadingService,
            IMessageBoxService messageBoxService,
            ISnackbarService snackbarService,
            IUserSession userSession)
        {
            service = goalService;
            this.loadingService = loadingService;
            this.messageBoxService = messageBoxService;
            this.snackbarService = snackbarService;
            this.userSession = userSession;

            _ = loadingService.RunWithLoadingAsync(InitData);

            DeleteCommand = new DelegateCommand<Goal>(Delete);
            OpenAddCommand = new DelegateCommand(OpenAdd);
            OpenEditCommand = new DelegateCommand<Goal>(OpenEdit);
            SaveCommand = new DelegateCommand(Save);
            AchieveCommand = new DelegateCommand<Goal>(Achieve);
            CancelCommand = new(() => IsShowAddEdit = 0);
        }

        #region 服务和数据
        private readonly GoalService service;
        private readonly ILoadingService loadingService;
        private readonly IMessageBoxService messageBoxService;
        private readonly ISnackbarService snackbarService;
        private readonly IUserSession userSession;

        private List<Goal> goals = [];
        public ObservableCollection<Goal> OngoingGoals { get; set; } = [];
        public ObservableCollection<Goal> AchievedGoals { get; set; } = [];
        private async Task InitData()
        {
            goals.Clear();
            OngoingGoals.Clear();
            AchievedGoals.Clear();

            goals = await service.GetGoalsByUserIdAsync(userSession.CurrentUser.Id);

            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is null).OrderBy(goal => goal.TargetDate))
            {
                OngoingGoals.Add(goal);
            }

            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is not null).OrderByDescending(goal => goal.AchieveDate))
            {
                AchievedGoals.Add(goal);
            }
        }
        #endregion

        #region 共用的删除功能
        public DelegateCommand<Goal> DeleteCommand { get; private set; }
        private async void Delete(Goal goal)
        {
            ButtonResult boxResult = await messageBoxService.ShowAsync("警告", "是否删除？");
            if (boxResult == ButtonResult.Cancel)
                return;

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                bool result = await service.DeleteGoalAsync(goal.Id);
                if (result is true)
                    await InitData();
            });
        }
        #endregion

        #region OngoingGoals 新增、编辑、达成 功能

        private int isAddEditShow = 0;
        /// <summary>
        /// Transitioner的SelectedIndex，0：不显示AddEdit，1：显示AddEdit。
        /// </summary>
        public int IsShowAddEdit
        {
            get { return isAddEditShow; }
            set { SetProperty(ref isAddEditShow, value); }
        }
        private string titleAddEdit = "新增";
        public string TitleAddEdit
        {
            get { return titleAddEdit; }
            set { SetProperty(ref titleAddEdit, value); }
        }
        #region 数据
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
        private DateTime targetDate;
        public DateTime TargetDate
        {
            get { return targetDate; }
            set { SetProperty(ref targetDate, value); }
        }
        #endregion
        public DelegateCommand OpenAddCommand { get; private set; }
        public DelegateCommand<Goal> OpenEditCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<Goal> AchieveCommand { get; private set; }

        private void OpenAdd()
        {
            TitleAddEdit = "新增";
            Title = "";
            Content = "";
            TargetDate = DateTime.Now;

            IsShowAddEdit = 1;
        }
        /// <summary>
        /// 要进行编辑更新的Goal
        /// </summary>
        private Goal goalEdit = new() { AchieveDate = null };
        private void OpenEdit(Goal goal)
        {
            TitleAddEdit = "编辑";
            //if (goal is null)
            //    return;
            goalEdit = goal;

            Title = goalEdit.Title;
            Content = goalEdit.Content;
            TargetDate = goalEdit.TargetDate;

            IsShowAddEdit = 1;
        }
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
                    AddGoal();
                    break;
                case "编辑":
                    EditGoal();
                    break;
                default:
                    break;
            }
        }
        private bool ValidateAchievement(out string errorMessage)
        {
            // 日期不能为空或默认值
            if (targetDate == default)
            {
                errorMessage = "日期不能为空，请选择一个有效日期。";
                return false;
            }

            // 日期不能小于现在（目标的预期完成时间只能是将来）
            // 这个跟 AchievementDisplayViewModel 中的 AchieveDate 的逻辑是相反的，要特别注意
            if (targetDate.DayOfYear < DateTime.Now.DayOfYear)
            {
                errorMessage = "日期不能早于当前时间。";
                return false;
            }

            // 标题不能为空
            // string.IsNullOrEmpty 只能判断下面的前两种情况
            // 判断字符串是否为：1.null 2.""（空字符串） 3." "（空格） 4."\n" "\t" 等空白字符
            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage = "标题不能为空。";
                return false;
            }

            // 内容不能为空
            if (string.IsNullOrWhiteSpace(content))
            {
                errorMessage = "内容不能为空。";
                return false;
            }

            // 全部通过
            errorMessage = string.Empty;
            return true;
        }
        private void AddGoal()
        {
            Goal goal = new()
            {
                UserId = userSession.CurrentUser.Id,
                Title = title,
                Content = content,
                TargetDate = targetDate,
                AchieveDate = null
            };

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                Goal newGoal = await service.CreateGoalAsync(goal);
                if (newGoal is not null)
                {
                    IsShowAddEdit = 0;
                    await InitData();
                }
            });
        }
        private void EditGoal()
        {
            goalEdit.Title = title;
            goalEdit.Content = content;
            goalEdit.TargetDate = targetDate;

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                bool result = await service.UpdateGoalAsync(goalEdit);
                if (result is true)
                {
                    IsShowAddEdit = 0;
                    await InitData();
                }
            });
        }
        private async void Achieve(Goal goal)
        {
            goal.AchieveDate = DateTime.Now;

            ButtonResult boxResult = await messageBoxService.ShowAsync("提示", "将该目标设置为完成吗？");
            if (boxResult == ButtonResult.Cancel)
                return;

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                bool result = await service.UpdateGoalAsync(goal);
                if (result is true)
                {
                    await InitData();
                }
            });
        }
        public DelegateCommand CancelCommand { get; private set; }
        #endregion
    }
}
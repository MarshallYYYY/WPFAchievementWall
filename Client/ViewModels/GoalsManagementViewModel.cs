using Client.Events;
using Client.Services;
using Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Client.ViewModels
{
    public class GoalsManagementViewModel : BindableBase
    {
        public GoalsManagementViewModel(GoalService goalService)
        {
            service = goalService;
            _ = LoadingHelper.RunWithLoadingAsync(InitData);
            DeleteCommand = new(Delete);
        }

        private readonly GoalService service;

        private List<Goal> goals = [];
        public ObservableCollection<Goal> OngoingGoals { get; set; } = [];
        public ObservableCollection<Goal> AchievedGoals { get; set; } = [];
        private async Task InitData()
        {
            goals.Clear();
            OngoingGoals.Clear();
            AchievedGoals.Clear();

            goals = await service.GetGoalsAsync();

            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is null).OrderByDescending(goal => goal.TargetDate))
            {
                OngoingGoals.Add(goal);
            }

            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is not null).OrderByDescending(goal => goal.AchieveDate))
            {
                AchievedGoals.Add(goal);
            }
        }
        public DelegateCommand<Goal> DeleteCommand { get; private set; }
        private void Delete(Goal goal)
        {
            MessageBoxResult boxResult = MessageBox.Show(
                "是否删除？", "警告",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (boxResult is MessageBoxResult.No)
                return;

            _ = LoadingHelper.RunWithLoadingAsync(async () =>
            {
                bool result = await service.DeleteGoalAsync(goal.Id);
                if (result is true)
                    await InitData();
            });
        }
    }
}
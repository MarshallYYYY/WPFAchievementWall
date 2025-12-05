using Client.Models;
using Models;
using System.Collections.ObjectModel;

namespace Client.Services.WebApi
{
    public class GoalService : ApiServiceBase, IGoalService
    {
        public GoalService(string baseUrl) : base(baseUrl) { }

        public async Task<ApiResult<List<Goal>>> GetUserGoalsAsync(int userId)
        {
            return await GetAsync<List<Goal>>($"api/goals?userid={userId}");
        }
        public async Task<ApiResult> SplitUserGoalsAsync(int userId, List<Goal> goals,
            ObservableCollection<Goal> ongoingGoals,
            ObservableCollection<Goal> achievedGoals)
        {
            goals.Clear();
            ongoingGoals.Clear();
            achievedGoals.Clear();

            ApiResult<List<Goal>> apiResult = await GetUserGoalsAsync(userId);
            if (apiResult.IsSuccess is false)
            {
                return new ApiResult()
                {
                    IsSuccess = false,
                    ErrorMessage = apiResult.ErrorMessage,
                };
            }

            goals = apiResult.Data!;
            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is null).OrderBy(goal => goal.TargetDate))
            {
                ongoingGoals.Add(goal);
            }

            foreach (Goal goal in
                goals.Where(goal => goal.AchieveDate is not null).OrderByDescending(goal => goal.AchieveDate))
            {
                achievedGoals.Add(goal);
            }

            return new ApiResult()
            {
                IsSuccess = true,
                ErrorMessage = null,
            };
        }

        public async Task<ApiResult<Goal>> CreateGoalAsync(Goal goal)
        {
            return await PostAsync<Goal>("api/goals", goal);
        }

        public async Task<ApiResult> UpdateGoalAsync(Goal goal)
        {
            return await PutAsync($"api/goals/{goal.Id}", goal);
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            return await DeleteAsync($"api/goals/{id}");
        }
    }
}
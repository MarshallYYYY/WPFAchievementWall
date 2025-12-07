using Client.Models;
using Models;
using System.Collections.ObjectModel;

namespace Client.Services.WebApi
{
    public class GoalService : ApiServiceBase, IGoalService
    {
        public GoalService(string baseUrl) : base(baseUrl) { }
        private const string endpointPrefix = "api/goals";

        public async Task<ApiResult<List<Goal>>> GetUserGoalsAsync(int userId)
        {
            return await GetAsync<List<Goal>>($"{endpointPrefix}?userid={userId}");
        }
        public async Task<ApiResult> SplitUserGoalsAsync(int userId,
            ObservableCollection<Goal> ongoingGoals,
            ObservableCollection<Goal> achievedGoals)
        {
            ApiResult<List<Goal>> apiResult = await GetUserGoalsAsync(userId);
            if (apiResult.IsSuccess is false)
            {
                return new ApiResult()
                {
                    IsSuccess = false,
                    ErrorMessage = apiResult.ErrorMessage,
                };
            }

            List<Goal> goals = apiResult.Data!;
            ongoingGoals.Clear();
            achievedGoals.Clear();

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
            return await PostAsync<Goal>(endpointPrefix, goal);
        }

        public async Task<ApiResult> UpdateGoalAsync(Goal goal)
        {
            return await PutAsync($"{endpointPrefix}/{goal.Id}", goal);
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            return await DeleteAsync($"{endpointPrefix}/{id}");
        }
    }
}
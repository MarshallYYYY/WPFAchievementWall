using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public class GoalService : ApiServiceBase
    {
        public GoalService(string baseUrl) : base(baseUrl) { }

        public async Task<ApiResult<List<Goal>>> GetGoalsByUserIdAsync(int userId)
        {
            return await GetAsync<List<Goal>>($"api/goals?userid={userId}");
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

using Models;

namespace Client.Services.WebApi
{
    public class GoalService : ApiServiceBase
    {
        public GoalService(string baseUrl) : base(baseUrl) { }

        public async Task<List<Goal>> GetGoalsAsync()
        {
            return await GetAsync<List<Goal>>("api/goals");
        }

        public async Task<Goal> GetGoalAsync(int id)
        {
            return await GetAsync<Goal>($"api/goals/{id}");
        }

        public async Task<Goal> CreateGoalAsync(Goal goal)
        {
            return await PostAsync<Goal>("api/goals", goal);
        }

        public async Task<bool> UpdateGoalAsync(Goal goal)
        {
            return await PutAsync($"api/goals/{goal.Id}", goal);
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            return await DeleteAsync($"api/goals/{id}");
        }
    }
}

using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public class AchievementService : ApiServiceBase
    {
        public AchievementService(string baseUrl) : base(baseUrl) { }

        public async Task<ApiResult<List<Achievement>>> GetAchievementsByUserIdAsync(int userId)
        {
            string endpoint = $"api/achievements?userId={userId}";
            return await GetAsync<List<Achievement>>(endpoint);
        }

        public async Task<ApiResult<Achievement>> CreateAchievementAsync(Achievement achievement)
        {
            return await PostAsync<Achievement>("api/achievements", achievement);
        }

        public async Task<bool> UpdateAchievementAsync(Achievement achievement)
        {
            return await PutAsync($"api/achievements/{achievement.Id}", achievement);
        }

        public async Task<bool> DeleteAchievementAsync(int id)
        {
            return await DeleteAsync($"api/achievements/{id}");
        }
    }
}

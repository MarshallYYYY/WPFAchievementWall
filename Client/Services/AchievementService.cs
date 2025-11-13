using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AchievementService : ApiServiceBase
    {
        public AchievementService(string baseUrl) : base(baseUrl) { }

        public async Task<List<Achievement>> GetAchievementsAsync()
        {
            return await GetAsync<List<Achievement>>("api/achievements");
        }

        public async Task<Achievement> GetAchievementAsync(int id)
        {
            return await GetAsync<Achievement>($"api/achievements/{id}");
        }

        public async Task<Achievement> CreateAchievementAsync(Achievement achievement)
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

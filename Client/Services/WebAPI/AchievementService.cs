using Models;

namespace Client.Services.WebApi
{
    public class AchievementService : ApiServiceBase
    {
        public AchievementService(string baseUrl) : base(baseUrl) { }

        public async Task<List<Achievement>> GetAchievementsAsync()
        {
            return await GetAsync<List<Achievement>>("api/achievements");
        }
        public async Task<List<Achievement>> GetAchievementsByTitleAsync(string title)
        {
            // Uri.EscapeDataString(title)
            // 处理特殊情况：空格、中文、? & = / 等特殊字符、非 ASCII 字符
            // 把字符串安全地转换成 URL 可用的格式（URL 编码），也叫 percent-encoding 百分号编码。
            string endpoint = $"api/achievements?title={Uri.EscapeDataString(title)}";
            return await GetAsync<List<Achievement>>(endpoint);
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

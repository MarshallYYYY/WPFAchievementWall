using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public class AchievementService : ApiServiceBase, IAchievementService
    {
        public AchievementService(string baseUrl) : base(baseUrl) { }

        public async Task<ApiResult<List<Achievement>>> GetUserAchievementsdAsync(int userId)
        {
            return await GetAsync<List<Achievement>>($"api/achievements?userId={userId}");
        }

        public async Task<ApiResult> SetUserAchievementsGroupedByYearAsync(
            int userId, List<YearAchievements> result)
        {
            result.Clear();
            ApiResult<List<Achievement>> apiResult = await GetUserAchievementsdAsync(userId);
            if (apiResult.IsSuccess is false)
            {
                return new ApiResult()
                {
                    IsSuccess = false,
                    ErrorMessage = apiResult.ErrorMessage,
                };
            }
            // 获取所有的成就
            List<Achievement> achievements = apiResult.Data!;
            // 将所有的成就按照年份分组
            List<YearAchievements> yearGroups = achievements
                // 每个 group 的 Key = 年份（int?）
                // 每个 group 的元素 = 该年份下的 Achievement 的列表
                // GroupBy 返回：IEnumerable<IGrouping<TKey, TElement>>
                .GroupBy(achievement => achievement.AchieveDate?.Year)
                // 按年份降序排列（最新年份排最前）
                // group.Key 是刚刚 GroupBy 生成的 key（year）
                // group 的数据类型是：IGrouping<TKey, TElement>
                // group 本质上就是一个：带有 Key 的 Achievement 列表
                .OrderByDescending(group => group.Key)
                // Select 投影（转换为YearAchievements）
                // 将每个 group 映射成 YearAchievements 类
                .Select(group => new YearAchievements
                {
                    Year = Convert.ToInt32(group.Key),
                    // 把每个年份内部的成就列表做降序排列（最近的成就排前面）
                    Achievements = group.OrderByDescending(achievement => achievement.AchieveDate).ToList(),

                    // 调用构造函数来进行赋值
                    //Achievements = new List<Achievement>(
                    //    group.OrderByDescending(achievement => achievement.AchieveDate).ToList())
                })
                .ToList();
            // 按照年份依次添加到 AllAchievement 中
            //yearGroups.ForEach(group => result.Add(group));
            yearGroups.ForEach(result.Add);

            return new ApiResult()
            {
                IsSuccess = true,
                ErrorMessage = null,
            };
        }

        public async Task<ApiResult<Achievement>> CreateAchievementAsync(Achievement achievement)
        {
            return await PostAsync<Achievement>("api/achievements", achievement);
        }

        public async Task<ApiResult> UpdateAchievementAsync(Achievement achievement)
        {
            return await PutAsync($"api/achievements/{achievement.Id}", achievement);
        }

        public async Task<bool> DeleteAchievementAsync(int id)
        {
            return await DeleteAsync($"api/achievements/{id}");
        }
    }
}
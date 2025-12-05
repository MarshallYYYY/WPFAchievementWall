using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public interface IAchievementService
    {
        /// <summary>
        /// 获取用户的所有成就数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<ApiResult<List<Achievement>>> GetUserAchievementsdAsync(int userId);
        /// <summary>
        /// 填充result：获取用户的所有成就数据，并按照年份进行分组，且按照时间排序。
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<ApiResult> SetUserAchievementsGroupedByYearAsync(
            int userId, List<YearAchievements> result);
        Task<ApiResult<Achievement>> CreateAchievementAsync(Achievement achievement);
        Task<ApiResult> UpdateAchievementAsync(Achievement achievement);
        Task<bool> DeleteAchievementAsync(int id);
    }
}
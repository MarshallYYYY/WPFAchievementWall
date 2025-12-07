using Client.Models;
using Models;
using System.Collections.ObjectModel;

namespace Client.Services.WebApi
{
    public interface IGoalService
    {
        Task<ApiResult<List<Goal>>> GetUserGoalsAsync(int userId);
        Task<ApiResult> SplitUserGoalsAsync(int userId,
            ObservableCollection<Goal> ongoingGoals,
            ObservableCollection<Goal> achievedGoals);
        Task<ApiResult<Goal>> CreateGoalAsync(Goal goal);
        Task<ApiResult> UpdateGoalAsync(Goal goal);
        Task<bool> DeleteGoalAsync(int id);
    }
}
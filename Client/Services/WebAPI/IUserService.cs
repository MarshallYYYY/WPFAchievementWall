using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public interface IUserService
    {
        Task<ApiResult<User>> GetUserForLoginAsync(string userName, string password);
        Task<ApiResult<User>> CreateUserAsync(User user);
        Task<ApiResult> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}
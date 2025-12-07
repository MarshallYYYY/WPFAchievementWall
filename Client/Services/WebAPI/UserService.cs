using Client.Models;
using Models;

namespace Client.Services.WebApi
{
    public class UserService : ApiServiceBase, IUserService
    {
        public UserService(string baseUrl) : base(baseUrl) { }
        private const string endpointPrefix = "api/users";

        public async Task<ApiResult<User>> GetUserForLoginAsync(string userName, string password)
        {
            // Uri.EscapeDataString(title)
            // 处理特殊情况：空格、中文、? & = / 等特殊字符、非 ASCII 字符
            // 把字符串安全地转换成 URL 可用的格式（URL 编码），也叫 percent-encoding 百分号编码。
            ApiResult<User> apiResult = await GetAsync<User>($"{endpointPrefix}?" +
                $"userName={Uri.EscapeDataString(userName)}&" +
                $"password={Uri.EscapeDataString(password)}");
            return apiResult;
        }

        public async Task<ApiResult<User>> CreateUserAsync(User user)
        {
            return await PostAsync<User>(endpointPrefix, user);
        }

        public async Task<ApiResult> UpdateUserAsync(User user)
        {
            return await PutAsync($"{endpointPrefix}/{user.Id}", user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await DeleteAsync($"{endpointPrefix}/{id}");
        }
    }
}
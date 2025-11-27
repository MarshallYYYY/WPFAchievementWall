using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    public class UserService : ApiServiceBase
    {
        public UserService(string baseUrl) : base(baseUrl) { }

        public async Task<List<User>> GetUsersAsync()
        {
            return await GetAsync<List<User>>("api/users");
        }

        public async Task<User> GetUserAsyncForLogin(string userName, string password)
        {
            var (user, errorMessage) = await GetAsyncWithErrorMessage<User>($"api/users?" +
                $"userName={Uri.EscapeDataString(userName)}&" +
                $"password={Uri.EscapeDataString(password)}");
            if (errorMessage is not null)
            {
                MessageBox.Show(errorMessage);
                return null!;
            }
            return user!;
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await GetAsync<User>($"api/users/{id}");
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await PostAsync<User>("api/users", user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await PutAsync($"api/users/{user.Id}", user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await DeleteAsync($"api/users/{id}");
        }
    }
}

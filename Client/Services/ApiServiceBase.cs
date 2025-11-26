using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public abstract class ApiServiceBase
    {
        protected readonly HttpClient _httpClient;
        /// <summary>
        /// http://localhost:5045
        /// </summary>
        protected readonly string _baseUrl;

        protected ApiServiceBase(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        protected async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            T? result = JsonConvert.DeserializeObject<T>(content);
            // 检查反序列化结果
            return result ?? throw new Exception($"反序列化失败，无法将内容转换为 {typeof(T).Name}");
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            // PostAsync：发送 POST 请求
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            T? result = JsonConvert.DeserializeObject<T>(responseContent);
            // 检查反序列化结果
            return result ?? throw new Exception($"反序列化失败，无法将内容转换为 {typeof(T).Name}");
        }

        protected async Task<bool> PutAsync(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
            return response.IsSuccessStatusCode;
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            return response.IsSuccessStatusCode;
        }
    }
}

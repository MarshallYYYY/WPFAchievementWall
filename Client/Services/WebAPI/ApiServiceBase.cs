using Client.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Client.Services.WebApi
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

        protected async Task<ApiResult<T>> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");

            // 这行代码不能添加，添加的话，在遇到来自WebAPI返回的错误信息时，会直接自动弹窗报错，
            // 而不会返回我们在WebAPI的Controller中自定义的错误信息了
            //response.EnsureSuccessStatusCode();
            // https://www.cnblogs.com/lovefoolself/p/18401391

            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode is false)
            {
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    ErrorMessage = content,
                };
            }

            // 成功，解析实体
            T? data = JsonConvert.DeserializeObject<T>(content);
            return new ApiResult<T>()
            {
                IsSuccess = true,
                Data = data,
            };
        }

        //protected async Task<T> PostAsync<T>(string endpoint, object data)
        //{
        //    string json = JsonConvert.SerializeObject(data);
        //    StringContent content = new(json, Encoding.UTF8, "application/json");
        //    // PostAsync：发送 POST 请求
        //    HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
        //    response.EnsureSuccessStatusCode();
        //    string responseContent = await response.Content.ReadAsStringAsync();
        //    T? result = JsonConvert.DeserializeObject<T>(responseContent);
        //    // 检查反序列化结果
        //    return result ?? throw new Exception($"反序列化失败，无法将内容转换为 {typeof(T).Name}");
        //}

        protected async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            // PostAsync：发送 POST 请求
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode is false)
            {
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    ErrorMessage = responseContent,
                };
            }

            // 成功，解析实体
            T? newData = JsonConvert.DeserializeObject<T>(responseContent);
            return new ApiResult<T>()
            {
                IsSuccess = true,
                Data = newData,
            };
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

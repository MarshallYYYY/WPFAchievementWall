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

            /*
            这行代码不能添加，添加的话，在遇到来自WebAPI返回的错误信息时，会直接自动弹窗报错，
            而不会返回我们在WebAPI的Controller中自定义的错误信息了。
            参考链接：https://www.cnblogs.com/lovefoolself/p/18401391
            */
            //response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            bool isSuccess = response.IsSuccessStatusCode;
            return new ApiResult<T>()
            {
                IsSuccess = isSuccess,
                // 成功就解析实体，失败则返回default。
                Data = isSuccess ? JsonConvert.DeserializeObject<T>(content) : default,
                // 成功就返回null，失败则就返回错误信息。
                ErrorMessage = isSuccess ? null : content,
            };
            /*
            if (response.IsSuccessStatusCode is false)
            {
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    ErrorMessage = strContent,
                };
            }
            else
            {
                // 成功，解析实体
                T? data = JsonConvert.DeserializeObject<T>(strContent);
                return new ApiResult<T>()
                {
                    IsSuccess = true,
                    Data = data,
                };
            }
            */
        }

        /*
        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            // PostAsync：发送 POST 请求
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            T? isSuccess = JsonConvert.DeserializeObject<T>(responseContent);
            // 检查反序列化结果
            return isSuccess ?? throw new Exception($"反序列化失败，无法将内容转换为 {typeof(T).Name}");
        }
        */

        protected async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent strContent = new(json, Encoding.UTF8, "application/json");
            // PostAsync：发送 POST 请求
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", strContent);

            string content = await response.Content.ReadAsStringAsync();

            bool isSuccess = response.IsSuccessStatusCode;
            return new ApiResult<T>()
            {
                IsSuccess = isSuccess,
                Data = isSuccess ? JsonConvert.DeserializeObject<T>(content) : default,
                ErrorMessage = isSuccess ? null : content,
            };
        }

        protected async Task<ApiResult> PutAsync(string endpoint, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);

            bool isSuccess = response.IsSuccessStatusCode;
            return new ApiResult()
            {
                IsSuccess = isSuccess,
                ErrorMessage = isSuccess ? null : await response.Content.ReadAsStringAsync(),
            };
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            return response.IsSuccessStatusCode;
        }
    }
}

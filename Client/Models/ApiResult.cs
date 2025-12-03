namespace Client.Models
{
    /// <summary>
    /// GET/POST 用 ApiResult<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        /// <summary>
        /// IsSuccess为true时，Data有值，否则为空。
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// IsSuccess为false时，ErrorMessage有值，否则为空。
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
    /// <summary>
    /// PUT/DELETE 用 ApiResult
    /// </summary>
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
namespace Models
{
    public class User
    {
        // 主键，自增
        public int Id { get; set; }
        // 非空，唯一
        public string? UserName { get; set; }
        // 非空
        public string? Password { get; set; }
        // 非空
        public string? AvatarPath { get; set; }
        // 非空，默认值为当前时间
        public DateTime CreateTime { get; set; }
    }
}
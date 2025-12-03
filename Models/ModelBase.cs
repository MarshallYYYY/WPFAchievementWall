namespace Models
{
    public class ModelBase
    {
        // 主键，自增
        public int Id { get; set; }
        // TODO：增加UserId，不同用户显示各自的成就和目标
        public int UserId { get; set; }
        // 非空
        public string Title { get; set; } = string.Empty;
        // 非空
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 成就达成日期，在 Goal 中可空
        /// </summary>
        public DateTime? AchieveDate { get; set; }
    }
}
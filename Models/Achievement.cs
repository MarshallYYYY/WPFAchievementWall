namespace Models
{
    // 单个成就项
    public class Achievement : ModelBase
    {
        // 重要程度(1-5星)：ImportanceLevel
        public int Level { get; set; } = 1;
        // 非空
        public string ImagePath { get; set; } = string.Empty;
        // 非空，有默认值
        public string Category { get; set; } = AchievementCategory.Default;
    }
}
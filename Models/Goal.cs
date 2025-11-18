namespace Models
{
    public class Goal : ModelBase
    {
        /// <summary>
        /// 目标日期：预计完成目标的时间；非空
        /// </summary>
        public DateTime TargetDate { get; set; }
    }
}
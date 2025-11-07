using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Models
{
    //public class Achievement
    //{
    //    public int Id { get; set; }
    //    public string? Title { get; set; }        // 成就标题
    //    public string? Description { get; set; }  // 详细描述
    //    public DateTime Date { get; set; }       // 达成日期
    //    public CategoryApperance Category { get; set; } // 分类
    //    public string? ImagePath { get; set; }    // 成就图片
    //    public int ImportanceLevel { get; set; } // 重要程度(1-5星)
    //}
    //public enum CategoryApperance
    //{
    //    学习成长,    // 学会新技能、考取证书等
    //    职业发展,    // 升职加薪、项目成功等
    //    个人生活,    // 旅行、爱好、个人突破等
    //    健康运动,    // 健身成果、体育成就等
    //    旅行经历,
    //    默认,
    //}
    // 年度成就集合


    // 单个成就项
    public class Achievement
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        // 成就达成日期
        public DateTime Date { get; set; }
        // 重要程度(1-5星)：ImportanceLevel
        public int Level { get; set; }
        public string? ImagePath { get; set; }
        //public string? Category { get; set; }
        public AchievementCategory Category { get; set; }
    }
}
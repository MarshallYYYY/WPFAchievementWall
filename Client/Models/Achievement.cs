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
    // 单个成就项
    public class Achievement : ModelBase
    {
        // 重要程度(1-5星)：ImportanceLevel
        public int Level { get; set; }
        public string? ImagePath { get; set; }
        //public string? Category { get; set; }
        public AchievementCategory Category { get; set; }
    }
}
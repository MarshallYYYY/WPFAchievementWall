using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class AchievementCategoryInfo(string name, string icon, string color)
    {
        // IDE0290：使用主构造函数

        // 只读属性，保证对象不可变性
        public string Name { get; set; } = name;
        public string Icon { get; } = icon;
        public string Color { get; } = color;
    }
}
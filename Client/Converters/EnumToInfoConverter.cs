using Client.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.Converters
{
    public class EnumToInfoConverter : IValueConverter
    {
        private static readonly Dictionary<AchievementCategory, AchievementCategoryInfo> dict = new()
        {
            {AchievementCategory.Default, new ("默认", "✨", "#FFFFCA28")},
            {AchievementCategory.Life, new ("生活经历", "👨‍👩‍👧‍👦", "#FF6B35")},
            {AchievementCategory.Learning, new ("学习成长", "📚", "#FF2E86AB")},
            {AchievementCategory.Health, new ("健康运动", "🏃", "#FFE74C3C")},
            {AchievementCategory.Career, new ("职业发展", "💼", "#FF8E44AD")},
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AchievementCategory category &&
                dict.TryGetValue(category, out var info))
            {
                string? propertyName = parameter as string;
                return propertyName switch
                {
                    "Name" => info.Name,
                    "Icon" => info.Icon,
                    "Color" => info.Color,
                    _ => info.Name
                };
            }
            return "默认"; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

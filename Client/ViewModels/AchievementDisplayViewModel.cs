using Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class AchievementDisplayViewModel : BindableBase
    {
        public AchievementDisplayViewModel()
        {
            InitAllAchievement();
        }
        public ObservableCollection<YearAchievements> AllAchievement { get; set; } = [];

        public void InitAllAchievement()
        {
            YearAchievements yearAchievements2025 = new()
            {
                Year = "2025",
                Achievements = new ObservableCollection<Achievement>()
                {
                    new Achievement()
                    {
                        Title = "111",
                        Content = "大风大酒店开发活动懂法守法达到法定发大法师发卡机老大发哈三大发山东发撒",
                        Date = new DateTime(2025,1,1),
                        Level = 2,
                        Category = AchievementCategory.Life
                    },
                    new Achievement()
                    {
                        Title = "代发大是大非久啊",
                        Content = "多发点发大水大法师大萨达发撒",
                        Date = new DateTime(2025,1,1),
                        Level = 0,
                        Category = AchievementCategory.Default
                    },
                    new Achievement()
                    {
                        Title = "111",
                        Content = "111111",
                        Date = new DateTime(2025,1,1),
                        Level = 3,
                        Category = AchievementCategory.Learning
                    },
                    new Achievement()
                    {
                        Title = "111",
                        Content = "大风大酒店开发活动懂法守法达到法定发大法师发卡机老大发哈三大发山东发撒",
                        Date = new DateTime(2025,1,1),
                        Level = 5,
                        Category = AchievementCategory.Career
                    },
                    new Achievement()
                    {
                        Title = "111",
                        Content = "111111",
                        Date = new DateTime(2025,1,1),
                        Level = 1,
                        Category = AchievementCategory.Health
                    },
                }
            };
            YearAchievements yearAchievements2024 = yearAchievements2025;
            AllAchievement.Add(yearAchievements2025);
            AllAchievement.Add(yearAchievements2024);
        }
    }
}

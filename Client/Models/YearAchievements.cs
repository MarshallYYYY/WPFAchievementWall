using Models;
using System.Collections.ObjectModel;

namespace Client.Models
{
    public class YearAchievements
    {
        public int? Year { get; set; }
        public List<Achievement> Achievements { get; set; } = [];
    }
}
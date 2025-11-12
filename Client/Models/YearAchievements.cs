using Models;
using System.Collections.ObjectModel;

namespace Client.Models
{
    public class YearAchievements
    {
        public string? Year { get; set; }
        public ObservableCollection<Achievement>? Achievements { get; set; }
    }
}
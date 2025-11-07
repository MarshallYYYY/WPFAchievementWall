using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ModelBase
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        // 成就达成日期
        public DateTime AchieveDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ModelBase
    {
        // 主键，自增
        public int Id { get; set; }
        // 非空
        public string? Title { get; set; }
        // 非空
        public string? Content { get; set; }
        /// <summary>
        /// 成就达成日期，当 Goals 继承时可空
        /// </summary>
        public DateTime AchieveDate { get; set; }
    }
}
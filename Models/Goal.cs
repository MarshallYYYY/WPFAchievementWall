using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Goal:ModelBase
    {
        /// <summary>
        /// 目标日期：预计完成目标的时间
        /// </summary>
        public DateTime TargetDate { get; set; }
    }
}
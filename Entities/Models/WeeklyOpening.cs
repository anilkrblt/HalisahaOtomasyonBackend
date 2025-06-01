using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class WeeklyOpening
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }

        // Pazartesi=1 … Pazar=7
        public DayOfWeek DayOfWeek { get; set; }

        // O günün başlangıç ve bitiş saatleri
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
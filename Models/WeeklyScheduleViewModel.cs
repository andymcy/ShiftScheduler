using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    // ViewModel להעבר ל־View של השבוע הקרוב
    public class WeeklyScheduleViewModel
    {
        public List<WeeklyShift> Shifts { get; set; } = new();
    }

    // קישור בין Shift ותאריך ספציפי בשבוע הקרוב
    public class WeeklyShift
    {
        public Shift    Shift { get; set; } = null!;
        public DateTime Date  { get; set; }
    }
}

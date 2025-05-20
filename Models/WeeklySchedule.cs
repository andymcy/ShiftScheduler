using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class WeeklySchedule
    {
        public int      ScheduleId   { get; set; }
        public DateTime WeekStartDate{ get; set; }

        public ICollection<ShiftAssignment> Assignments { get; set; } = new List<ShiftAssignment>();
    }
}

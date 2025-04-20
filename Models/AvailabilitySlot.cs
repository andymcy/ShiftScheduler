using System;

namespace ShiftScheduler.Models
{
    public class AvailabilitySlot
    {
        public DayOfWeek Day  { get; set; }
        public ShiftTime Time { get; set; }

        public AvailabilitySlot(DayOfWeek day, ShiftTime time)
        {
            Day  = day;
            Time = time;
        }
    }
}

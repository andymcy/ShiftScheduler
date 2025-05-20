using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;using System;

namespace ShiftScheduler.Models.Entities
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
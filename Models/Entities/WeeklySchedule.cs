using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftScheduler.Models.Entities
{
    public class WeeklySchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public DateTime WeekStartDate { get; set; }

        // Navigation properties
        public ICollection<ShiftAssignment> ShiftAssignments { get; set; }

        public WeeklySchedule()
        {
            ShiftAssignments = new HashSet<ShiftAssignment>();
        }
    }
}

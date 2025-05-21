using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }

        [StringLength(40)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string DayOfWeek { get; set; } = string.Empty;

        // wide enough for “08:00-12:00”
        [StringLength(20)]
        public string ShiftTime { get; set; } = string.Empty;

        // navigation
        public ICollection<ShiftRequiredSkill> RequiredSkills { get; set; } = new List<ShiftRequiredSkill>();
        public ICollection<ShiftAssignment>    Assignments    { get; set; } = new List<ShiftAssignment>();
    }
}

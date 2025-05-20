using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int    ShiftId           { get; set; }
        public string Name              { get; set; } = "";
        public string DayOfWeek         { get; set; } = ""; // e.g. "Monday"
        public string ShiftTime         { get; set; } = ""; // "Morning"/"Evening"
        public int    RequiredEmployees { get; set; }

        public ICollection<ShiftRequiredSkill> RequiredSkills { get; set; } = new List<ShiftRequiredSkill>();
        public ICollection<ShiftAssignment>    Assignments    { get; set; } = new List<ShiftAssignment>();
    }
}

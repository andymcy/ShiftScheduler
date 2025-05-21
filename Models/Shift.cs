using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int    ShiftId    { get; set; }
        public string Name       { get; set; } = string.Empty;
        public string DayOfWeek  { get; set; } = string.Empty;

        // ▼ add (or rename an existing column to) this property
        public string ShiftTime  { get; set; } = string.Empty;   // no max-length constraint

        // navigation properties
        public ICollection<ShiftRequiredSkill> RequiredSkills  { get; set; } = new List<ShiftRequiredSkill>();
        public ICollection<ShiftAssignment>    Assignments     { get; set; } = new List<ShiftAssignment>();
    }
}

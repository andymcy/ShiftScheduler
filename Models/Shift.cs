using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int    Id                { get; set; }
        public string Name              { get; set; }
        public DayOfWeek Day            { get; set; }
        public ShiftTime ShiftTime      { get; set; }
        public int    RequiredEmployees { get; set; }
        public List<Skill> RequiredSkills { get; set; } = new List<Skill>();
        public Employee AssignedEmployee { get; set; }
        public Employee BackupEmployee   { get; set; }
    }
}

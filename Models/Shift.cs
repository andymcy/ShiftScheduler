using System;
using System.Collections.Generic;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int RequiredEmployees { get; set; }
        public List<string> RequiredSkills { get; set; } = new List<string>();
    }
}
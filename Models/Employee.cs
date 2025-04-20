using System;
using System.Collections.Generic;
using System.Linq;

namespace ShiftScheduler.Models
{
    public enum EmploymentType
    {
        Permanent,
        Temporary
    }

    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public EmploymentType Type { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek>();

        public bool IsAvailableForShift(Shift shift)
        {
            return AvailableDays.Contains(shift.Date.DayOfWeek);
        }

        public bool HasSkillsForShift(Shift shift)
        {
            return shift.RequiredSkills.All(skill => Skills.Contains(skill));
        }
    }
}